using HanselAcceloka.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanselAcceloka.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? CategoryName { get; set; }

        public ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();
    }

}