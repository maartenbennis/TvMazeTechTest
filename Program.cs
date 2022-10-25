using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TvMazeTechTest;
using TvMazeTechTest.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ShowsContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ShowsConnStr")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<PeriodicHostedService>();
builder.Services.AddHostedService(
    provider => provider.GetRequiredService<PeriodicHostedService>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
