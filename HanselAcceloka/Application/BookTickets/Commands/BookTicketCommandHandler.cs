using HanselAcceloka.Entities;
using HanselAcceloka.Models.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class BookTicketCommandHandler : IRequestHandler<BookTicketCommand, BookTicketResponse>
{
    private readonly AccelokaContext _context;

    public BookTicketCommandHandler(AccelokaContext context)
    {
        _context = context;
    }

    public async Task<BookTicketResponse> Handle(BookTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketCodes = request.Request.Tickets.Select(t => t.TicketCode).ToList();
        var tickets = await _context.Tickets
            .Where(t => ticketCodes.Contains(t.ticket_code))
            .Include(t => t.category)
            .ToListAsync(cancellationToken);

        if (tickets.Count != ticketCodes.Count)
        {
            throw new Exception("Satu atau lebih tiket tidak ditemukan");
        }

        var bookedTicket = new bookedticket
        {
            booked_at = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.BookedTickets.Add(bookedTicket);
        await _context.SaveChangesAsync(cancellationToken);

        var bookedTicketDetails = new List<bookedticketdetail>();

        foreach (var item in request.Request.Tickets)
        {
            var ticket = tickets.First(t => t.ticket_code == item.TicketCode);
            ticket.quota -= item.Quantity;

            bookedTicketDetails.Add(new bookedticketdetail
            {
                booked_ticket_id = bookedTicket.booked_ticket_id,
                ticket_code = item.TicketCode,
                quantity = item.Quantity
            });
        }

        _context.BookedTicketDetails.AddRange(bookedTicketDetails);
        await _context.SaveChangesAsync(cancellationToken);

        return new BookTicketResponse
        {
            PriceSummary = bookedTicketDetails.Sum(bt =>
                tickets.First(t => t.ticket_code == bt.ticket_code).price * bt.quantity),
            TicketsPerCategories = tickets
                .GroupBy(t => t.category.category_name)
                .Select(g => new CategorySummary
                {
                    CategoryName = g.Key,
                    SummaryPrice = g.Sum(t => t.price * request.Request.Tickets.First(bt => bt.TicketCode == t.ticket_code).Quantity),
                    Tickets = g.Select(t => new TicketDetail
                    {
                        TicketCode = t.ticket_code,
                        TicketName = t.ticket_name,
                        Price = t.price * request.Request.Tickets.First(bt => bt.TicketCode == t.ticket_code).Quantity
                    }).ToList()
                }).ToList()
        };
    }
}