using PostService.DAOs;
using PostService.Repositories.Implement;
using PostService.Repositories.Interface;
using PostService.Services.Implement;
using PostService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<CategoryNewsDAO>();
builder.Services.AddScoped<CategoryPostDAO>();
builder.Services.AddScoped<CommentPostDAO>();
builder.Services.AddScoped<LikePostDAO>();
builder.Services.AddScoped<NewsDAO>();
builder.Services.AddScoped<PostDAO>();
builder.Services.AddScoped<PostImageDAO>();
builder.Services.AddScoped<SharePostDAO>();
builder.Services.AddScoped<ViewDAO>();

builder.Services.AddScoped<ICategoryNewsRepository, CategoryNewsRepository>();
builder.Services.AddScoped<ICategoryPostRepository, CategoryPostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ILikePostRepository, LikePostRepository>();
builder.Services.AddScoped<INewsRepository, NewsRepository>();
builder.Services.AddScoped<IPostImageRepository, PostImageRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ISharePostRepository, SharePostRepository>();
builder.Services.AddScoped<IViewRepository, ViewRepository>();

builder.Services.AddScoped<ICategoryPostService, CategoryPostService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPostImageService, PostImageService>();
builder.Services.AddScoped<IPostService, PostsService>();
builder.Services.AddScoped<IViewService, ViewService>();


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
