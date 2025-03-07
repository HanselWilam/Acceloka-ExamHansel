using HanselAcceloka.Services;
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
        private readonly TicketService _service;
        private readonly ILogger<TicketController> _logger;

        public TicketController(TicketService service, ILogger<TicketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get(
            [FromQuery] string? ticketCode,
            [FromQuery] string? ticketName,
            [FromQuery] string? categoryName,
            [FromQuery] int? maxPrice,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? orderBy = "ticket_code",
            [FromQuery] string? orderState = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _service.Get(
                    ticketCode, ticketName, categoryName, maxPrice, startDate, endDate, orderBy, orderState, pageNumber, pageSize);

                return Ok(new
                {
                    tickets = result.Items,
                    totalTickets = result.TotalCount
                });
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