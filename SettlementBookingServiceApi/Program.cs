using Configurations;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Serilog;
using Services.Implementations;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) => {
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/SettlementBookingServiceApi.txt", rollingInterval: RollingInterval.Day);
});

// Add services to the container.
builder.Services.Configure<BookingOptions>(builder.Configuration.GetSection("BookingOptions"));
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddSingleton<IBookingRepository, BookingRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
