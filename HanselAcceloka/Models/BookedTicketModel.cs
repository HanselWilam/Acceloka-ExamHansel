using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanselAcceloka.Models
{
    public class BookedTicketModel
    {
        public int BookedTicketId { get; set; }
        public DateTime BookingDate { get; set; }
        public int UserId { get; set; }
    }
}
