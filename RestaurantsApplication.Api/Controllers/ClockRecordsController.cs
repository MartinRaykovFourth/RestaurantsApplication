using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.Api.ApiDTOs;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;

namespace RestaurantsApplication.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClockRecordsController : ControllerBase
    {
        private readonly RestaurantsContext _context;

        public ClockRecordsController(RestaurantsContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostClockRecords([FromBody]ClockRecordsDTO dto)
        {
            var request = new Request()
            {
                Date = dto.Date,
                LocationCode = dto.LocationCode,
                Status = "Pending"
            };

            var records = new List<Record>();

            foreach (var r in dto.Records)
            {
                records.Add(new Record
                {
                    Request = request,
                    ClockStatus = r.ClockStatus,
                    ClockValue = r.ClockValue,
                    EmployeeCode = r.EmployeeCode
                });
            }

            _context.Requests.Add(request);
            _context.Records.AddRange(records);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
