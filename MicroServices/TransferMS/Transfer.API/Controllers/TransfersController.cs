using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferMS.Application.Commands.CreateTransfer;

namespace Transfer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransfersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransfersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TransferCommand command)
        {
            if (command == null)
                return BadRequest(new { message = "Requisição inválida.", type = "INVALID_REQUEST" });

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    message = result.Errors,
                    type = result.GetType()
                });
            }

            return NoContent();
        }
    }
}
