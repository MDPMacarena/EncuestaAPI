using EncuestaAPI.Models;
using EncuestaAPI.Models.Validators;
using EncuestaAPI.Repositories;
using EncuestaAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*movido, daba error al correr*/
builder.Services.AddCors(x =>
{
    x.AddPolicy("todos", builder =>
    {
        builder.AllowCredentials().AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:7233");
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtOptions =>
{
    jwtOptions.Audience = builder.Configuration["Jwt:Audience"];
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateLifetime = true
    };
});

var cs = builder.Configuration.GetConnectionString("EncuestasCS");
builder.Services.AddDbContext<EncuastasContext>(x => x.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));
builder.Services.AddTransient<JwtService>();

builder.Services.AddScoped(typeof(UsuarioValidator));
builder.Services.AddScoped<IValidator<Encuesta>, ListaEncuestaValidator>();

builder.Services.AddControllers();
//sngalR
builder.Services.AddSignalR();
var app = builder.Build();
app.MapHub<listaEncuestahub>("/listaEncuestahub");

//builder.Services.AddScoped(typeof(Repository<>), typeof(Repository<>));
app.UseRouting();
app.UseCors("todos");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseFileServer();
app.Run();
