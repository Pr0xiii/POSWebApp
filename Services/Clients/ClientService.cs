using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PointOfSalesWebApplication.Data;
using PointOfSalesWebApplication.Models;
using System.Security.Claims;

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

        public async Task<List<Person>> GetAllClientsAsync(string userid) 
        {
            return await _context.Clients
                .Where(c => c.UserId == userid)
                .ToListAsync();
        }

        public async Task<Person?> GetClientByIdAsync(int? id, string userid) 
        {
            if (id == null) return null;

            return await _context.Clients
                .Where(c => c.UserId == userid)
                .Include(c => c.Sales)
                .ThenInclude(s => s.Lines)
                .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task UpdateClientAsync(Person client) 
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id, string userid) 
        {
            var existing = await GetClientByIdAsync(id, userid);
            if(existing != null) 
            { 
                _context.Clients.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Sale>> GetAllSalesAsync(int clientID, string userid) 
        { 
            var client = await GetClientByIdAsync(clientID, userid);
            return client?.Sales ?? new List<Sale>();
        }
        
        public async Task AddSaleAsync(int clientID, Sale sale, string userid) 
        {
            var client = await GetClientByIdAsync(clientID, userid);
            if(client == null) return;

            if(sale.ClientID == clientID) 
            {
                client.Sales.Add(sale);
            }
            await _context.SaveChangesAsync();
        }
    }
}