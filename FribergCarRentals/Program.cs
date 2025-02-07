using FribergCarRentals.Data;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using System;
using FribergCarRentals.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Elfie.Serialization;
using System.Drawing;

namespace FribergCarRentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = FribergCarRentals; Integrated Security = True; Connect Timeout = 30; Encrypt = False; Trust Server Certificate = False; Application Intent = ReadWrite; Multi Subnet Failover = False"));
            builder.Services.AddTransient<IRepository<Car>, CarRepository>();
            builder.Services.AddTransient<IRepository<UserRole>, UserRoleRepository>();
            builder.Services.AddTransient<ICustomerRepository,CustomerRepository>();
            builder.Services.AddTransient<IBookingRepository,BookingRepository>();
            builder.Services.AddTransient<IAdminRepository, AdminRepository>();
 
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/User/Login";
                options.AccessDeniedPath = "/User/AccessDenied";
            });
            var app = builder.Build();

            app.UseAuthentication(); // Must come before UseAuthorization
            app.UseAuthorization();

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

            // fick skapa en egen routing för Unbooking methoden i BookingController
            app.MapControllerRoute(
                name: "booking",
                pattern: "Booking/UnbookBooking/{bookingId}",
                defaults: new { controller = "Booking", action = "UnbookBooking" });

            app.Run();
        }
    }
}
