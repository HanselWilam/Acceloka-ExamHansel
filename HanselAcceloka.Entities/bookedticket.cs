using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class bookedticket
{
    public int booked_ticket_id { get; set; }

    public DateTime? booked_at { get; set; }

    public virtual ICollection<bookedticketdetail> bookedticketdetails { get; set; } = new List<bookedticketdetail>();
}
