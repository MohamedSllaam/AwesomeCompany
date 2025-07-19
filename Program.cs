using AwesomeCompany;
using AwesomeCompany.Entities;
using AwesomeCompany.Models;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.Design;

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
    .FirstOrDefaultAsync(c=>c.Id == companyId);

    if(company is null)
    {
        return Results.NotFound($"this comapny with Id '{companyId}' was not found.");
    }
   
    await dbContext.Database.BeginTransactionAsync();

    await dbContext.Database.ExecuteSqlInterpolatedAsync(
        $"Update Employees set Salary= Salary * 1.1 where CompanyId = {company.Id}");

    company.LastSalaryUpdateUtc = DateTime.UtcNow;

    await dbContext.SaveChangesAsync();

    await dbContext.Database.CommitTransactionAsync();

    return Results.NoContent();
});

app.MapPut("companies-dapper/{companyId:int}", async (int companyId, DatabaseContext dbContext) =>
{
    var company = await dbContext
    .Set<Company>()
    .FirstOrDefaultAsync(c=>c.Id == companyId);

    if(company is null)
    {
        return Results.NotFound($"this comapny with Id '{companyId}' was not found.");
    }
   
   var transaction= await dbContext.Database.BeginTransactionAsync();

    await dbContext.Database.GetDbConnection()
        .ExecuteAsync(
        "Update Employees set Salary= Salary * 1.1 where CompanyId=@CompanyId", new { CompanyId = company.Id },
        transaction.GetDbTransaction());
    company.LastSalaryUpdateUtc = DateTime.UtcNow;

    await dbContext.SaveChangesAsync();

    await dbContext.Database.CommitTransactionAsync();

    return Results.NoContent();
});


app.MapGet("companies/{companyId:int}", async (int companyId, DatabaseContext dbContext) =>
{
    var company = await dbContext
    .Set<Company>()
    .AsNoTracking()
    .FirstOrDefaultAsync(c => c.Id == companyId);

    if (company is null)
    {
        return Results.NotFound($"this comapny with Id '{companyId}' was not found.");
    }

    var response = new CompanyResponse(company.Id, company.Name);
    return Results.Ok(response);
});

app.Run();
