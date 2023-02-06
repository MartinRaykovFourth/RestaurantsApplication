using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.Api.ApiDTOs;
using RestaurantsApplication.DTOs.DatabaseCopiesDTOs;
using RestaurantsApplication.Repositories.Contracts;

namespace RestaurantsApplication.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClockRecordsController : ControllerBase
    {
        private readonly IRequestRepository _requestRepository;

        public ClockRecordsController(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostClockRecords([FromBody]ClockRecordsDTO dto)
        {
            var request = new RequestCopyDTO()
            {
                Date = dto.Date,
                LocationCode = dto.LocationCode,
                Status = "Pending"
            };

            var records = new List<RecordCopyDTO>();

            foreach (var r in dto.Records)
            {
                records.Add(new RecordCopyDTO
                {
                    Request = request,
                    ClockStatus = r.ClockStatus,
                    ClockValue = r.ClockValue,
                    EmployeeCode = r.EmployeeCode
                });
            }

            _requestRepository.Add(request, records);
            await _requestRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
