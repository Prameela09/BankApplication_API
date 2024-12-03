using BankManagement.Database.DbContexts;
using BankManagement.Database.UserData.Implementations;
using BankManagement.Database.UserData.Interfaces;
using BankManagement.Services.UserServices.Implementations;
using BankManagement.Services.UserServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using BankManagement.Services.Mappers;
using BankManagement.Database.NotificationData.Interfaces;
using BankManagement.Database.NotificationData.Implementations;
using BankManagement.Services.NotificationServices.Interfaces;
using BankManagement.Services.NotificationServices.Implementations;
using BankManagement.Database.BranchData.Interfaces;
using BankManagement.Database.BranchData.Implementations;
using BankManagement.Services.BranchServices.Interfaces;
using BankManagement.Services.BranchServices.Implementations;
using BankManagement.Database.AccountData.Interfaces;
using BankManagement.Database.AccountData.Implementations;
using BankManagement.Services.AccountServices.Interfaces;
using BankManagement.Services.AccountServices.Implementations;
using BankManagement.Database.TransactionData.Interfaces;
using BankManagement.Database.TransactionData.Implementations;
using BankManagement.Services.TransactionServices.Interfaces;
using BankManagement.Services.TransactionServices.Implementations;
using BankManagement.Utilities.ExceptionHandlers;
using BankManagement.API.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    // Add security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Initiating serilog 
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/BankManagementLog.txt")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddDistributedMemoryCache();

 // Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework with SQL Server
builder.Services.AddDbContext<BankDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BankConnection")));

builder.Services.AddAutoMapper(typeof(EntityMapper));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>(); 
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IBranchServices, BranchServices>();
builder.Services.AddScoped<ITransactionServices, TransactionServices>();

builder.Services.AddTransient<UserValidationFilterAttribute>();
builder.Services.AddTransient<BranchValidationFilterAttribute>();
builder.Services.AddTransient<AccountValidationFilterAttribute>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});

// Configure JWT authentication directly from appsettings.json
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = jwtSection["Key"];
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
});

builder.Services.AddScoped<GlobalExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank Management API v1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseSession(); 
app.UseAuthentication(); 
app.UseAuthorization();  

app.MapControllers();

app.Run();