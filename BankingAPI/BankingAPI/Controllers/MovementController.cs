using AutoMapper;
using BankingAPI.Dto;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class MovementController : Controller
  {
    private readonly IMovementRepository _movementRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private const Decimal DAILY_DEBITS_LIMIT = 1000;
    public MovementController(IMovementRepository movementRepository, IAccountRepository accountRepository, IMapper mapper)
    {
      _movementRepository = movementRepository;
      _accountRepository = accountRepository;
      _mapper = mapper;
    }

    [HttpGet("Day")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<MovementDto>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetMovements([FromQuery] Int64 clientId, [FromQuery] DateTime date)
    {
      // Check if the data sent by the client is valid
      if (!ModelState.IsValid)
      {
        // If the data is not valid, return a "400 Bad Request" response with the model state as the body
        return BadRequest(ModelState);
      }
      try
      {
        // Get the current date and the next day date
        var currentDay = date.Date;
        var nextDay = currentDay.AddDays(1);

        // Get the movements from the repository and map them to a List<MovementDto>
        var movements = _mapper.Map<List<MovementDto>>(_movementRepository.GetMovements(clientId, currentDay, nextDay));

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
        // If there is an exception, return a "500 Internal Server Error"
        return StatusCode(500);
      }
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(MovementDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetMovements([FromQuery] Int64 movementId)
    {
      // Check if the model state is valid
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        // Get movement by id
        var movement = _movementRepository.GetMovement(movementId);
        // If movement not found, return 404
        if (movement == null)
          return NotFound();

        // Map movement to DTO and return 200 with DTO
        var movementDto = _mapper.Map<MovementDto>(movement);
        return Ok(movementDto);
      }
      catch (Exception e)
      {
        // Return 500 if an exception occurs
        return StatusCode(500);
      }
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public IActionResult CreateMovement([FromBody] MovementCreationDto newMovement)
    {
      if (!ModelState.IsValid || newMovement == null)
      {
        return BadRequest(ModelState);
      }
      try
      {
        bool accountExists = _accountRepository.AccountExists(newMovement.AccountId);
        if (!accountExists || newMovement.Value == 0)
        {
          ModelState.AddModelError("", "Invalid transaction data");
          return StatusCode(422, ModelState);
        }
        Movement movement = _mapper.Map<Movement>(newMovement);
        movement.PrepareToBeCreated();

        switch (_movementRepository.CreateMovement(movement, DAILY_DEBITS_LIMIT))
        {
          case MovementState.InsuficientFunds:
            {
              ModelState.AddModelError("", "Insuficient funds");
              return StatusCode(422, ModelState);
            }
          case MovementState.ExceededDailyDebitsLimit:
            {
              ModelState.AddModelError("", "Transaction would exceed the daily debits limit of " + DAILY_DEBITS_LIMIT.ToString("C"));
              return StatusCode(422, ModelState);
            }
          case MovementState.ServerError:
            {
              ModelState.AddModelError("", "Something went grong while saving to database");
              return StatusCode(500, ModelState);
            }
        }
        return Ok("Successfully created");
      }
      catch (Exception e)
      {
        ModelState.AddModelError("", "Something went grong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult UpdateMovement([FromQuery] Int64 movementId, [FromBody] MovementUpdateDto updatedMovement)
    {
      if (!ModelState.IsValid || updatedMovement == null)
        return BadRequest(ModelState);

      try
      {
        if (!_movementRepository.MovementExists(movementId))
          return NotFound();

        var movement = _mapper.Map<Movement>(updatedMovement);
        if (!_movementRepository.UpdateMovement(movementId, movement))
        {
          ModelState.AddModelError("", "Something went grong while saving to database");
          return StatusCode(500, ModelState);
        }

        return NoContent();
      }
      catch (Exception e)
      {
        ModelState.AddModelError("", "Something went grong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpPatch]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult PatchMovement([FromQuery] Int64 movementId, [FromBody] JsonPatchDocument<Movement> movement)
    {
      if (!ModelState.IsValid || movement == null)
        return BadRequest(ModelState);

      try
      {
        if (!_movementRepository.MovementExists(movementId))
          return NotFound();

        if (!_movementRepository.PatchMovement(movementId, movement))
        {
          ModelState.AddModelError("", "Something went grong while saving to database");
          return StatusCode(500, ModelState);
        }

        return NoContent();
      } catch (Exception e)
      {
        ModelState.AddModelError("", "Something went grong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult DeleteMovement([FromQuery] Int64 movementId)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        if (!_movementRepository.MovementExists(movementId))
          return NotFound();

        if (!_movementRepository.DeleteMovement(movementId))
        {
          ModelState.AddModelError("", "Something went grong while deleting");
          return StatusCode(500, ModelState);
        }

        return NoContent();
      } catch (Exception e)
      {
        ModelState.AddModelError("", "Something went grong while deleting");
        return StatusCode(500, ModelState);
      }
    }
  }
}
