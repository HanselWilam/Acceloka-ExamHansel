using HanselAcceloka.Application.Tickets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HanselAcceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TicketController> _logger;

        public TicketController(IMediator mediator, ILogger<TicketController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get([FromQuery] GetAvailableTicketsQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(new { tickets = result.Items, totalTickets = result.TotalCount });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid request parameters.");
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid request",
                    Status = 400,
                    Detail = ex.Message,
                    Type = "https://httpstatuses.com/400"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching tickets.");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = 500,
                    Detail = "An unexpected error occurred while processing your request.",
                    Type = "https://httpstatuses.com/500"
                });
            }
        }
    }
}