using EncuestaAPI.Models.Validators;
using EncuestaAPI.Models;
using EncuestaAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EncuestaAPI.Services;
using EncuestaAPI.Models.DTOs;

namespace EncuestaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        public UsuariosController(Repository<Usuario> repository, UsuarioValidator validador, JwtService service)
        {
            Repositoryy = repository;
            Validador = validador;
            Service = service;
        }
        public Repository<Usuario> Repositoryy { get; }
        public UsuarioValidator Validador { get; }
        public JwtService Service { get; }
        [HttpPost("login")]
        public IActionResult Login(UsuarioDTO dto)
        {
            var token = Service.GenerateToken(dto);
            if (token == null)
            {
                return Unauthorized("Credenciales incorrectas");
            }
            return Ok(token);
        }
    }
}
