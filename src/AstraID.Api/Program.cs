using AstraID.Infrastructure.Extensions;
using AstraID.Persistence;
using AstraID.Persistence.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddPersistence(builder.Configuration)
                .AddIdentityAndAuth(builder.Configuration)
                .AddOpenIddictServer(builder.Configuration)
                .AddTelemetry(builder.Configuration)
                .AddAstraIdOptions(builder.Configuration);

builder.Services.AddCors(options =>
{
    var origins = (builder.Configuration["ASTRAID_ALLOWED_CORS"] ?? string.Empty)
        .Split(';', StringSplitOptions.RemoveEmptyEntries);
    options.AddPolicy("cors", p => p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseCors("cors");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();
