namespace BankingAPI.Models
{
  public class Movement
  {
    public Int64 Id { get; set; }
    public DateTime Date { get; set; }
    public Decimal InitialBalance { get; set; }
    public Decimal Value { get; set; }
    public Decimal Balance { get; set; }
    public Int64 AccountId { get; set; }
    public Account Account { get; set; }

    public void PrepareToBeCreated()
    {
      this.Date = DateTime.Now;
    }

    public void UpdateFrom(Movement other)
    {
      this.Date = other.Date;
      this.InitialBalance = other.InitialBalance;
      this.Value = other.Value;
      this.Balance = other.Balance;
      this.AccountId = other.AccountId;
    }
  }
}
