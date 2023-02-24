using BankingAPI.Models;
using Microsoft.Data.SqlClient;

namespace BankingAPI.Dto
{
  public class MovementDto
  {
    public Int64 Id { get; set; }
    public DateTime Date { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Value { get; set; }
    public Decimal Balance { get; set; }
    public Int64 AccountNumber { get; set; }
    public Account.AccountType AccountType { get; set; }
    public Boolean AccountStatus { get; set; }
    public String? ClientName { get; set; }
  }
}
