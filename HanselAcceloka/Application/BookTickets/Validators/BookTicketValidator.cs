using FluentValidation;
using HanselAcceloka.Models.Requests;
using HanselAcceloka.Entities;
using System.Linq;
using System.Net.Sockets;

public class BookTicketValidator : AbstractValidator<BookTicketRequest>
{
    private readonly List<ticket> _tickets;

    public BookTicketValidator(AccelokaContext context)
    {
        // Ambil semua tiket dari database sekali di constructor
        _tickets = context.Tickets.ToList();

        RuleForEach(x => x.Tickets).ChildRules(ticket =>
        {
            ticket.RuleFor(x => x.TicketCode)
                .NotEmpty().WithMessage("Kode tiket tidak boleh kosong.")
                .Must(TicketExists).WithMessage("Kode tiket tidak terdaftar.");

            ticket.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity harus lebih dari 0.")
                .Must(EnoughQuota).WithMessage("Quantity tiket yang dibooking melebihi sisa quota.");
        });
    }

    private bool TicketExists(string ticketCode)
    {
        return _tickets.Any(t => t.ticket_code == ticketCode);
    }

    private bool EnoughQuota(BookTicketItem ticket, int quantity)
    {
        var dbTicket = _tickets.FirstOrDefault(t => t.ticket_code == ticket.TicketCode);
        if (dbTicket == null) return false;

        return quantity <= dbTicket.quota;
    }
}