using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class user
{
    public int user_id { get; set; }

    public string username { get; set; } = null!;

    public string email { get; set; } = null!;

    public string password_hash { get; set; } = null!;

    public DateTime? created_at { get; set; }

    public virtual ICollection<bookedticket> bookedtickets { get; set; } = new List<bookedticket>();
}
