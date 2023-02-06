using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Repositories;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;
using RestaurantsApplication.Services.RecordValidator;
using RestaurantsApplication.Services.Services;

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

                    services.AddScoped<IRequestRepository, RequestRepository>();
                    services.AddScoped<IShiftRepository, ShiftRepository>();
                    services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                    services.AddScoped<IRecordValidator, RecordValidator>();

                    services.AddScoped<ITNAService, TNAService>();

                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();
        }
    }
}