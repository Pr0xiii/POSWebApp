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

        public async Task<List<Person>> GetAllClientsAsync() 
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Person?> GetClientByIdAsync(int? id) 
        {
            if (id == null) return null;

            return await _context.Clients
                .Include(c => c.Sales)
                .ThenInclude(s => s.Lines)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task UpdateClientAsync(Person client) 
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
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id) 
        {
            var existing = await GetClientByIdAsync(id);
            if(existing != null) 
            { 
                _context.Clients.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetRandomIDAsync()
        {
            int id;
            bool exists;

            do
            {
                id = _rand.Next(1000, 10000); // 1000–9999
                exists = await _context.Clients.AnyAsync(c => c.ID == id);
            }
            while (exists);

            return id;
        }

        public async Task<List<Sale>> GetAllSalesAsync(int clientID) 
        { 
            var client = await GetClientByIdAsync(clientID);
            return client?.Sales ?? new List<Sale>();
        }
        
        public async Task AddSaleAsync(int clientID, Sale sale) 
        {
            var client = await GetClientByIdAsync(clientID);
            if(client == null) return;

            if(sale.ClientID == clientID) 
            {
                client.Sales.Add(sale);
            }
            await _context.SaveChangesAsync();
        }
    }
}