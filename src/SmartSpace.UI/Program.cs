using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartSpace.UI;
using SmartSpace.UI.Components.RoomSearch;
using SmartSpace.UI.Components.Reservations;
using SmartSpace.UI.Components.Admin;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5080") });
builder.Services.AddScoped<RoomSearchClient>();
builder.Services.AddScoped<BookingClient>();
builder.Services.AddScoped<MyReservationsClient>();
builder.Services.AddScoped<AdminRoomClient>();
builder.Services.AddScoped<AdminLocationClient>();

await builder.Build().RunAsync();
