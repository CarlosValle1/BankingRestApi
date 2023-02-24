using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Interfaces
{
  public interface IAccountRepository
  {
    Account GetAccount(Int64 id);
    bool AccountExists(Int64 id);
    bool CreateAccount(Account account);
    bool UpdateAccount(Int64 id, Account account);
    bool PatchAccount(Int64 id, JsonPatchDocument<Account> account);
    bool DeleteAccount(Int64 id);
  }
}
