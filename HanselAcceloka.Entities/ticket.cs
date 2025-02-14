using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class ticket
{
    public string ticket_code { get; set; } = null!;

    public string ticket_name { get; set; } = null!;

    public int category_id { get; set; }

    public DateTime event_date { get; set; }

    public int price { get; set; }

    public int quota { get; set; }

    public virtual ICollection<bookedticketdetail> bookedticketdetails { get; set; } = new List<bookedticketdetail>();

    public virtual category category { get; set; } = null!;
}
