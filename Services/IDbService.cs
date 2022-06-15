using cwiczenia_5.Models;
using cwiczenia_5.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cwiczenia_5.Services
{
    public interface IDbService
    {

        Task<IEnumerable<SomeSortOfTrip>> GetTrips();
        Task<bool> RemoveClient(int id);
        Task<IEnumerable<SomeSortOfClientTrip>> CheckClientsTrips(int id);
        Task<bool> AddClientToTrip(HttpPostInput httpPostInput);
        Task<IEnumerable<SomeSortOfClient>> CheckIfClientExists(string pesel);
        Task<IEnumerable<SomeSortOfTrip>> CheckIfTripExists(int id);
        Task<IEnumerable<SomeSortOfClientTrip>> CheckIfClientAlreadyAdded(int IdClient, int IdTrip);
        int GetClientId(string pesel);
    }
}
