using PostService.DAOs;
using PostService.Repositories.Implement;
using PostService.Repositories.Interface;
using PostService.Services.Implement;
using PostService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// News DI
builder.Services.AddScoped<NewsDAO>();
builder.Services.AddScoped<CategoryNewsDAO>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<ICategoryNewsRepository, CategoryNewsRepository>();
builder.Services.AddScoped<INewsService, NewsService>();



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
