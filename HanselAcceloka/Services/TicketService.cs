using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using Microsoft.EntityFrameworkCore;

namespace HanselAcceloka.Services
{
    public class TicketService
    {
        private readonly AccelokaContext _db;

        public TicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<object> Get(
            string? ticketCode = null,
            string? ticketName = null,
            string? categoryName = null,
            int? maxPrice = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string orderBy = "ticket_code",
            string orderState = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _db.Tickets
                .Include(q => q.category)
                .AsQueryable();

            // Filter pencarian
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
                    EventDate = q.event_date.ToLocalTime()
                })
                .ToListAsync();

            return new { Tickets = data, TotalTickets = totalTickets };
        }
    }
}
