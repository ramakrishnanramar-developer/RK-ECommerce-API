using Application.Command;
using Application.Queries;
using Domain.Enitities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(ISender sender) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> AddClientsAsync([FromBody] ClientEntity clientEntity)
        {
            var result = await sender.Send(new AddClientCommand(clientEntity));
            return Ok(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClientsAsync([FromRoute] int id,[FromBody] ClientEntity clientEntity)
        {
            var result = await sender.Send(new UpdateClientCommand(id,clientEntity));
            return Ok(result);
        }
        [HttpGet()]
        public async Task<IActionResult> GetAllClientsAsync()
        {
            var result = await sender.Send(new GetAllClientQuery());
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllClientsAsync([FromRoute] int id)
        {
            var result = await sender.Send(new GetClientByIdQuery(id));
            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientsAsync([FromRoute] int id)
        {
            var result = await sender.Send(new DeleteClientByIdCommand(id));
            return Ok(result);
        }
    }
}
