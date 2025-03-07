namespace HanselAcceloka.Models.Responses
{
    public class BookTicketResponse
    {
        public decimal PriceSummary { get; set; }
        public List<CategorySummary> TicketsPerCategories { get; set; } = new();
    }

    public class CategorySummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal SummaryPrice { get; set; }
        public List<TicketDetail> Tickets { get; set; } = new();
    }

    public class TicketDetail
    {
        public string TicketCode { get; set; } = string.Empty;
        public string TicketName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}