using System;
using System.Collections.Generic;

namespace HanselAcceloka.Entities;

public partial class category
{
    public int category_id { get; set; }

    public string category_name { get; set; } = null!;

    public virtual ICollection<ticket> tickets { get; set; } = new List<ticket>();
}
