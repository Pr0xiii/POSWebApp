using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IClientService 
    {
        Task<List<Person>> GetAllClientsAsync();
        Task<Person?> GetClientByIdAsync(int? id);
        Task UpdateClientAsync(Person client);
        Task DeleteClientAsync(int id);
        Task<int> GetRandomIDAsync();

        Task<List<Sale>> GetAllSalesAsync(int clientID);
        Task AddSaleAsync(int clientID, Sale sale);
    }
}