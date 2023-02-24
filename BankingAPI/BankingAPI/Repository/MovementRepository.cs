using BankingAPI.Data;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Repository
{
  public class MovementRepository : IMovementRepository
  {
    private readonly DataContext _context;
    public MovementRepository(DataContext context)
    {
      _context = context;
    }

    public Movement GetMovement(Int64 id)
    {
      var query = _context.Movements.Where(m => m.Id == id);
      var movement = query.FirstOrDefault();
      if (movement == null)
        return movement;
      var account = query.Select(m => m.Account).FirstOrDefault();
      movement.Account = account;
      var client = query.Select(m => m.Account).Select(m => m.Client).FirstOrDefault();
      movement.Account.Client = client;
      return movement;
    }
    public ICollection<Movement> GetMovements(Int64 clientId, DateTime initDate, DateTime endDate)
    {
      var movements = new List<Movement>();
      var accounts = _context.Clients.Where(c => c.Id == clientId).Select(c => c.Accounts).FirstOrDefault();
      var client = _context.Clients.Where(c => c.Id == clientId).FirstOrDefault();
      if (accounts == null)
        return movements;

      foreach (var account in accounts)
      {
        account.Client = client;
        var accountMovements = _context.Movements.Where(m => m.AccountId == account.Id && m.Date >= initDate && m.Date < endDate).ToList();
        if (accountMovements == null)
          break;
        movements.AddRange(accountMovements);
      }
      return movements.OrderBy(m => m.AccountId).ThenBy(m => m.Date).ToList();
    }

    public MovementState CreateMovement(Movement movement, in Decimal DAILY_DEBITS_LIMIT)
    {
      var accountBalance = GetAccountBalance(movement.AccountId);
      if (accountBalance + movement.Value < 0)
        return MovementState.InsuficientFunds;

      var isADebit = movement.Value < 0 ? true : false;
      if (isADebit)
      {
        var todaysDebits = GetAccountDailyDebits(movement.AccountId, movement.Date);
        var potentialDailyDebits = todaysDebits + movement.Value * -1;
        if (potentialDailyDebits > DAILY_DEBITS_LIMIT)
          return MovementState.ExceededDailyDebitsLimit;
      }
      
      movement.InitialBalance = accountBalance;
      movement.Balance = movement.InitialBalance + movement.Value;

      _context.Add(movement);
      if (UpdateBalanceIntoAccount(movement.AccountId, movement.Balance) == false)
        return MovementState.ServerError;

      return Save() ? MovementState.Ok : MovementState.ServerError;
    }

    public bool UpdateMovement(Int64 movementId, Movement movement)
    {
      var oldMovement = _context.Movements.Where(m => m.Id == movementId).FirstOrDefault();
      if (oldMovement == null)
        return false;

      oldMovement.UpdateFrom(movement);
      _context.Update(oldMovement);
      return Save();
    }

    public bool PatchMovement(Int64 movementId, JsonPatchDocument<Movement> movement)
    {
      var oldMovement = _context.Movements.Where(m => m.Id == movementId).FirstOrDefault();
      if (oldMovement == null)
        return false;

      movement.ApplyTo(oldMovement);
      return Save();
    }
    public bool DeleteMovement(Int64 id)
    {
      var movement = _context.Movements.Where(m => m.Id == id).FirstOrDefault();
      if (movement == null)
        return false;

      _context.Movements.Remove(movement);
      return Save();
    }
    public bool MovementExists(Int64 movementId)
    {
      var movement = _context.Movements.Where(x => x.Id == movementId).FirstOrDefault();
      return movement == null ? false : true;
    }

    private bool Save()
    {
      return _context.SaveChanges() > 0;
    }

    private Decimal GetAccountBalance(Int64 accountId)
    {
      var account = _context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
      return account != null ? account.Balance : -1;

    }

    private Decimal GetAccountDailyDebits(Int64 accountId, DateTime movementTime)
    {
      var today = movementTime.Date;
      var tomorrow = today.AddDays(1);

      var account = _context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
      var movements = _context.Movements.Where(m => m.AccountId == accountId &&
                                                m.Date >= today && m.Date <= tomorrow &&
                                                m.Value < 0).ToList();
      Decimal result = 0;
      if (movements != null)
      {
        foreach (var movement in movements)
          result += movement.Value;
      }
      
      return result * -1; // Return as positive
    }

    private bool UpdateBalanceIntoAccount(Int64 accountId, Decimal newBalance)
    {
      var account = _context.Accounts.Where(a => a.Id == accountId).FirstOrDefault();
      if (account == null)
        return false;
      account.Balance = newBalance;
      _context.Update(account);
      return true;
    }
  }
}
