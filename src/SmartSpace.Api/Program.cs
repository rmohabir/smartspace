using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SmartSpace.Api.Data;
using SmartSpace.Api.Infrastructure;
using SmartSpace.Api.Security;

var builder = WebApplication.CreateBuilder(args);

var developmentIdentity = builder.Configuration
	.GetSection(DevelopmentIdentityOptions.SectionName)
	.Get<DevelopmentIdentityOptions>() ?? new DevelopmentIdentityOptions();

if (developmentIdentity.Enabled && !builder.Environment.IsDevelopment())
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseCors("Development");
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.Run();
