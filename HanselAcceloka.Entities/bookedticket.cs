using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class bookedticket
{
    public int booked_ticket_id { get; set; }

    public int user_id { get; set; }

    public DateTime? booked_at { get; set; }

    public virtual ICollection<bookedticketdetail> bookedticketdetails { get; set; } = new List<bookedticketdetail>();

    public virtual user user { get; set; } = null!;
}
