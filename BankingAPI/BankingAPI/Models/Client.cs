using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace BankingAPI.Models
{
  public class Client : Person
  {
    public Int64 Id { get; set; }
    [MaxLength(300)]
    public String Password { get; set; }
    public Boolean Status { get; set; }
    public virtual ICollection<Account> Accounts { get; set; }

    public void PrepareToBeCreatedOrUpdated()
    {
      this.CivilId = this.CivilId.Trim().ToLower();
    }

    public void UpdateFrom(Client other)
    {
      this.CivilId = other.CivilId;
      this.Name = other.Name;
      this.Gender = other.Gender;
      this.Age = other.Age;
      this.Address = other.Address;
      this.PhoneNumber = other.PhoneNumber;
      this.Password = other.Password;
      this.Status = other.Status;
    }
  }
}
