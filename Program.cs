using AwesomeCompany;
using AwesomeCompany.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DatabaseContext>(
    e => e.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();
app.UseHttpsRedirection();
//app.MapGet("/", () => "Hello World!");
app.MapPut("companies/{companyId:int}", async (int companyId, DatabaseContext dbContext) =>
{
    var company = await dbContext
    .Set<Company>()
    .Include(x=>x.Employees)
   // .AsNoTracking()
    .FirstOrDefaultAsync(c=>c.Id == companyId);

    if(company is null)
    {
        return Results.NotFound($"this comapny with Id '{companyId}' was not found.");
    }
    foreach (var employee in company.Employees)
    {
        employee.Salary += 1.1m;
    }

    company.LastSalaryUpdateUtc = DateTime.UtcNow;

  await  dbContext.SaveChangesAsync();
    return Results.NoContent();
});
app.Run();
