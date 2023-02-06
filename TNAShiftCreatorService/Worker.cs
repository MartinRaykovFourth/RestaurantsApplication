using RestaurantsApplication.Services.Contracts;

namespace TNAShiftCreatorService
{
    public class Worker : BackgroundService
    {
        private readonly ITNAService _tnaService;

        public Worker(IServiceProvider serviceProvider)
        {
            _tnaService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ITNAService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _tnaService.ProcessRequestsAsync();

                Thread.Sleep(60 * 1000);
            }
        }
       
    }
}
