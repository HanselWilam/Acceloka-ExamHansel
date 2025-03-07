using HanselAcceloka.Models.Requests;
using HanselAcceloka.Services;
using Microsoft.AspNetCore.Mvc;

namespace HanselAcceloka.Controllers
{
    [Route("api/v1/book-ticket")]
    [ApiController]
    public class BookTicketController : ControllerBase
    {
        private readonly BookTicketService _bookTicketService;
        public BookTicketController(BookTicketService bookTicketService)
        {
            _bookTicketService = bookTicketService ?? throw new ArgumentNullException(nameof(bookTicketService));
        }

        [HttpPost]
        public async Task<IActionResult> BookTicket([FromBody] BookTicketRequest request)
        {
            try
            {
                var response = await _bookTicketService.BookTicketAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = "Bad Request",
                    status = 400,
                    detail = ex.Message
                });
            }
        }
    }
}
