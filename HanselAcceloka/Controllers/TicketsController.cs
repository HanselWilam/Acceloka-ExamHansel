using MediatR;
using Microsoft.AspNetCore.Mvc;
using HanselAcceloka.Application.Tickets.Queries;
using HanselAcceloka.Application.Tickets.Validators;

[ApiController]
[Route("api/v1/get-available-ticket")]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableTickets([FromQuery] GetAvailableTicketsQuery query)
    {
        var validator = new GetAvailableTicketsValidator();
        var validationResult = await validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return BadRequest(new ValidationProblemDetails
            {
                Title = "Invalid request parameters",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Errors = validationResult.Errors.ToDictionary(e => e.PropertyName, e => new[] { e.ErrorMessage })
            });
        }

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}