using cwiczenia_5.Models;
using cwiczenia_5.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cwiczenia_5.Services
{
    public class DbService : IDbService
    {

        private readonly cw5Context _dbContext;
        public DbService(cw5Context dbContext) { 
        _dbContext = dbContext;
        }

        public async Task<IEnumerable<SomeSortOfTrip>> GetTrips()
        {
            return await _dbContext.Trips
      //          .Include(e => e.CountryTrips)
      //          .Include(e => e.ClientTrips)
                .Select(e =>new SomeSortOfTrip {
                Name = e.Name,
                Description = e.Description,
                MaxPeople = e.MaxPeople,
                DateFrom = e.DateFrom,
                DateTo = e.DateTo,
                Countries = e.CountryTrips.Select(e => new SomeSortOfCountry { Name = e.IdCountryNavigation.Name}).ToList(),
                Clients = e.ClientTrips.Select(e => new SomeSortOfClient { FirstName = e.IdClientNavigation.FirstName, LastName = e.IdClientNavigation.LastName}).ToList(),
                }).OrderByDescending(c => c.DateFrom).ToListAsync();
        }

        public async Task<bool> RemoveClient(int id)
        {

            // notatki
            // dodawanie
            // var addTrip = new Trip {IdTrip = id, Name = "nazwaWycieczki"};
            // _dbContext.Add(addTrip);

            // edycja
            // var editTrip = await _dbContext.Trips.Where(e => e.IdTrip == id).FirstOrDefaultAsync();
            // editTrip.Name = "aaa";

            // 1 metoda
            //  var trip = await _dbContext.Trips.Where(e => e.IdTrip == id).FirstOrDefaultAsync();

            // check if client has some trips
            var trips = await CheckClientsTrips(id);

            if (!trips.Any())
            {
                // 2 metoda
                var client = new Client { IdClient = id };
                _dbContext.Attach(client);

                _dbContext.Remove(client);

                await _dbContext.SaveChangesAsync();
                return true;
            }
            else {
                return false;
            }

        }

        public async Task<IEnumerable<SomeSortOfClientTrip>> CheckClientsTrips(int id)
        {
            return await _dbContext.ClientTrips.Select(e => new SomeSortOfClientTrip { IdTrip = e.IdTrip, IdClient = e.IdClient }).Where(e => e.IdClient == id).ToListAsync();
        }

        public async Task<bool> AddClientToTrip(HttpPostInput httpPostInput)
        {

            // checking if client exists
            var client = await CheckIfClientExists(httpPostInput.Pesel);

            // if client do not exist add to the db
            if (!client.Any()) {
                 var addClient = new Client {FirstName = httpPostInput.FirstName, LastName = httpPostInput.LastName, Email = httpPostInput.Email, Telephone = httpPostInput.Telephone, Pesel = httpPostInput.Pesel };
                 _dbContext.Add(addClient);
                await _dbContext.SaveChangesAsync();
            }

            // check if client is already added to this trip
            var clientId = GetClientId(httpPostInput.Pesel);
            var clientTrip = await CheckIfClientAlreadyAdded(clientId, httpPostInput.IdTrip);
            if (clientTrip.Any()) {
                return false;
            }

            // check if trip exists
            var trip = await CheckIfTripExists(httpPostInput.IdTrip);
            if (!trip.Any()) {
                return false;
            }

            // check paymentDate
            var dateTimeToInsert = (DateTime?)null;
            if (httpPostInput.PaymentDate != null) {

                // edit date string to proper format
                string[] splitted = httpPostInput.PaymentDate.Split("/");
                string[] newDate;

                for (int i = 0; i < splitted.Length; i++) { 
                    if(splitted[i].Length == 1)
                    {
                        splitted[i] = "0" + splitted[i];
                    }
                }
                string joined = string.Join("/", splitted);

                dateTimeToInsert = DateTime.ParseExact(joined, "MM/dd/yyyy", null);
            }
           
            // add
            var addClientToTrip = new ClientTrip {IdClient = clientId , IdTrip = httpPostInput.IdTrip, RegisteredAt = DateTime.Now, PaymentDate = dateTimeToInsert };
             _dbContext.Add(addClientToTrip);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SomeSortOfClient>> CheckIfClientExists(string pesel)
        {
            return await _dbContext.Clients.Select(e => new SomeSortOfClient { FirstName = e.FirstName, LastName = e.LastName, Pesel = e.Pesel }).Where(e => e.Pesel == pesel).ToListAsync();
        }

        public async Task<IEnumerable<SomeSortOfTrip>> CheckIfTripExists(int idTrip)
        {
            return await _dbContext.Trips.Select(e => new SomeSortOfTrip {IdTrip = e.IdTrip, Name = e.Name, Description = e.Description, DateFrom = e.DateFrom, DateTo = e.DateTo, MaxPeople = e.MaxPeople }).Where(e => e.IdTrip == idTrip).ToListAsync();
        }

        public async Task<IEnumerable<SomeSortOfClientTrip>> CheckIfClientAlreadyAdded(int IdClient, int IdTrip)
        {
            return await _dbContext.ClientTrips.Select(e => new SomeSortOfClientTrip { IdClient = e.IdClient, IdTrip = e.IdTrip }).Where(e => e.IdClient == IdClient).Where(e => e.IdTrip == IdTrip).ToListAsync();
        }

        public int GetClientId(string pesel)
        {
            return _dbContext.Clients.Where(e=>e.Pesel == pesel).Select(e => e.IdClient).SingleOrDefault();
        }
    }
}
