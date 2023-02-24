using BankingAPI.Data;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Repository
{
  public class AccountRepository : IAccountRepository
  {
    private readonly DataContext _context;

    public AccountRepository(DataContext context)
    {
      _context = context;
    }

    public Account GetAccount(Int64 id)
    {
      var query = _context.Accounts.Where(a => a.Id == id);
      var account = query.FirstOrDefault(); // Get the account with the specified id
      account.Client = query.Select(a => a.Client).FirstOrDefault(); // Assign the account's client to the account object
      return account; // return the account object
    }

    public bool AccountExists(Int64 id)
    {
      return _context.Accounts.Any(a => a.Id == id); // Check if an account with the specified id exists in the context
    }

    public bool CreateAccount(Account account)
    {
      _context.Add(account);
      return Save();
    }

    public bool UpdateAccount(Int64 id, Account account)
    {
      var oldAccount = _context.Accounts.Where(a => a.Id == id).FirstOrDefault();
      if (oldAccount == null)
        return false;

      oldAccount.UpdateFrom(account);
      _context.Update(oldAccount);
      return Save();
    }

    public bool PatchAccount(Int64 id, JsonPatchDocument<Account> account)
    {
      var oldAccount = _context.Accounts.Where(a => a.Id == id).FirstOrDefault();
      if (oldAccount == null)
        return false;

      account.ApplyTo(oldAccount);
      return Save();
    }

    public bool DeleteAccount(Int64 id)
    {
      var account = _context.Accounts.Where(a => a.Id == id).FirstOrDefault();
      if (account == null)
        return false;

      _context.Accounts.Remove(account);
      return Save();
    }

    private bool Save()
    {
      return _context.SaveChanges() > 0;
    }
  }
}
