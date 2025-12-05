using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public interface IClientService 
    {
        public List<Person> GetAllClients();
        public Person? GetClientById(int? id);
        public void UpdateClient(Person client);
        public void DeleteClient(int id);
        public int GetRandomID();

        public List<Sale> GetAllSales(int clientID);
        public void AddSale(int clientID, Sale sale);
    }
}