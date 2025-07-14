using Application.Commands.CreateAccount;
using Application.Commands.DeactivateAccount;
using Application.Exeption;
using Application.Queries.GetAccountBalance;
using Application.Queries.GetAccountByAccountNumber;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("{AccountNumber}")]
        public async Task<IActionResult> GetByAccountNumber(long accountNumber)
        {
            var result = await _mediator.Send(new GetAccountByAccountNumberQuery(accountNumber));
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetByAccountNumber), new { accountNumber = result.AccountNumber }, result);
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

        [Authorize]
        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return NoContent(); // 204
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                var error = new ApiError(ex.Error);
                return StatusCode(StatusCodes.Status403Forbidden, error);
            }
        }

        [Authorize]
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var result = await _mediator.Send(new GetAccountBalanceQuery());
                return Ok(result);
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
