using Application.Commands.CreateTransaction;
using Application.Exeption;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CheckingAccount.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTransactionCommand command)
        {
            if (command.IdempotencyKey == Guid.Empty)
                return BadRequest(new { type = "INVALID_IDEMPOTENCY_KEY", message = "Missing Idempotency-Key" });

            try
            {
                var transactionId = await _mediator.Send(command);
                return Ok(new { transactionId });
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
