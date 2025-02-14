using System.ComponentModel.DataAnnotations;

namespace HanselAcceloka.Models.Requests
{
    public class BookTicketRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public List<BookTicketItem> Tickets { get; set; } = new();
    }

    public class BookTicketItem
    {
        public string TicketCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
