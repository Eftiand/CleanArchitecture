using CleanArchitecture.Application;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using Delta;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder);

builder.Host.AddHosts();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
    await app.InitializeDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseSerilogRequestLogging();
// When Delta can be used with postgres activate this.
//app.UseDelta<ApplicationDbContext>();
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapEndpoints();
app.Map("/", () => Results.Redirect("/swagger"));
app.Run();

public abstract partial class Program;
