using cwiczenia_5.Models.DTO;
using cwiczenia_5.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cwiczenia_5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public TripsController(IDbService dbService) {
            _dbService = dbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTrips() { 
        var trips = await _dbService.GetTrips();
            return Ok(trips);
        }

        [HttpPost]
        [Route("{id}/clients")]
        public async Task<IActionResult> AddClientToTrip(HttpPostInput httpPostInput)
        {
            var added = await _dbService.AddClientToTrip(httpPostInput);
            if (added)
            {
                return Ok("Added client to trip.");
            }
            else
            {
                return BadRequest("Client cannot be added to trip.");
            }
        }
    }
}
