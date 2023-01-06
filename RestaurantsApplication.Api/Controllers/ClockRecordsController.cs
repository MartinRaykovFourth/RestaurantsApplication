using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.Api.ApiDTOs;

namespace RestaurantsApplication.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClockRecordsController : ControllerBase
    {

        [HttpPost]
        public IActionResult PostClockRecords(ClockRecordsDTO dto)
        {
            
            return Ok();
        }
    }
}
