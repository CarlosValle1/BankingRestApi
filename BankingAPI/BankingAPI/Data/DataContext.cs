using BankingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BankingAPI.Data
{
  public class DataContext : DbContext
  {
    public DbSet<Client> Clients { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Movement> Movements { get; set; }
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    
    }
    protected override void ConfigureConventions(
      ModelConfigurationBuilder configurationBuilder)
    {
      configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
  }
}
