using Microsoft.AspNetCore.Mvc.Testing;
using BankingAPI.Models;
namespace BankingAPI.Test
{
  [TestClass]
  public class ApiTests
  {
    // Integration testing
    [TestMethod]
    public async Task ApiClientRoute_ReturnsSuccessCode()
    {
      var webAppFactory = new WebApplicationFactory<Program>();
      var httpClient = webAppFactory.CreateDefaultClient();

      var response = await httpClient.GetAsync("api/Client");
      Assert.AreEqual(response.IsSuccessStatusCode, true);
    }

    // Unit testing
    [TestMethod]
    public void ClientCreatePreparationTrimsCivilId()
    {
      // Arrange
      var client = new Client()
      {
        Id = 1,
        CivilId = "  JS123MN  "
      };
      var expectedCivilId = "js123mn";

      // Act
      client.PrepareToBeCreatedOrUpdated();

      // Assert
      Assert.AreEqual(client.CivilId, expectedCivilId);
    }

    // Unit testing
    [TestMethod]
    public void AccountAUpdateClonesDesiredFieldsOnly()
    {
      // Arrange
      var accountA = new Account()
      {
        Id = 1,
        Type = Account.AccountType.Checking,
        InitialBalance = 1500,
        Balance = 1000,
        Status = true,
        ClientId = 2
      };
      var accountB = new Account()
      {
        Id = -1,
        Type = Account.AccountType.Savings,
        InitialBalance = 1800,
        Balance = 4000,
        Status = true,
        ClientId = 4
      };


      // Act
      accountA.UpdateFrom(accountB);

      // Assert
      Assert.AreNotEqual(accountA.Id, accountB.Id);
    }
  }
}