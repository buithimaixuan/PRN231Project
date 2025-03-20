using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using UserService.Config;
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

// Friend DI
builder.Services.AddScoped<FriendRequestDAO>();
builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();

builder.Services.AddScoped<AuthenConfig>();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<MicroserviceUserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHttpClient("PostService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7231/api/post/"); // URL của PostService bằng swagger
    //client.BaseAddress = new Uri("http://host.docker.internal:5007/api/post/"); // URL của PostService bằng docker
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer {token}' vào ô bên dưới (không có dấu ngoặc kép)",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

var app = builder.Build();
//app.Urls.Add("http://0.0.0.0:5157");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.Urls.Add("http://0.0.0.0:5157");*/

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
