using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaneTickets.Hubs;
using PlaneTickets.Persistence;
using PlaneTickets.Services.Airports;
using PlaneTickets.Services.Flights;
using PlaneTickets.Services.Reservations;
using PlaneTickets.Services.Users;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddScoped<IFlightService, FlightService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddScoped<IAirportService, AirportService>();
    builder.Services.AddDbContext<PlaneTicketsDbContext>(options =>
        options.UseSqlite("Data Source=PlaneTickets.db"));
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSignalR();
    builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
        builder.WithOrigins("http://localhost:5173")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod());
});
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }
    );
}

var app = builder.Build();
{
    app.UseExceptionHandler("/error");
    app.UseHttpsRedirection();
    app.UseCors("AllowOrigin");
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapHub<FlightHub>("/flightHub");
    app.MapControllers();
    app.Run();
}