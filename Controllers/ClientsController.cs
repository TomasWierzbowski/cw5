using cwiczenia_5.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cwiczenia_5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : Controller
    {
        private readonly IDbService _dbService;
        public ClientsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveClient(int id)
        {

            var deleted = await _dbService.RemoveClient(id);
            if (deleted)
            {
                return Ok("Removed client");
            }
            else
            {
                return BadRequest("Client cannot be removed as he has trips assigned to him");
            }

        }
    }
}
