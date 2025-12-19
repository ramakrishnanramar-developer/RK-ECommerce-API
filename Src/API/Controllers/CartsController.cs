using Application.Command;
using Application.DTO;
using Application.Queries;
using Domain.Enitities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController(ISender sender) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> AddCartsAsync([FromBody] CartDTO cartEntity)
        {
            var result = await sender.Send(new AddCartCommand(cartEntity));
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartsAsync([FromRoute] int id, [FromBody] CartWithIdDTO cartEntity)
        {
            var result = await sender.Send(new UpdateCartCommand(id, cartEntity));
            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetAllCartsAsync()
        {
            var result = await sender.Send(new GetAllCartQuery());
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCartByIdAsync([FromRoute] int id)
        {
            var result = await sender.Send(new GetCartByIdQuery(id));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartsAsync([FromRoute] int id)
        {
            var result = await sender.Send(new DeleteCartByIdCommand(id));
            return Ok(result);
        }
    }
}
