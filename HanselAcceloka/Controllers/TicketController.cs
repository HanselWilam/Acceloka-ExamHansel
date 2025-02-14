using HanselAcceloka.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HanselAcceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;

        public TicketController(TicketService service)
        {
            _service = service;
        }

        [HttpGet("get-available-ticket")]
        public async Task<IActionResult> Get()
        {
            var datas = await _service.Get();
            return Ok(datas);
        }
    }
}
