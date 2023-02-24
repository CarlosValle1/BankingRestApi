using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace BankingAPI.Models
{
  public class Account
  {
    public enum AccountType
    {
      Checking = 1,
      Savings = 2
    }
    public Int64 Id { get; set; }
    public AccountType Type { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Balance { get; set; }
    public Boolean Status { get; set; }

    public Int64 ClientId { get; set; }
    public Client Client { get; set; }
    public virtual ICollection<Movement> Movements { get; set; }

    public void PrepareToBeCreated()
    {
      this.Balance = InitialBalance;
    }

    public void UpdateFrom(Account other)
    {
      this.Type = other.Type;
      this.InitialBalance = other.InitialBalance;
      this.Balance = other.Balance;
      this.Status = other.Status;
      this.ClientId = other.ClientId;
    }
  }
}
