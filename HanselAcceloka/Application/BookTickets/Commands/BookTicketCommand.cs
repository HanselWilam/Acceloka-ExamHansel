using HanselAcceloka.Models.Requests;
using HanselAcceloka.Models.Responses;
using MediatR;

public class BookTicketCommand : IRequest<BookTicketResponse>
{
    public BookTicketRequest Request { get; }

    public BookTicketCommand(BookTicketRequest request)
    {
        Request = request;
    }
}