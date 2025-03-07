using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using HanselAcceloka.Models.Requests;
using HanselAcceloka.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace HanselAcceloka.Services
{
    public class BookTicketService
    {
        private readonly AccelokaContext _context;

        public BookTicketService(AccelokaContext context)
        {
            _context = context;
        }

        public async Task<BookTicketResponse> BookTicketAsync(BookTicketRequest request)
        {

            var ticketCodes = request.Tickets.Select(t => t.TicketCode).ToList();
            var tickets = await _context.Tickets
                .Where(t => ticketCodes.Contains(t.ticket_code))
                .Include(t => t.category)
                .ToListAsync();

            if (tickets.Count != ticketCodes.Count)
            {
                throw new Exception("Satu atau lebih tiket tidak ditemukan");
            }

            foreach (var item in request.Tickets)
            {
                var ticket = tickets.FirstOrDefault(t => t.ticket_code == item.TicketCode);
                if (ticket == null) continue;

                if (ticket.quota < item.Quantity)
                {
                    throw new Exception($"Stok tiket {ticket.ticket_code} tidak mencukupi");
                }

                if (ticket.event_date <= DateTime.UtcNow)
                {
                    throw new Exception($"Tiket {ticket.ticket_code} sudah kadaluarsa");
                }
            }

            var bookedTicket = new bookedticket
            {
                booked_at = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            _context.BookedTickets.Add(bookedTicket);
            await _context.SaveChangesAsync();

            var bookedTicketDetails = new List<bookedticketdetail>();
            foreach (var item in request.Tickets)
            {
                bookedTicketDetails.Add(new bookedticketdetail
                {
                    booked_ticket_id = bookedTicket.booked_ticket_id,
                    ticket_code = item.TicketCode,
                    quantity = item.Quantity
                });

                var ticket = tickets.First(t => t.ticket_code == item.TicketCode);
                ticket.quota -= item.Quantity;
            }

            _context.BookedTicketDetails.AddRange(bookedTicketDetails);
            await _context.SaveChangesAsync();

            var response = new BookTicketResponse
            {
                PriceSummary = bookedTicketDetails.Sum(bt =>
                    tickets.First(t => t.ticket_code == bt.ticket_code).price * bt.quantity)
            };

            response.TicketsPerCategories = tickets
    .GroupBy(t => t.category.category_name)
    .Select(g => new CategorySummary
    {
        CategoryName = g.Key,
        SummaryPrice = g.Sum(t => t.price * request.Tickets.First(bt => bt.TicketCode == t.ticket_code).Quantity),
        Tickets = g.Select(t => new TicketDetail
        {
            TicketCode = t.ticket_code,
            TicketName = t.ticket_name,
            Price = t.price * request.Tickets.First(bt => bt.TicketCode == t.ticket_code).Quantity
        }).ToList()
    }).ToList();

            return response;
        }
    }
}