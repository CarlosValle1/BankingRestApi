using static BankingAPI.Models.Account;

namespace BankingAPI.Dto
{
  public class AccountUpdateDto
  {
    public AccountType Type { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Balance { get; set; }
    public Boolean Status { get; set; }
    public Int64 ClientId { get; set; }
  }
}
