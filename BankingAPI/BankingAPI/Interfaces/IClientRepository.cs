using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Interfaces
{
  public interface IClientRepository
  {
    ICollection<Client> GetAllClients();
    Client GetClient(Int64 id);
    Client GetClientByCivilId(String civilId);
    bool ClientExists(Int64 id);
    bool ClientExistsByCivilId(String civilId);
    bool CreateClient(Client client);
    bool UpdateClient(Int64 clientId, Client client);
    bool PatchClient(Int64 clientId, JsonPatchDocument<Client> client);
    bool DeleteClient(Int64 clientId);
  }
}
