using Application.Command;
using Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController(ISender sender) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> AddCartsAsync([FromBody] CartWithItemsDto cartEntity)
        {
            var result = await sender.Send(new AddCartItemsCommand(cartEntity));
            return Ok(result);
        }
    }
}
