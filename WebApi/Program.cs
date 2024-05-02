using Configurations;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using Services.Implementations;
using Services.Interfaces;
using System.Net;

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
builder.Services.AddSingleton<IBookingOptionsService, BookingOptionsService>();
builder.Services.AddScoped(typeof(IApiLogger<>), typeof(ApiLogger<>));
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddSingleton<IBookingRepository, BookingRepository>();

// Add API versioning
builder.Services.AddApiVersioning(options => {
    options.ApiVersionReader = new MediaTypeApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Settlement Booking Service Api", Version = "v1" });   
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Version 1");       
    });
}

app.UseHttpsRedirection();

// Global exception handler
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Log.Error($"Something went wrong: {contextFeature.Error}");

            var errorResponse = new
            {
                context.Response.StatusCode,
                Message = "Oops something went wrong. Please try again later."
            };

            var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    });
});

app.UseAuthorization();

app.MapControllers();

app.Run();
