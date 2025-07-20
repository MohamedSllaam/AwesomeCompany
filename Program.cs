using AwesomeCompany;
using AwesomeCompany.Entities;
using AwesomeCompany.Models;
using AwesomeCompany.Options;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.ComponentModel.Design;

var builder = WebApplication.CreateBuilder(args);
 builder.Services.ConfigureOptions<DataBaseOptionsSetup>();
 builder.Services.ConfigureOptions<ApplicationOptionsSetup>();

//builder.Services
//    .Configure<ApplicationOptions>(
//    builder.Configuration
//    .GetSection(nameof(ApplicationOptions))
//    ); 

builder.Services.AddDbContext<DatabaseContext>(
    (serviceProvider,dbContextBuilder) => {
        var databaseOptions = serviceProvider.GetService<IOptions<DataBaseOptions>>()!.Value;

        dbContextBuilder.UseSqlServer(databaseOptions.ConnectionString,sqlServerAction =>
        {
            sqlServerAction.EnableRetryOnFailure(databaseOptions.MaxRetryCount);
            sqlServerAction.CommandTimeout(databaseOptions.CommandTimeOut);  
        });
        dbContextBuilder.EnableDetailedErrors(databaseOptions.EnableDetailedErrors);
        dbContextBuilder.EnableSensitiveDataLogging(databaseOptions.EnableSensitiveDataLogging);
    });

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

app.MapGet("options",  
    (IOptions<ApplicationOptions> options,
    IOptionsSnapshot<ApplicationOptions> optionsSnapshot,
    IOptionsMonitor<ApplicationOptions> optionsMonitor
    ) =>
{
    var response = new
    {
        ///singlton , not refresh
        optionsValue=options.Value.ExampleValue,
        //scope refresh
        optionsSnapshot = optionsSnapshot.Value.ExampleValue,
        ///singlton , refresh -- read from config
        MonitorValue = optionsMonitor.CurrentValue.ExampleValue

         
    };
 return Results.Ok(response);
});
app.Run();
