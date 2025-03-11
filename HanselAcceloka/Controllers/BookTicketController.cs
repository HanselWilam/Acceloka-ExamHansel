using HanselAcceloka.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/v1/book-ticket")]
[ApiController]
public class BookTicketController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookTicketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request)
    {
        try
        {
            var response = await _mediator.Send(new BookTicketCommand(request));
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(
                type: "https://tools.ietf.org/html/rfc7807",
                title: "Bad Request",
                statusCode: 400,
                detail: ex.Message
            );
        }
    }
}