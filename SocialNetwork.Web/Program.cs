using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Data.Context;
using SocialNetwork.Models.Db;
using SocialNetwork.Web.Views.Extensions;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString))
    .AddUnitOfWork()
    .AddCustomRepository<Friend,FriendsRepository>()
    .AddCustomRepository<Message, MessageRepository>()
    .AddIdentity<User, IdentityRole>(opts => {
        opts.Password.RequiredLength = 5;
        opts.Password.RequireNonAlphanumeric = false;
        opts.Password.RequireDigit = false;
        opts.Password.RequireLowercase = false;
        opts.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add AutoMapper
var mapperConfig = new MapperConfiguration(mc => {
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


