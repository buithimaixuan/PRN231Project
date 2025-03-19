using CommunicateService.DAOs;
using CommunicateService.Models;
using CommunicateService.Repository.AccountConversationRepo;
using CommunicateService.Repository.ConversationRepo;
using CommunicateService.Repository.MessageRepo;
using CommunicateService.Services.Implement;
using CommunicateService.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MicroserviceCommunicateDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<MessageDAO>();


builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddSingleton<ConversationDAO>();

builder.Services.AddScoped<IAccountConversationService, AccountConversationService>();
builder.Services.AddScoped<IAccountConversationRepository, AccountConversationRepository>();
builder.Services.AddSingleton<AccountConversationDAO>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
//app.MapGet("/", () => "Hello from my project!");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Urls.Add("http://0.0.0.0:5163");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
