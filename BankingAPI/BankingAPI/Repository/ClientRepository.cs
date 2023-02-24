using BankingAPI.Data;
using BankingAPI.Interfaces;
using BankingAPI.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingAPI.Repository
{
  public class ClientRepository : IClientRepository
  {
    private readonly DataContext _context;
    public ClientRepository(DataContext context)
    {
      _context = context;
    }

    public ICollection<Client> GetAllClients()
    {
      //Retrieves all clients from the database, sorted by Id in ascending order
      return _context.Clients.OrderBy(c => c.Id).ToList();
    }

    public Client GetClient(Int64 id)
    {
      //Retrieves the client with the provided Id from the database
      return _context.Clients.Where(c => c.Id == id).FirstOrDefault();
    }

    public Client GetClientByCivilId(String civilId)
    {
      //Retrieves the client with the provided CivilId from the database
      return _context.Clients.Where(c => c.CivilId == civilId).FirstOrDefault();
    }

    public bool ClientExists(Int64 id)
    {
      //Checks if a client with the provided Id exists in the database
      return _context.Clients.Any(p => p.Id == id);
    }

    public bool ClientExistsByCivilId(String civilId)
    {
      //Checks if a client with the provided CivilId exists in the database
      return _context.Clients.Any(p => p.CivilId == civilId);
    }

    public bool CreateClient(Client client)
    {
      _context.Add(client);
      return Save();
    }

    public bool UpdateClient(Int64 clientId, Client client)
    {
      var oldClient = _context.Clients.Where(c => c.Id == clientId).FirstOrDefault();
      if (oldClient == null)
        return false;

      oldClient.UpdateFrom(client);

      _context.Update(oldClient);
      return Save();
    }

    public bool PatchClient(Int64 clientId, JsonPatchDocument<Client> client)
    {
      var oldClient = _context.Clients.Where(c => c.Id == clientId).FirstOrDefault();
      if (oldClient == null)
        return false;

      client.ApplyTo(oldClient);

      return Save();
    }

    public bool DeleteClient(Int64 clientId)
    {
      var client = _context.Clients.Where(c => c.Id == clientId).FirstOrDefault();
      if (client == null)
        return false;

      _context.Clients.Remove(client);
      return Save();
    }

    private bool Save()
    {
      return _context.SaveChanges() > 0;
    }
  }
}
