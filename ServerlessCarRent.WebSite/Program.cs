using Microsoft.Extensions.Configuration;
using ServerlessCarRent.RestClient;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
	config.AddJsonFile(
		 "appsettings.json", optional: false, reloadOnChange: true);
	config.AddJsonFile(
		"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
	config.AddJsonFile(
		"appsettings.local.json", optional: true, reloadOnChange: true);
	config.AddJsonFile(
		"appsettings.local.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
});

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddManagementClients();

builder.Services.AddControllersWithViews();

builder.Services.AddAutoMapper(Assembly.GetEntryAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
