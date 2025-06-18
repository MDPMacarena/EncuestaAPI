using EncuestaAPI.Models.DTOs;
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
            Repository = repository;
        }
        public IConfiguration Configuration { get; }
        public Repository<Usuario> Repository { get; }
        public string? GenerateToken(UsuarioDTO dto)
        {
            var usuario = Repository.GetAll().FirstOrDefault(x => x.NumControl == dto.NumControl && x.Contraseña == dto.Contraseña);
            if (usuario == null)
            {
                Console.WriteLine("No se encontró usuario con esas credenciales.");
                return null; // Usuario no encontrado o credenciales incorrectas  
            }
            else
            {
                List<Claim> claims = new List<Claim>()
                   {
                       new Claim("Id", usuario.Id.ToString()),
                       new Claim(ClaimTypes.Name, usuario.Nombre),
                       new Claim("NumControl", usuario.NumControl)
                   };
                var descriptor = new JwtSecurityToken(
                   issuer: Configuration["Jwt:Issuer"],
                   audience: Configuration["Jwt:Audience"],
                   claims: claims,

                   expires: DateTime.UtcNow.AddMinutes(15),
                   signingCredentials: new SigningCredentials(new SymmetricSecurityKey(
                       System.Text.Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
                   );
                var handler = new JwtSecurityTokenHandler();
                return handler.WriteToken(descriptor); // Genera el token JWT  
            }
        }
    }
}
