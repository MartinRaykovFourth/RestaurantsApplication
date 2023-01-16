using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;

namespace TNAShiftCreatorService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;
                    var connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");

                    services.AddDbContext<RestaurantsContext>(options =>
                    options.UseSqlServer(connectionString));

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}