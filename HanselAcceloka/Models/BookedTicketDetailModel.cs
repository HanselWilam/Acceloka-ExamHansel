using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanselAcceloka.Models
{
    public class BookedTicketDetailModel
    {
        public int BookedTicketDetailId { get; set; }
        public int BookedTicketId { get; set; }
        public string? TicketCode { get; set; }
        public int Quantity { get; set; }
    }
}