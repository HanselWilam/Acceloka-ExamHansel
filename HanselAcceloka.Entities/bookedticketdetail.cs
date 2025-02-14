using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class bookedticketdetail
{
    public int booked_ticket_detail_id { get; set; }

    public int booked_ticket_id { get; set; }

    public string ticket_code { get; set; } = null!;

    public int quantity { get; set; }

    public virtual bookedticket booked_ticket { get; set; } = null!;

    public virtual ticket ticket_codeNavigation { get; set; } = null!;
}
