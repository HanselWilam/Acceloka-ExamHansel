public class TicketDto
{
    public string TicketCode { get; set; }
    public string TicketName { get; set; }
    public string CategoryName { get; set; }
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
    public int Quota { get; set; }
}