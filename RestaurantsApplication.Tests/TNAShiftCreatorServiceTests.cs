using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RestaurantsApplication.Data;
using RestaurantsApplication.Data.Entities;
using RestaurantsApplication.Data.Enums;
using TNAShiftCreatorService;

namespace RestaurantsApplication.Tests
{
    public class TNAShiftCreatorServiceTests
    {
        private RestaurantsContext _context;
        private IServiceCollection _services;
        private ServiceProvider _serviceProvider;

        [SetUp]
        public async Task Setup()
        {
            var contextOptions = new DbContextOptionsBuilder<RestaurantsContext>()
               .UseInMemoryDatabase("RestaurantsApplicationDB")
            .Options;

            _context = new RestaurantsContext(contextOptions);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            await SeedDatabase();

            _services = new ServiceCollection();

            _services.AddDbContext<RestaurantsContext>(
                options =>
                    options.UseInMemoryDatabase("RestaurantsApplicationDB"));

            _services.AddHostedService<Worker>();

            _serviceProvider = _services.BuildServiceProvider();
        }

        [Test]
        public async Task WorkerServiceCreatesShiftWithKnownRole_WhenDataIsAccurateAndEmploymentIsOne_Successfully()
        {
            //Arange
            var request = new Request
            {
                Id = 1,
                Date = new DateTime(2023, 1, 25),
                LocationCode = "PIZZAHUT",
                Status = "Pending",
            };

            var records = new List<Record>()
                {
                    new Record
                    {
                        Id = 1,
                        EmployeeCode = "RAYMAR61",
                        ClockStatus = 0,
                        ClockValue = new DateTime(2023,1,25,10,00,00),
                        RequestId = 1
                    },
                    new Record
                    {
                        Id = 2,
                        EmployeeCode = "RAYMAR61",
                        ClockStatus = (ClockStatus)1,
                        ClockValue = new DateTime(2023,1,25,11,00,00),
                        RequestId = 1
                    },
                    new Record
                    {
                        Id = 3,
                        EmployeeCode = "RAYMAR61",
                        ClockStatus = (ClockStatus)2,
                        ClockValue = new DateTime(2023,1,25,12,00,00),
                        RequestId = 1
                    },
                    new Record
                    {
                        Id = 4,
                        EmployeeCode = "RAYMAR61",
                        ClockStatus = (ClockStatus)3,
                        ClockValue = new DateTime(2023,1,25,19,00,00),
                        RequestId = 1
                    },
                };

            _context.Requests.Add(request);
            _context.Records.AddRange(records);
            await _context.SaveChangesAsync();

            var worker = _serviceProvider.GetService<IHostedService>() as Worker;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10));
            //Act
            await worker!.StartAsync(cancellationTokenSource.Token);
            await worker!.StopAsync(cancellationTokenSource.Token);

            //Assert
            Assert.That(_context.Shifts.Count() == 1);

            var shift = await _context.Shifts.FindAsync(1);

            Assert.That(shift.Start == new DateTime(2023, 1, 25, 10, 00, 00));
            Assert.That(shift.BreakStart == new DateTime(2023, 1, 25, 11, 00, 00));
            Assert.That(shift.BreakEnd == new DateTime(2023, 1, 25, 12, 00, 00));
            Assert.That(shift.End == new DateTime(2023, 1, 25, 19, 00, 00));
            Assert.That(shift.RoleId == 3);
            Assert.That(shift.DepartmentId == 3);
            Assert.That(shift.EmployeeId == 2);

            var existingRequest = await _context.Requests.Where(r => r.Id == 1).SingleAsync();

            Assert.That(existingRequest.Status == "Completed");
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private async Task SeedDatabase()
        {
            SeedLocations();
            SeedDepartments();
            SeedEmployees();
            SeedRoles();
            SeedEmployments();

            await _context.SaveChangesAsync();
        }

        private void SeedLocations()
        {
            var locations = new List<Location>()
            {
                new Location
                {
                    Id = 1,
                    Name = "Mc Donalds",
                    Code = "MCDONLDS"
                },
                new Location
                {
                    Id = 2,
                    Name = "Pizza Hut",
                    Code = "PIZZAHUT"
                }
            };

            _context.Locations.AddRange(locations);
        }

        private void SeedDepartments()
        {
            var departments = new List<Department>()
            {
                new Department
                {
                    Id = 1,
                    LocationId = 1,
                    Name = "Mc Donalds Department 1"
                },
                new Department
                {
                    Id = 2,
                    LocationId = 1,
                    Name = "Mc Donalds Department 2"
                },
                new Department
                {
                    Id = 3,
                    LocationId = 2,
                    Name = "Pizza Hut Department 1"
                }
            };

            _context.Departments.AddRange(departments);
        }
        private void SeedEmployees()
        {
            var employees = new List<Employee>()
            {
                new Employee
                {
                    Id = 1,
                    FirstName = "Martin",
                    LastName = "Raykov",
                    Code = "MARRAY16"
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Rayko",
                    LastName = "Martinov",
                    Code = "RAYMAR61"
                }
            };

            _context.Employees.AddRange(employees);
        }

        private void SeedRoles()
        {
            var roles = new List<Role>()
            {
                new Role
                {
                    Id = 1,
                    Name = "Manager"
                },
                new Role
                {
                    Id = 2,
                    Name = "Waiter"
                },
                 new Role
                {
                    Id = 3,
                    Name = "Chef"
                }
            };

            _context.Roles.AddRange(roles);
        }
        
        private void SeedEmployments()
        {
            var employments = new List<Employment>()
            {
               new Employment
               {
                   Id = 1,
                   DepartmentId = 1,
                   EmployeeId = 1,
                   IsMain = true,
                   RoleId = 1,
                   StartDate = new DateTime(2023,1,1),
                   EndDate = new DateTime(2023,12,30),
                   Rate = 20
               },
               new Employment
               {
                   Id = 2,
                   DepartmentId = 2,
                   EmployeeId = 1,
                   IsMain = false,
                   RoleId = 2,
                   StartDate = new DateTime(2023,1,1),
                   EndDate = new DateTime(2023,2,15),
                   Rate = 5
               },
               new Employment
               {
                   Id = 3,
                   DepartmentId = 3,
                   EmployeeId = 2,
                   IsMain = true,
                   RoleId = 3,
                   StartDate = new DateTime(2023,1,1),
                   Rate = 10
               },
            };

            _context.Employments.AddRange(employments);
        }
    }
}