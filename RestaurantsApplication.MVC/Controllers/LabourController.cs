using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.MVC.Models.Labour;
using RestaurantsApplication.MVC.Models.Location;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.ProcessErrorMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class LabourController : Controller
    {
        private readonly ILabourService _labourService;
        private readonly ILocationService _locationService;

        public LabourController(ILabourService labourService, ILocationService locationService)
        {
            _labourService = labourService;
            _locationService = locationService;
        }

        public async Task<IActionResult> Cost(string locationCode, DateTime? date)
        {
            try
            {
                DateTime filterDate;
                if (date.HasValue)
                {
                    filterDate = date.Value;
                }
                else
                {
                    filterDate = DateTime.Now.Date;
                }

                ViewBag.Date = filterDate.Date.ToString("yyyy-MM-dd");

                var locationDTOs = await _locationService.GetAllAsync();

                var locationModels = locationDTOs
                    .Select(d => new LocationShortInfoViewModel
                    {
                        Name = d.Name,
                        Code = d.Code
                    });

                if (string.IsNullOrEmpty(locationCode))
                {
                    var modelWithLocations = new LabourCostPerDayViewModel
                    {
                        Locations = locationModels
                    };
                    return View(modelWithLocations);
                }

                var dto = await _labourService.GetLabourCostAsync(filterDate, locationCode);

                var model = new LabourCostPerDayViewModel
                {
                    Employees = dto.Employees,
                    Total = dto.Total,
                    WeeklyCost = dto.WeeklyCost,
                    Locations = locationModels
                };

                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = RetrievingCostsError });
            }
            
        }
    }
}
