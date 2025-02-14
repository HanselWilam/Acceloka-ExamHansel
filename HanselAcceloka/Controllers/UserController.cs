using HanselAcceloka.Models;
using HanselAcceloka.Services;
using Microsoft.AspNetCore.Mvc;

namespace HanselAcceloka.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] UserRequest userRequest)
    {
        try
        {
            var newUser = await _userService.AddUserAsync(userRequest);

            if (newUser == null)
            {
                return Problem(type: "https://tools.ietf.org/html/rfc7807", title: "Internal Server Error", detail: "Gagal menyimpan user ke database.", statusCode: 500);
            }

            return CreatedAtAction(nameof(AddUser), new { id = newUser.UserId }, newUser);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(type: "https://tools.ietf.org/html/rfc7807", title: "Conflict", detail: ex.Message, statusCode: 409);
        }
        catch (Exception ex)
        {
            return Problem(type: "https://tools.ietf.org/html/rfc7807", title: "Internal Server Error", detail: ex.Message, statusCode: 500);
        }
    }


}
