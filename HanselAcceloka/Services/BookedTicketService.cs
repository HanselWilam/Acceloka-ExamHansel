using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using Microsoft.EntityFrameworkCore;

namespace HanselAcceloka.Services
{
    public class BookedTicketService
    {
        private readonly AccelokaContext _db;

        public BookedTicketService(AccelokaContext db)
        {
            _db = db;
        }

        public async Task<object?> GetBookedTicket(int bookedTicketId)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(b => b.bookedticketdetails)
                    .ThenInclude(d => d.ticket_codeNavigation)
                        .ThenInclude(t => t.category)
                .FirstOrDefaultAsync(b => b.booked_ticket_id == bookedTicketId);

            if (bookedTicket == null)
            {
                throw new KeyNotFoundException($"Booked ticket dengan ID {bookedTicketId} tidak ditemukan.");
            }


            var groupedTickets = bookedTicket.bookedticketdetails
                .GroupBy(d => d.ticket_codeNavigation.category.category_name)
                .Select(g => new
                {
                    qtyPerCategory = g.Sum(d => d.quantity),
                    categoryName = g.Key,
                    tickets = g.Select(d => new
                    {
                        ticketCode = d.ticket_code,
                        ticketName = d.ticket_codeNavigation.ticket_name,
                        eventDate = d.ticket_codeNavigation.event_date.ToString("dd-MM-yyyy HH:mm"),
                        quantity = d.quantity
                    }).ToList()

                })
                .ToList();

            return groupedTickets;
        }

        public async Task<object?> RevokeTicketAsync(int bookedTicketId, string kodeTicket, int qty)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(b => b.bookedticketdetails)
                .ThenInclude(d => d.ticket_codeNavigation)
                .ThenInclude(t => t.category)
                .FirstOrDefaultAsync(b => b.booked_ticket_id == bookedTicketId);

            if (bookedTicket == null)
                return null;

            var ticketDetail = bookedTicket.bookedticketdetails
                .FirstOrDefault(d => d.ticket_code == kodeTicket);

            if (ticketDetail == null)
                return null;

            if (qty > ticketDetail.quantity)
            {
                return "Requested quantity exceeds booked quantity.";
            }

            if (qty == ticketDetail.quantity)
            {
                _db.BookedTicketDetails.Remove(ticketDetail);
            }
            else
            {
                ticketDetail.quantity -= qty;
            }

            if (!bookedTicket.bookedticketdetails.Any(d => d.quantity > 0))
            {
                _db.BookedTickets.Remove(bookedTicket);
            }

            await _db.SaveChangesAsync();

            return new
            {
                ticketCode = ticketDetail.ticket_code,
                ticketName = ticketDetail.ticket_codeNavigation.ticket_name,
                categoryName = ticketDetail.ticket_codeNavigation.category.category_name,
                remainingQuantity = ticketDetail.quantity
            };

        }

        public async Task<object?> EditBookedTicketAsync(int bookedTicketId, List<EditBookedTicketModel> tickets)
        {
            var bookedTicket = await _db.BookedTickets
                .Include(b => b.bookedticketdetails)
                .ThenInclude(d => d.ticket_codeNavigation)
                .ThenInclude(t => t.category)
                .FirstOrDefaultAsync(b => b.booked_ticket_id == bookedTicketId);

            if (bookedTicket == null)
                return null;

            foreach (var item in tickets)
            {
                var bookedDetail = bookedTicket.bookedticketdetails.FirstOrDefault(d => d.ticket_code == item.TicketCode);

                if (bookedDetail == null)
                    return $"Kode tiket {item.TicketCode} tidak ditemukan dalam booking ini.";

                var availableTicket = await _db.Tickets.FirstOrDefaultAsync(t => t.ticket_code == item.TicketCode);

                if (availableTicket == null)
                    return $"Kode tiket {item.TicketCode} tidak terdaftar.";

                if (item.Quantity < 1)
                    return $"Quantity tiket {item.TicketCode} harus minimal 1.";

                if (item.Quantity > availableTicket.quota)
                    return $"Quantity tiket {item.TicketCode} melebihi sisa quota yang tersedia.";

                bookedDetail.quantity = item.Quantity;
            }

            await _db.SaveChangesAsync();

            var updatedTickets = bookedTicket.bookedticketdetails
                .Select(d => new
                {
                    ticketCode = d.ticket_code,
                    ticketName = d.ticket_codeNavigation.ticket_name,
                    categoryName = d.ticket_codeNavigation.category.category_name,
                    quantity = d.quantity
                })
                .ToList();

            return updatedTickets;
        }


    }
}
