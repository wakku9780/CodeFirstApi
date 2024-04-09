using CodeFirstApi.Interfaces;
using CodeFirstApi.Models;
using CodeFirstApi.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CodeFirstApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var provider = builder.Services.BuildServiceProvider();
            var config = provider.GetRequiredService<IConfiguration>();
            //redis-connection

            builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(builder.Configuration.GetSection("RedisConnection:Connection").Value!.ToString()));

            builder.Services.AddDbContext<StudentDBContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));
            builder.Services.AddTransient<IRedisCache, RedisCache>();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

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

            app.Run();
        }
    }
}
