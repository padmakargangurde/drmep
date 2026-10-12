using Application.Features.Users.DTOs;
using Application.Features.Users.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MEP_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
            => _userService = userService;

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<UserListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _userService.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _userService.GetByIdAsync(id, ct);

            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { message = result.Error });
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserRequest request, CancellationToken ct)
        {
            var result = await _userService.CreateAsync(request, ct);

            if (!result.Succeeded)
                return BadRequest(new { message = result.Error });

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Data!.UserId },
                result.Data);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateUserRequest request, CancellationToken ct)
        {
            var result = await _userService.UpdateAsync(id, request, ct);

            if (result.Succeeded)
                return Ok(result.Data);

            return result.Error == "User not found."
                ? NotFound(new { message = result.Error })
                : BadRequest(new { message = result.Error });
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _userService.DeleteAsync(id, ct);

            return result.Succeeded
                ? NoContent()
                : NotFound(new { message = result.Error });
        }
    }
}
