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
  public class AccountController : Controller
  {
    private readonly IAccountRepository _accountRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IMapper _mapper;

    public AccountController(IAccountRepository accountRepository, IClientRepository clientRepository, IMapper mapper)
    {
      _accountRepository = accountRepository;
      _clientRepository = clientRepository;
      _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(AccountDto))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult GetAccount([FromQuery] int accountId)
    {
      // Validate the model state
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      // Check if the account exists in the repository
      if (!_accountRepository.AccountExists(accountId))
        return NotFound();
      try
      {
        // Get the account from the repository and map it to the AccountDto model
        var account = _mapper.Map<AccountDto>(_accountRepository.GetAccount(accountId));
        // Return the account as OK with the AccountDto model as the response
        return Ok(account);
      }
      catch (Exception e)
      {
        // Return a status code of 500 with the exception message if there is an error
        return StatusCode(500);
      }
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public IActionResult CreateAccount([FromBody] AccountCreationDto newAccount)
    {
      // Check if the model state is valid and if the newAccount object is not null
      if (!ModelState.IsValid || newAccount == null)
      {
        // If not valid, return a BadRequest with the ModelState
        return BadRequest(ModelState);
      }
      try
      {
        // Use the mapper to map the newAccount object to an Account object
        Account account = _mapper.Map<Account>(newAccount);
        // Prepare the account to be created
        account.PrepareToBeCreated();
        // Call the CreateAccount method on the repository to save the account to the database
        _accountRepository.CreateAccount(account);
        // Return a Ok status with message "Successfully created"
        return Ok("Successfully created");
      }
      catch (Exception e)
      {
        // If an exception is thrown, add an error message to the ModelState and return a status code of 500
        ModelState.AddModelError("", "Something went grong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpPut]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult UpdateAccount([FromQuery] Int64 accountNumber, [FromBody] AccountUpdateDto updatedAccount)
    {
      // Check if the model state is valid and if the updatedAccount is not null
      if (!ModelState.IsValid || updatedAccount == null)
        return BadRequest(ModelState);

      try
      {
        // Check if the account and client exists
        if (!_accountRepository.AccountExists(accountNumber) ||
          !_clientRepository.ClientExists(updatedAccount.ClientId))
          return NotFound();

        // maps the AccountUpdateDto to Account and call the updateAccount method
        var account = _mapper.Map<Account>(updatedAccount);
        if (!_accountRepository.UpdateAccount(accountNumber, account))
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

    [HttpPatch]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult PatchAccount([FromQuery] Int64 accountId,
      [FromBody] JsonPatchDocument<Account> account)
    {
      // Check if the request model is valid
      if (!ModelState.IsValid || account == null)
        return BadRequest(ModelState);

      try
      {
        // Check if the account exists
        if (!_accountRepository.AccountExists(accountId))
          return NotFound();

        // Attempt to update the account in the repository
        if (!_accountRepository.PatchAccount(accountId, account))
        {
          // If something went wrong, add an error message to the model state and return a 500 status code
          ModelState.AddModelError("", "Something went wrong while saving to database");
          return StatusCode(500, ModelState);
        }

        // If everything is ok, return a 204 status code
        return NoContent();
      }
      catch (Exception e)
      {
        // If an exception occurs, add an error message to the model state and return a 500 status code
        ModelState.AddModelError("", "Something went wrong while saving to database");
        return StatusCode(500, ModelState);
      }
    }

    [HttpDelete]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public IActionResult DeleteAccount([FromQuery] Int64 accountId)
    {
      //Check if the request is valid
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        //Check if the account exists
        if (!_accountRepository.AccountExists(accountId))
          return NotFound();

        //Try to delete the account
        if (!_accountRepository.DeleteAccount(accountId))
        {
          //If something goes wrong add error and return 500
          ModelState.AddModelError("", "Something went wrong while deleting");
          return StatusCode(500, ModelState);
        }

        //If everything goes well return 204
        return NoContent();
      }
      catch (Exception e)
      {
        //If something goes wrong add error and return 500
        ModelState.AddModelError("", "Something went wrong while deleting");
        return StatusCode(500, ModelState);
      }
    }
  }
}
