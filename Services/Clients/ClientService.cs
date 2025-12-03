using PointOfSalesWebApplication.Models;

namespace PointOfSalesWebApplication.Services
{
    public class ClientService : IClientService
    {
        private static List<Person> _clients = new() 
        {
            new Person {
                ID = 1,
                Name = "John Doe",
                Address = "Avenue des Mars 13, 1200 Brussel",
                Email = "johndoe@gmail.com",
                PhoneNumber = "+32 123 45 67 89"
            },
            new Person {
                ID = 2,
                Name = "Polo Pan",
                Address = "Bvd des papyrus 990, 1200 Brussel",
                Email = "poloandpan@gmail.com",
                PhoneNumber = "+32 987 65 43 21"
            }
        };

        public List<Person> GetAllClients() 
        {
            return _clients;
        }

        public Person? GetClientById(int? id) 
        {
            return _clients.FirstOrDefault(x => x.ID == id);
        }

        public void UpdateClient(Person client) 
        {
            var existing = GetClientById(client.ID);
            if(existing != null) 
            {
                existing.Name = client.Name;
                existing.Address = client.Address;
                existing.Email = client.Email;
                existing.PhoneNumber = client.PhoneNumber;
            }
            else 
            {
                _clients.Add(client);
            }
        }

        public void DeleteClient(int id) 
        {
            var existing = GetClientById(id);
            if(existing != null) { _clients.Remove(existing); }
        }

        public int GetRandomID() 
        {
            int maxID = GetAllClients().Max(x => x.ID);
            return maxID + 1;
        }
    }
}