
using Refit;
using HackerNews.API.BackgroundServices;
using System.Diagnostics.CodeAnalysis;
using HackerNews.API.Service.Interface;
using HackerNews.API.Service;
using HackerNews.API.Service.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<INewStoriesService, NewStoriesService>();
builder.Services.AddRefitClient<IHackerNewsApi>().ConfigureHttpClient(
    c => c.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
);
builder.Services.AddHostedService<TimedHostedService>();
builder.Services.AddControllers();

// configuring Swagger/OpenAPI, read more at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("corsPolicy", c =>
{
    c.AllowAnyOrigin().AllowAnyMethod();
}));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseCors("corsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


[ExcludeFromCodeCoverage]
public partial class Program { }