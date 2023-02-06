using Microsoft.AspNetCore.Mvc;
using RestaurantsApplication.MVC.Models.Submition;
using RestaurantsApplication.Services.Contracts;
using static RestaurantsApplication.MVC.Messages.ProcessErrorMessages;

namespace RestaurantsApplication.MVC.Controllers
{
    public class SubmitionController : Controller
    {
        private readonly ISubmitionService _submitionService;
        public SubmitionController(ISubmitionService submitionService)
        {
            _submitionService = submitionService;
        }

        public async Task<IActionResult> ViewAll(DateTime? date)
        {
            try
            {
                DateTime filterDate;
                if (!date.HasValue)
                {
                    filterDate = DateTime.Now.Date;
                }
                else
                {
                    filterDate = date.Value.Date;
                }

                ViewBag.Date = filterDate.Date.ToString("yyyy-MM-dd");

                var dtos = await _submitionService.GetRequestsByDateAsync(filterDate);

                var models = dtos
                    .Select(d => new SubmitionShortInfoViewModel
                    {
                        Date = d.Date,
                        FailMessage = d.FailMessage,
                        LocationCode = d.LocationCode,
                        Status = d.Status
                    });

                return View(models);
            }
            catch (Exception)
            {
                return RedirectToAction("Error", "Home", new { message = RetrievingSubmitionsError });
            }

        }
    }
}
