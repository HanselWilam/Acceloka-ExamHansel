using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using HanselAcceloka.Entities;

namespace HanselAcceloka.Models
{
    public class TicketModel
    {
        [Required]
        [JsonIgnore]
        public DateTime? EventDate { get; set; }

        [NotMapped]
        [JsonPropertyName("eventDate")]
        public string? EventDateFormatted => EventDate?.ToString("yyyy-MM-dd HH:mm");

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quota harus lebih dari 0")]
        public int Quota { get; set; }

        [Required]
        [Key]
        public string TicketCode { get; set; } = string.Empty;

        public string TicketName { get; set; } = string.Empty;

        [JsonIgnore]
        public CategoryModel? Category { get; set; }

        [NotMapped]
        [JsonPropertyName("categoryName")]
        public string? CategoryName => Category?.CategoryName;

        [Required]
        public int Price { get; set; }
    }
}
