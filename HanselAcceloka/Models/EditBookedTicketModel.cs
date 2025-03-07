using System.ComponentModel.DataAnnotations;

namespace HanselAcceloka.Models
{
    public class EditBookedTicketModel
    {
        [Required]
        public string TicketCode { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity harus minimal 1")]
        public int Quantity { get; set; }
    }
}