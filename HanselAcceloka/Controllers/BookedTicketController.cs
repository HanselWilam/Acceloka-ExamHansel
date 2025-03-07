using HanselAcceloka.Models;
using HanselAcceloka.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HanselAcceloka.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookedTicketController : ControllerBase
    {
        private readonly BookedTicketService _service;

        public BookedTicketController(BookedTicketService service)
        {
            _service = service;
        }

        [HttpGet("get-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> GetBookedTicket(int bookedTicketId)
        {
            try
            {
                var result = await _service.GetBookedTicket(bookedTicketId);

                if (result == null)
                {
                    Log.Warning("BookedTicketId {BookedTicketId} tidak ditemukan.", bookedTicketId);
                    return NotFound(new
                    {
                        type = "https://tools.ietf.org/html/rfc7807",
                        title = "Not Found",
                        status = 404,
                        detail = $"BookedTicketId {bookedTicketId} tidak ditemukan."
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saat mengambil data booked ticket {BookedTicketId}", bookedTicketId);
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = "Internal Server Error",
                    status = 500,
                    detail = "Terjadi kesalahan saat mengambil data booked ticket. Silakan coba lagi nanti."
                });
            }
        }

        [HttpDelete("revoke-ticket/{bookedTicketId}/{kodeTicket}/{qty}")]
        public async Task<IActionResult> RevokeTicket(int bookedTicketId, string kodeTicket, int qty)
        {
            try
            {
                var result = await _service.RevokeTicketAsync(bookedTicketId, kodeTicket, qty);

                if (result == null)
                {
                    Log.Warning("Booked Ticket ID {BookedTicketId} atau Ticket Code {KodeTicket} tidak ditemukan.", bookedTicketId, kodeTicket);
                    return NotFound(new
                    {
                        type = "https://tools.ietf.org/html/rfc7807",
                        title = "Not Found",
                        status = 404,
                        detail = "Booked Ticket ID atau Ticket Code tidak ditemukan."
                    });
                }

                if (result is string errorMessage)
                {
                    Log.Warning("Gagal revoke ticket: {ErrorMessage}", errorMessage);
                    return BadRequest(new
                    {
                        type = "https://tools.ietf.org/html/rfc7807",
                        title = "Bad Request",
                        status = 400,
                        detail = errorMessage
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saat melakukan revoke ticket {BookedTicketId}, {KodeTicket}", bookedTicketId, kodeTicket);
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = "Internal Server Error",
                    status = 500,
                    detail = "Terjadi kesalahan saat melakukan revoke ticket. Silakan coba lagi nanti."
                });
            }
        }

        [HttpPut("edit-booked-ticket/{bookedTicketId}")]
        public async Task<IActionResult> EditBookedTicket(int bookedTicketId, [FromBody] List<EditBookedTicketModel> tickets)
        {
            try
            {
                var result = await _service.EditBookedTicketAsync(bookedTicketId, tickets);

                if (result == null)
                {
                    Log.Warning("Booked Ticket ID {BookedTicketId} tidak ditemukan.", bookedTicketId);
                    return NotFound(new
                    {
                        type = "https://tools.ietf.org/html/rfc7807",
                        title = "Not Found",
                        status = 404,
                        detail = "Booked Ticket ID tidak ditemukan."
                    });
                }

                if (result is string errorMessage)
                {
                    Log.Warning("Gagal edit booked ticket: {ErrorMessage}", errorMessage);
                    return BadRequest(new
                    {
                        type = "https://tools.ietf.org/html/rfc7807",
                        title = "Bad Request",
                        status = 400,
                        detail = errorMessage
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error saat mengedit booked ticket {BookedTicketId}", bookedTicketId);
                return StatusCode(500, new
                {
                    type = "https://tools.ietf.org/html/rfc7807",
                    title = "Internal Server Error",
                    status = 500,
                    detail = "Terjadi kesalahan saat mengedit booked ticket. Silakan coba lagi nanti."
                });
            }
        }
    }
}