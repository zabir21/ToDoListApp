using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ToDoListApp.BLL.Services;
using ToDoListApp.DAL.Entity.Identity;
using ToDoList.DAL.DbContext;
using Newtonsoft.Json.Converters;
using ToDoListApp.BLL.Services.Interfaces;
using ToDoListApp.DAL;
using ToDoListApp.Validators;
using FluentValidation;
using ToDoListApp.Contracts.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITaskService, TaskService>();
builder.Services.AddTransient<DataSeeder>();
builder.Services.AddScoped<UserAccessManager>();
builder.Services.AddScoped<IValidator<RegistrationRequestModel>, RegistrationRequestModelValidator>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoListApp", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var conn = builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
builder.Services.AddDbContext<UsersDbContext>(options =>
    options.UseNpgsql(conn));

// For Identity  
builder.Services
    .AddIdentity<User, UserRoles>()
    .AddEntityFrameworkStores<UsersDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication  
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWTKey:ValidAudience"],
        ValidIssuer = builder.Configuration["JWTKey:ValidIssuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey:Secret"])),
        RequireExpirationTime = true
    };
});

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

////////////////////////////
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
await using var scope = scopeFactory.CreateAsyncScope();
var seedManager = scope.ServiceProvider.GetRequiredService<DataSeeder>();

await seedManager.SeedDataAsync();
////////////////////////////

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

