using BankingAPI.Models;

namespace BankingAPI.Dto
{
  public class AccountDto
  {
    public Int64 Id { get; set; }
    public Account.AccountType Type { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Balance { get; set; }
    public Boolean Status { get; set; }
    public Int64 ClientId { get; set; }
    public String ClientName { get; set; }
  }
}
