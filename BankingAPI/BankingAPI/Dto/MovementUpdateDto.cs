namespace BankingAPI.Dto
{
  public class MovementUpdateDto
  {
    public DateTime Date { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Value { get; set; }
    public Decimal Balance { get; set; }
    public Int64 AccountId { get; set; }
  }
}
