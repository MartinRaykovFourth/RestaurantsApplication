using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data;
using RestaurantsApplication.Repositories;
using RestaurantsApplication.Repositories.Contracts;
using RestaurantsApplication.Services.Contracts;
using RestaurantsApplication.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RestaurantsContext>(
    options =>
        options.UseSqlServer(
            connectionString,
            b => b.MigrationsAssembly("RestaurantsApplication.Data")));

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmploymentRepository, EmploymentRepository>();
builder.Services.AddScoped<ISubmitionRepository, SubmitionRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();

builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmploymentService, EmploymentService>();
builder.Services.AddScoped<IValidatorService, ValidatorService>();
builder.Services.AddScoped<ISubmitionService, SubmitionService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<ILabourService, LabourService>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
