using GymManagmet.DAL.Models;
using GymManagmet.DAL.Repositorites.Classes;
using GymManagmet.DAL.Repositorites.Interfaces;
using GymManagmet.DbContexts;
using GymManagmet.Models;
using GymMangment.BLL;
using GymMangment.BLL.Services.classes;
using GymMangment.BLL.Services.Interfaces;
using GymMangment.PL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymMangmentSystem
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<GymDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

            } );
            builder.Services.AddScoped(typeof(IgenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ImemberService , MemberService>();
            builder.Services.AddScoped<IPlanServices, PlanService>();
            builder.Services.AddScoped<ITrainerService, TrainerService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
            builder.Services.AddScoped<IMembershipService, MembershipService>();
            builder.Services.AddScoped<IBookingRepository , BookingRepository>();
            builder.Services.AddScoped<IbokkingService, BokkingService>();
            builder.Services.AddScoped<IAttachmentService , AttachmentService>();
            builder.Services.AddIdentity<ApplicationUser , IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                config.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<GymDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
               
                //options.LoginPath = "/Account/Login";
          
                //options.AccessDeniedPath = "/Account/AccessDenied";
            });


            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));


            var app = builder.Build();

            await app.MigrateAndSeedAsync();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
