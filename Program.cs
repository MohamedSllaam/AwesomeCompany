using AwesomeCompany;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(
    e => e.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();
app.UseHttpsRedirection();
app.MapGet("/", () => "Hello World!");

app.Run();
