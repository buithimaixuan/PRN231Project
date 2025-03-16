using Microsoft.EntityFrameworkCore;
using UserService.DAOs;
using UserService.PasswordHashing;
using UserService.Repositories;
using UserService.Repositories.AccountRepo;
using UserService.Services.Implement;
using UserService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Account DI
builder.Services.AddScoped<AccountDAO>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<PasswordHasher>();


builder.Services.AddHttpClient();

builder.Services.AddDbContext<MicroserviceUserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
