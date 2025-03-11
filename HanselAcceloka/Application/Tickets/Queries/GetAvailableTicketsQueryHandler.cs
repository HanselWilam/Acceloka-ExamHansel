using HanselAcceloka.Application.Tickets.Queries;
using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class GetAvailableTicketsQueryHandler : IRequestHandler<GetAvailableTicketsQuery, PagedResult<TicketModel>>
{
    private readonly AccelokaContext _db;
    private readonly ILogger<GetAvailableTicketsQueryHandler> _logger;

    public GetAvailableTicketsQueryHandler(AccelokaContext db, ILogger<GetAvailableTicketsQueryHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<TicketModel>> Handle(GetAvailableTicketsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Tickets
            .Include(q => q.category)
            .Where(q => q.quota > 0)
            .AsQueryable();

        // **Filtering**
        if (!string.IsNullOrEmpty(request.TicketCode))
            query = query.Where(q => q.ticket_code.Contains(request.TicketCode));

        if (!string.IsNullOrEmpty(request.TicketName))
            query = query.Where(q => q.ticket_name.Contains(request.TicketName));

        if (!string.IsNullOrEmpty(request.CategoryName))
            query = query.Where(q => q.category.category_name.Contains(request.CategoryName));

        if (request.MaxPrice.HasValue)
            query = query.Where(q => q.price <= request.MaxPrice.Value);

        var orderedQuery = request.OrderBy.ToLower() switch
        {
            "ticket_name" => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.ticket_name) : query.OrderByDescending(q => q.ticket_name),
            "category_name" => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.category.category_name) : query.OrderByDescending(q => q.category.category_name),
            "price" => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.price) : query.OrderByDescending(q => q.price),
            "event_date" => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.event_date) : query.OrderByDescending(q => q.event_date),
            "quota" => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.quota) : query.OrderByDescending(q => q.quota),
            _ => request.OrderState.ToLower() == "asc" ? query.OrderBy(q => q.ticket_code) : query.OrderByDescending(q => q.ticket_code)
        };

        orderedQuery = orderedQuery.ThenByDescending(q => q.event_date).ThenBy(q => q.price);

        const int pageSize = 10;
        int totalTickets = await query.CountAsync(cancellationToken);

        var data = await query
            .Skip((request.PageNumber - 1) * pageSize)
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
            .ToListAsync(cancellationToken);

        return new PagedResult<TicketModel>
        {
            Items = data,
            TotalCount = totalTickets,
            PageNumber = request.PageNumber,
            PageSize = pageSize
        };
    }
}