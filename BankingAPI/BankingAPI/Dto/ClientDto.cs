using BankingAPI.Models;

namespace BankingAPI.Dto
{
  public class ClientDto : Person
  {
    public Int64 Id { get; set; }
    public Boolean Status { get; set; }
  }
}
