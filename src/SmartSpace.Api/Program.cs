using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SmartSpace.Api;
using SmartSpace.Api.Data;
using SmartSpace.Api.Features.Reservations;
using SmartSpace.Api.Features.Rooms;
using SmartSpace.Api.Infrastructure;
using SmartSpace.Api.Security;

var builder = WebApplication.CreateBuilder(args);

var developmentIdentity = builder.Configuration
	.GetSection(DevelopmentIdentityOptions.SectionName)
	.Get<DevelopmentIdentityOptions>() ?? new DevelopmentIdentityOptions();

if (developmentIdentity.Enabled && !developmentIdentity.IsAllowedInEnvironment(builder.Environment))
{
	throw new InvalidOperationException("Development identity is only allowed in Development.");
}

builder.Services.AddProblemDetails(ProblemDetailsMapping.Configure);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<SmartSpaceDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("SmartSpace")));
builder.Services.AddCors(options => options.AddPolicy("Development", policy =>
	policy.WithOrigins("http://localhost:3000")
		.AllowAnyHeader()
		.AllowAnyMethod()));

if (developmentIdentity.Enabled)
{
	builder.Services
		.AddAuthentication("Development")
		.AddScheme<AuthenticationSchemeOptions, DevelopmentIdentityHandler>("Development", _ => { });
}
else
{
	builder.Services.AddAuthentication();
}

builder.Services.AddAuthorization(options =>
	options.AddPolicy(AuthorizationPolicies.Administrator, policy =>
		policy.RequireRole("Administrator")));
builder.Services.AddScoped<RoomAvailabilityService>();
builder.Services.AddScoped<ReservationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<SmartSpaceDbContext>();
	await db.Database.MigrateAsync();
	await DevelopmentDataSeeder.SeedAsync(db);

	app.MapOpenApi();
	app.UseCors("Development");
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
	name = "SmartSpace",
	status = "Foundation",
	release = "Release 1",
	message = "Vergaderruimte-reserveringen voor BIDN"
}));

app.MapRoomsEndpoints();
app.MapReservationEndpoints();

app.Run();
