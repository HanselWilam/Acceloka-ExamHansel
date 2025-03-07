using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HanselAcceloka.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<TicketService> _logger;

        public TicketService(AccelokaContext db, ILogger<TicketService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<TicketModel>> Get(
            string? ticketCode = null,
            string? ticketName = null,
            string? categoryName = null,
            int? maxPrice = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? orderBy = "ticket_code",
            string? orderState = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _db.Tickets
                    .Include(q => q.category)
                    .Where(q => q.quota > 0)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(ticketCode))
                    query = query.Where(q => q.ticket_code.Contains(ticketCode));

                if (!string.IsNullOrEmpty(ticketName))
                    query = query.Where(q => q.ticket_name.Contains(ticketName));

                if (!string.IsNullOrEmpty(categoryName))
                    query = query.Where(q => q.category.category_name.Contains(categoryName));

                if (maxPrice.HasValue)
                    query = query.Where(q => q.price <= maxPrice.Value);

                if (startDate.HasValue)
                    query = query.Where(q => q.event_date >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(q => q.event_date <= endDate.Value);

                var allowedOrderByColumns = new HashSet<string>
                {
                    "ticket_code", "ticket_name", "category_name", "price", "event_date", "quota"
                };

                if (!allowedOrderByColumns.Contains(orderBy.ToLower()))
                {
                    _logger.LogWarning($"Invalid orderBy column: {orderBy}");
                    throw new ArgumentException("Invalid orderBy column.");
                }

                query = orderState.ToLower() == "desc"
                    ? query.OrderByDescending(q => EF.Property<object>(q, orderBy))
                    : query.OrderBy(q => EF.Property<object>(q, orderBy));

                int totalTickets = await query.CountAsync();

                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(q => new TicketModel
                    {
                        TicketCode = q.ticket_code,
                        TicketName = q.ticket_name,
                        Category = new CategoryModel { CategoryName = q.category.category_name },
                        Price = q.price,
                        Quota = q.quota,
                        EventDate = q.event_date
                    })
                    .ToListAsync();

                return new PagedResult<TicketModel>
                {
                    Items = data,
                    TotalCount = totalTickets
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available tickets.");
                throw;
            }
        }
    }
}