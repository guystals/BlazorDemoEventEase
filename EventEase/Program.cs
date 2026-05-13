using EventEase;
using EventEase.Repositories;
using EventEase.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register BrowserStorageService for localStorage/sessionStorage access
builder.Services.AddScoped<BrowserStorageService>();

// Register UserSessionService as scoped to track user activity
builder.Services.AddScoped<UserSessionService>();

// Register AttendanceService as singleton using interface for attendance tracking
builder.Services.AddSingleton<IAttendanceService, AttendanceService>();

// Register EventRepository as singleton to maintain state across navigations
builder.Services.AddSingleton<IEventRepository>(sp =>
{
    var repository = new EventRepository();

    // Uncomment the line below to generate test data for performance testing
    // repository.GenerateTestData(500); // Generates 500 test events

    return repository;
});

// Register EventService as singleton using interface
builder.Services.AddSingleton<IEventService, EventService>();

await builder.Build().RunAsync();
