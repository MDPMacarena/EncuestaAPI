using EncuestaAPI.Repositories;
using EncuestaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using EncuestaAPI.Models.DTOs;

namespace EncuestaAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ListaEncuestaControllers : ControllerBase
    {
        private readonly Repository<ListaEncuesta> repository;
        private readonly Repository<ListaPregunta> preguntaRepository;
        private readonly Repository<ListaEncuestados> EncuestadoRepository;
        private readonly IValidator<ListaEncuesta> validator;
        public ListaEncuestaControllers(
            Repository<ListaEncuesta> repository,
            Repository<ListaPregunta> preguntaRepository,
            Repository<ListaEncuestados> EncuestadoRepository,
            IValidator<ListaEncuesta> validator)
        {
            this.repository = repository;
            this.preguntaRepository = preguntaRepository;
            this.EncuestadoRepository = EncuestadoRepository;
            this.validator = validator;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var id = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            var lista = repository.GetAll()
                .Where(x => x.IdUsuario == id).Select(x => new
                {
                    x.Id,
                    x.Nombre,
                });
            var encuestas = EncuestadoRepository.GetAll()
                .Where(x => x.IdUsuarios == id)
                .Select(x => new
                {
                    x.Id,
                    x.IdEncuestasNavigation.Nombre,
                    x.IdEncuestasNavigation.FechaCreacion,
                    x.IdEncuestasNavigation.Activo
                });
            return Ok(new
            {
                encuestas = encuestas,
                Pregunta = lista
            });
        }
        [HttpPost]
        public IActionResult Post(ListaEncuestaDTO dto)
        {
            var validationResult = validator.Validate(new ListaEncuesta
            {
                Nombre = dto.Nombre ?? ""
            });
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }
            var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");
            var nuevaEncuesta = new ListaEncuesta
            {
                Nombre = dto.Nombre ?? "",
                IdUsuario = idUsuario,
                FechaCreacion = DateTime.UtcNow,
                Activo = BitConverter.GetBytes(true)
            };
            repository.Insert(nuevaEncuesta);
            return Ok();
        }
        [HttpPut]
        public IActionResult Put(ListaEncuestaDTO drto)
        {
            if (drto.Id == null || string.IsNullOrEmpty(drto.Nombre))
            {
                return BadRequest("El ID y el Nombre son requeridos.");
            }

            var encuestaExistente = repository.Get(drto.Id.Value);
            if (encuestaExistente == null)
            {
                return NotFound("La encuesta no existe.");
            }

            var validationResult = validator.Validate(new ListaEncuesta
            {
                Nombre = drto.Nombre
            });

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }
            encuestaExistente.Nombre = drto.Nombre;
            repository.Update(encuestaExistente);
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var claimId = User.FindFirst("Id")?.Value;
            if (string.IsNullOrEmpty(claimId) || !int.TryParse(claimId, out var idUsuario))
            {
               return Unauthorized("No tienes permiso para realizar esta acción.");
            }
            var lista = repository.GetAll().FirstOrDefault(x=> x.Id == id && x.IdUsuario == idUsuario);
            if (lista == null)
            {
                return NotFound("La encuesta no existe o no tienes permiso para eliminarla.");
            }
            foreach(var usuario in EncuestadoRepository.GetAll().Where(e=> e.IdEncuestas == id).ToList())
            {
                EncuestadoRepository.Delete(usuario.Id);
            }
            repository.Delete(id);
            return Ok();
        }
        
    }
}
