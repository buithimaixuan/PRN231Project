using BookingService.DAOs;
using BookingService.Models;
using BookingService.Repositories.BookingServiceRepo;
using BookingService.Repositories.CategoryServiceRepo;
using BookingService.Repositories.RatingServiceRepo;
using BookingService.Repositories.ServiceRepo;
using BookingService.Services.Implement;
using BookingService.Services.Interface;
using Microsoft.EntityFrameworkCore;
using RequestService.DAOs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MicroserviceBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IServicingService, ServicingService>();

builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<ServiceDAO>();

builder.Services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();
builder.Services.AddScoped<BookingServiceDAO>();

builder.Services.AddScoped<ICategoryServiceRepository, CategoryServiceRepository>();
builder.Services.AddScoped<CategoryServiceDAO>();

builder.Services.AddScoped<IRatingServiceRepository, RatingServiceRepository>();
builder.Services.AddScoped<RatingServiceDAO>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.Urls.Add("http://0.0.0.0:5122");*/

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
