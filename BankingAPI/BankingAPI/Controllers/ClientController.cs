using BankingAPI.Models;
using BankingAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BankingAPI.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ClientController : Controller
  {
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;
    public ClientController(IClientRepository clientRepository, IMapper mapper)
    {
      _clientRepository = clientRepository;
      _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<ClientDto>))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetAllClients()
    {
      // Check if the model state is valid
      if (!ModelState.IsValid)
      {
        // If it's not valid, return a Bad Request with the model state
        return BadRequest(ModelState);
      }
      try
      {
        // Retrieve all clients from the database
        var clients = _mapper.Map<List<ClientDto>>(_clientRepository.GetAllClients());

        // If clients are found, return a 200 OK status with the clients 
        return Ok(clients);
      }
      catch (Exception e)
      {
        // If an exception is thrown, return a 500 Internal Server Error with the exception message
        return StatusCode(500);
      }

    }


    [HttpGet("GetOne/")]
    [ProducesResponseType(200, Type = typeof(ClientDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetClient([FromQuery] int clientId)
    {
      // Check if the provided clientId is valid
      if (!ModelState.IsValid)
      {
        // If not, return a BadRequest response
        return BadRequest(ModelState);
      }

      // Check if a client with the provided clientId exists in the repository
      if (!_clientRepository.ClientExists(clientId))
        // If not, return a NotFound response
        return NotFound();

      try
      {
        // Retrieve the client from the repository
        var client = _mapper.Map<ClientDto>(_clientRepository.GetClient(clientId));

        // Return the client as an OK response
        return Ok(client);
      }
      catch (Exception e)
      {
        // In case of an exception, return a StatusCode of 500 and the exception message
        return StatusCode(500);
      }

    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ProducesResponseType(500)]
    public IActionResult CreateClient([FromBody] ClientCreationDto newClient)
    {
      // Check if the request data is valid
      if (!ModelState.IsValid || newClient == null)
      {
        return BadRequest(ModelState);
      }

      try
      {
        // Check if client already exists by civil ID
        bool existsClient = _clientRepository.ClientExistsByCivilId(newClient.CivilId.Trim().ToLower());
        if (!existsClient)
        {
          // Map DTO to entity and prepare for creation
          Client client = _mapper.Map<Client>(newClient);
          client.PrepareToBeCreatedOrUpdated();

          // Create client
          _clientRepository.CreateClient(client);

          return Ok("Successfully created");
        }
        else
        {
          ModelState.AddModelError("", "Client with same civil ID already exists");
          return StatusCode(422, ModelState);
        }
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
    public IActionResult UpdateClient([FromQuery] Int64 clientId, [FromBody] ClientCreationDto updatedClient)
    {
      //Check if model state is valid and if the request body is not empty
      if (!ModelState.IsValid || updatedClient == null)
        return BadRequest(ModelState);

      try
      {
        //Check if client with the given id exists
        if (!_clientRepository.ClientExists(clientId))
          return NotFound();

        //Map DTO to client model and prepare for update
        var client = _mapper.Map<Client>(updatedClient);
        client.PrepareToBeCreatedOrUpdated();

        //Try to update client in the database
        if (!_clientRepository.UpdateClient(clientId, client))
        {
          ModelState.AddModelError("", "Something went grong while saving to database");
          return StatusCode(500, ModelState);
        }

        //Return 204 status code if update was successful
        return NoContent();
      }
      catch (Exception e)
      {
        //Return 500 status code if there was an error while trying to update client
        ModelState.AddModelError("", "Something went grong while saving to database");
        return StatusCode(500, ModelState);
      }

    }

    [HttpPatch]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult PatchClient([FromQuery] Int64 clientId,
      [FromBody] JsonPatchDocument<Client> client)
    {
      // Validate request
      if (!ModelState.IsValid || client == null)
        return BadRequest(ModelState);

      try
      {
        // Check if client exists
        if (!_clientRepository.ClientExists(clientId))
          return NotFound();

        // Attempt to update client
        if (!_clientRepository.PatchClient(clientId, client))
        {
          ModelState.AddModelError("", "Something went wrong while saving to database");
          return StatusCode(500, ModelState);
        }

        return NoContent();

      }
      catch (Exception e)
      {
        ModelState.AddModelError("", "Something went wrong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult DeleteClient([FromQuery] Int64 clientId)
    {
      // return bad request if model state is invalid
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        // return not found if client does not exist
        if (!_clientRepository.ClientExists(clientId))
          return NotFound();

        // delete client and return appropriate status code
        if (!_clientRepository.DeleteClient(clientId))
        {
          ModelState.AddModelError("", "Something went wrong while deleting");
          return StatusCode(500, ModelState);
        }

        // return no content if successful
        return NoContent();
      }
      catch (Exception e)
      {
        // return internal server error if exception is thrown
        ModelState.AddModelError("", "Something went wrong while deleting");
        return StatusCode(500, ModelState);
      }
    }
  }
}
