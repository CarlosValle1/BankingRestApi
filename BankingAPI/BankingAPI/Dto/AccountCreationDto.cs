using BankingAPI.Models;

namespace BankingAPI.Dto
{
  public class AccountCreationDto
  {
    public Account.AccountType Type { get; set; }
    public Decimal InitialBalance { get; set; }
    public Boolean Status { get; set; }
    public Int64 ClientId { get; set; }
  }
}
