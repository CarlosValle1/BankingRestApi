using System.ComponentModel.DataAnnotations;

namespace BankingAPI.Models
{
  public class Person
  {
    [MaxLength(75)]
    public String CivilId { get; set; }
    [MaxLength(200)]
    public String Name { get; set; }
    [MaxLength(75)]
    public String? Gender { get; set; }
    public int? Age { get; set; }
    [MaxLength(500)]
    public String? Address { get; set; }
    [MaxLength(50)]
    public String PhoneNumber { get; set; }
  }
}
