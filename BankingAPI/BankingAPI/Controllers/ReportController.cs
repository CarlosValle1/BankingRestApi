using AutoMapper;
using BankingAPI.Dto;
using BankingAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ReportController : Controller
  {
    private readonly IMovementRepository _movementRepository;
    private readonly IMapper _mapper;
    public ReportController(IMovementRepository movementRepository, IMapper mapper)
    {
      _movementRepository = movementRepository;
      _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MovementDto>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetMovements([FromQuery] Int64 clientId, [FromQuery] DateTime initDate, [FromQuery] DateTime endDate)
    {
      // Check if the data sent by the client is valid or if the initDate is greater or equal to the endDate
      if (!ModelState.IsValid || initDate >= endDate)
      {
        // If the data is not valid, return a "400 Bad Request" response with the model state as the body
        return BadRequest(ModelState);
      }
      try
      {
        // Get the movements from the repository and map them to a List<MovementDto>
        var movements = _mapper.Map<List<MovementDto>>(_movementRepository.GetMovements(clientId, initDate, endDate));

        // Check if there is no movements
        if (movements.Count() == 0)
        {
          // If there is no movements, return a "404 Not Found" response
          return NotFound();
        }
        else
        {
          // If there are movements, return a "200 OK" response with the movements as the body
          return Ok(movements);
        }
      }
      catch (Exception e)
      {
        // If there is an exception, return a "500 Internal Server Error" response with the exception message as the body
        return StatusCode(500);
      }
    }
  }
}
