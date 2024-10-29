using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
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
app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapEndpoints();
app.Map("/", () => Results.Redirect("/swagger"));
app.Run();

public abstract partial class Program;
