using BuildingBlocks.Exeption;
using CheckingAccountMS.Application.Commands.CreateTransfer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CheckingAccount.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : Controller
    {
        private readonly IMediator _mediator;

        public TransferController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTransferCommand command)
        {
            if (command.IdempotencyKey == Guid.Empty)
                return BadRequest(new { type = "INVALID_IDEMPOTENCY_KEY", message = "Missing Idempotency-Key" });

            try
            {
                var accountId = await _mediator.Send(command);
                return Ok(new { success = true, accountId, errorMessage = string.Empty });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                var error = new ApiError(ex.Error);
                return StatusCode(StatusCodes.Status400BadRequest, error);
            }
        }
    }
}
