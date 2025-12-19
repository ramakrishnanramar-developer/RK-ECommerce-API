using Application.Command;
using Application.Queries;
using Domain.Enitities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ISender sender) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> AddProductsAsync([FromBody] ProductsEntity entity)
        {
            var result = await sender.Send(new AddProductCommand(entity));
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductsAsync([FromRoute] int id, [FromBody] ProductsEntity entity)
        {
            var result = await sender.Send(new UpdateProductCommand(id, entity));
            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var result = await sender.Send(new GetAllProductQuery());
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllProductsAsync([FromRoute] int id)
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductsAsync([FromRoute] int id)
        {
            var result = await sender.Send(new DeleteProductByIdCommand(id));
            return Ok(result);
        }
    }
}
