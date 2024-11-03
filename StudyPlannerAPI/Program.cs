using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.StudyMaterialServices;
using StudyPlannerAPI.Services.StudyPlanServices;
using StudyPlannerAPI.Services.StudySessionsServices;
using StudyPlannerAPI.Services.UserServices;
using StudyPlannerAPI.Validators;
using StudyPlannerAPI.Validators.StudyMaterialValidators;
using StudyPlannerAPI.Validators.StudyPlanValidators;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowProductionOrigins",
        builder =>
        {
            builder.WithOrigins("#") //TODO: Add once the frontend is deployed
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });

    options.AddPolicy("AllowLocalhost",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Your frontend URL
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); 
        });
});


builder.Services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnString")));

builder.Services.AddAutoMapper(typeof(Program));

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudyPlanService, StudyPlanService>();
builder.Services.AddScoped<IStudyTopicService, StudyTopicService>();
builder.Services.AddScoped<IStudySessionService, StudySessionService>();
builder.Services.AddScoped<IStudyMaterialService, StudyMaterialService>();

// Validators
builder.Services.AddScoped<IValidator<UserRegistrationDTO>, UserRegistrationValidator>();
builder.Services.AddScoped<IValidator<UserLoginDTO>, UserLoginValidator>();
builder.Services.AddScoped<IValidator<UserUpdateDTO>, UserUpdateValidator>();
builder.Services.AddScoped<IValidator<StudyPlanDTO>, StudyPlanValidator>();
builder.Services.AddScoped<IValidator<StudyTopicDTO>, StudyTopicValidator>();
builder.Services.AddScoped<IValidator<StudySessionDTO>, StudySessionValidator>();
builder.Services.AddScoped<IValidator<StudyMaterialDTO>, StudyMaterialDTOValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    // Set cookies for Google OAuth sign-in
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultScheme = "Cookies";
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    // Custom token retrieval from cookies
    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Retrieve the token from the HttpOnly cookie
            var token = context.Request.Cookies["accessToken"];

            // Set the token in the context if it exists
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            return Task.CompletedTask;
        }
    };
})
.AddCookie("Cookies")
.AddGoogle("Google", options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowLocalhost");
} else
{
    app.UseCors("AllowProductionOrigins");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
