using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Interfaces
{
  public enum MovementState
  {
    Ok,
    InsuficientFunds,
    ExceededDailyDebitsLimit,
    ServerError
  }
  public interface IMovementRepository
  {
    Movement GetMovement(Int64 id);
    ICollection<Movement> GetMovements(Int64 clientId, DateTime initDate, DateTime endDate);
    MovementState CreateMovement(Movement movement, in Decimal DAILY_DEBITS_LIMIT);
    bool UpdateMovement(Int64 movementId, Movement movement);
    bool MovementExists(Int64 movementId);
    bool PatchMovement(Int64 movementId, JsonPatchDocument<Movement> movement);
    bool DeleteMovement(Int64 id);
  }
}
