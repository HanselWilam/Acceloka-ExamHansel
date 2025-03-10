using HanselAcceloka.Application.Tickets.Queries;
using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HanselAcceloka.Application.Tickets.Handlers
{
    public class GetAvailableTicketsHandler : IRequestHandler<GetAvailableTicketsQuery, PagedResult<TicketModel>>
    {
        private readonly AccelokaContext _db;
        private readonly ILogger<GetAvailableTicketsHandler> _logger;

        public GetAvailableTicketsHandler(AccelokaContext db, ILogger<GetAvailableTicketsHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<PagedResult<TicketModel>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _db.Tickets
                    .Include(q => q.category)
                    .Where(q => q.quota > 0)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.TicketCode))
                    query = query.Where(q => q.ticket_code.Contains(request.TicketCode));

                if (!string.IsNullOrEmpty(request.TicketName))
                    query = query.Where(q => q.ticket_name.Contains(request.TicketName));

                if (!string.IsNullOrEmpty(request.CategoryName))
                    query = query.Where(q => q.category.category_name.Contains(request.CategoryName));

                if (request.MaxPrice.HasValue)
                    query = query.Where(q => q.price <= request.MaxPrice.Value);

                if (request.StartDate.HasValue)
                    query = query.Where(q => q.event_date >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(q => q.event_date <= request.EndDate.Value);

                var totalTickets = await query.CountAsync(cancellationToken);

                var data = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(q => new TicketModel
                    {
                        TicketCode = q.ticket_code,
                        TicketName = q.ticket_name,
                        Category = new CategoryModel { CategoryName = q.category.category_name },
                        Price = q.price,
                        Quota = q.quota,
                        EventDate = q.event_date
                    })
                    .ToListAsync(cancellationToken);

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