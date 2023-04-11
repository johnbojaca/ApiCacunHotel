using CancunHotel.Entities.Bookings;
using CancunHotel.Entities.Guests;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c=>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CANCUN Hotel", Description = "API for managing Cancun Hotel", Version = "v1" })
);

builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IValidator<Booking>, BookingValidator>();
builder.Services.AddTransient<IValidator<ListGuest>, ListGuestValidator>();

builder.Services.AddAuthorization();



var app = builder.Build();



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
