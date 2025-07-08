using Application.DTOs;
using Application.Exeption;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankMore.BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaCorrenteController : ControllerBase
    {
        private readonly ContaCorrenteService _service;

        public ContaCorrenteController(ContaCorrenteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _service.GetByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContaCorrenteDto dto)
        {
            try
            {
                var conta = await _service.CriarAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = conta.IdContaCorrente }, conta);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ServiceException ex)
            {
                var error = new ApiError(ex.Error);
                return StatusCode((int)ex.Error.Status, error);
            }
        }
    }
}
