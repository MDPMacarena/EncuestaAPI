using EncuestaAPI.Models;
using EncuestaAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EncuestaAPI.Services
{
    public class JwtService
    {
        public JwtService(IConfiguration configuration, Repository<Usuario> repository)
        {
            Configuration = configuration;
            Repositoryy = repository;
        }
        public IConfiguration Configuration { get; }
        public Repository<Usuario> Repositoryy { get; }
        public string? GenerateToken(Usuario dto)
        {
            var usuario = Repositoryy.GetAll()
                 .FirstOrDefault(u => u.NumControl == dto.NumControl && u.Contrsena == dto.Contrsena);
            if (usuario == null)
            {
                return null; // Usuario no encontrado o credenciales incorrectas
            }
            else
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim("Id",usuario.Id.ToString()),
                    new Claim("Nombre",usuario.Nombre),
                    new Claim("NumControl",usuario.NumControl)
                };
                var descriptor = new JwtSecurityToken(
                   issuer: Configuration["Jwt:Issuer"],
                   audience: Configuration["Jwt:Audience"],
                   claims: claims,

                   expires: DateTime.UtcNow.AddMinutes(5),
                   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                       System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
                   );
                var handler = new JwtSecurityTokenHandler();
                return handler.WriteToken(descriptor); // Genera el token JWT
            }
        }
    }
}
