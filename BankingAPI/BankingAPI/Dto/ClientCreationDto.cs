using BankingAPI.Models;

namespace BankingAPI.Dto
{
  public class ClientCreationDto : Person
  {
    public String Password { get; set; }
    public Boolean Status { get; set; }
  }
}
