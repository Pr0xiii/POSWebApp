using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IClientService 
    {
        Task<List<Person>> GetAllClientsAsync(string userid);
        Task<Person?> GetClientByIdAsync(int? id, string userid);
        Task UpdateClientAsync(Person client);
        Task DeleteClientAsync(int id, string userid);

        Task<List<Sale>> GetAllSalesAsync(int clientID, string userid);
        Task AddSaleAsync(int clientID, Sale sale, string userid);
    }
}