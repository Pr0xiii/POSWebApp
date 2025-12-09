using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class ClientService : IClientService
    {
        private readonly PosContext _context;
        private readonly Random _rand = new Random();

        public ClientService(PosContext context)
        {
            _context = context;
        }

        public List<Person> GetAllClients() 
        {
            return _context.Clients.ToList();
        }

        public Person? GetClientById(int? id) 
        {
            if (id == null) return null;

            return _context.Clients
                .Include(c => c.Sales)
                .ThenInclude(s => s.Lines)
                .FirstOrDefault(c => c.ID == id);
        }

        public void UpdateClient(Person client) 
        {
            //var existing = GetClientById(client.ID);
            //if(existing != null) 
            //{
            //    existing.Name = client.Name;
            //    existing.Address = client.Address;
            //    existing.Email = client.Email;
            //    existing.PhoneNumber = client.PhoneNumber;
            //}
            //else 
            //{
            //    _context.Clients.Add(client);
            //}
            _context.Clients.Update(client);
            _context.SaveChanges();
        }

        public void DeleteClient(int id) 
        {
            var existing = GetClientById(id);
            if(existing != null) 
            { 
                _context.Clients.Remove(existing);
                _context.SaveChanges();
            }
        }

        public int GetRandomID() 
        {
            int id;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
            }
            while (GetAllClients().Any(s => s.ID == id));

            return id;
        }

        public List<Sale> GetAllSales(int clientID) 
        { 
            var client = GetClientById(clientID);
            return client?.Sales ?? new List<Sale>();
        }
        
        public void AddSale(int clientID, Sale sale) 
        {
            var client = GetClientById(clientID);
            if(client == null) return;

            if(sale.ClientID == clientID) 
            {
                client.Sales.Add(sale);
            }
            _context.SaveChanges();
        }
    }
}