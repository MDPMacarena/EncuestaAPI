using EncuestaAPI.Models;
using EncuestaAPI.Models.DTOs;
using EncuestaAPI.Repositories;
using EncuestaAPI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EncuestaAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ListaEncuestaController : ControllerBase
    {
        private readonly Repository<Encuesta> EncuestaRepository;
        private readonly Repository<Pregunta> PreguntaRepository;
        private readonly Repository<Respuesta> RespuestaRepository;
        private readonly Repository<Detallerespuesta> DetallesRespuestaRepository;
        private readonly Repository<Aplicacionencuesta> AplicacionRepository;
        private readonly IValidator<Encuesta> Validator;
        private readonly IHubContext<listaEncuestahub> _hubContext;
        public ListaEncuestaController(
            Repository<Encuesta> encuestaRepository,
            Repository<Pregunta> preguntaRepository,
            Repository<Respuesta> respuestaRepository,
            Repository<Detallerespuesta> detallesRespuestaRepository,
            Repository<Aplicacionencuesta> aplicacionRepository,
            IValidator<Encuesta> validator,
             IHubContext<listaEncuestahub> hubContext)
        {
            this.EncuestaRepository = encuestaRepository;
            this.PreguntaRepository = preguntaRepository;
            this.RespuestaRepository = respuestaRepository;
            this.DetallesRespuestaRepository = detallesRespuestaRepository;
            this.AplicacionRepository = aplicacionRepository;
            this.Validator = validator;
            _hubContext=hubContext;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var id = int.Parse(User.FindFirst("Id")?.Value ?? "0");
                var encuestas = EncuestaRepository.GetAll().Where(e => e.IdUsuario == id).OrderBy(x => x.FechaCreacion).Select(e => new
                {
                    id = e.Id,
                    nombre = e.Nombre,
                    fecha = e.FechaCreacion
                }).ToList();

                return Ok(encuestas);
            }
            catch (Exception excep)
            {
                return StatusCode(500, new
                {
                    error = excep.Message
                });
            }
        }
        [HttpGet("AplicacionActiva")]
        public IActionResult GetAplicacionActiva(int encuestaId)
        {
            try
            {
                var aplicacion = AplicacionRepository.GetAll().FirstOrDefault(a => a.IdEncuesta == encuestaId && a.FechaFin >= DateTime.Now);
                if (aplicacion == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No hay una aplicación activa para esta encuesta"
                    });
                }
                return Ok(new
                {
                    success = true,
                    id = aplicacion.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        [HttpGet("Preguntas")]
        public IActionResult GetPreguntas(int encuestaId)
        {
            try
            {
                var preguntas = PreguntaRepository.GetAll().Where(p => p.IdEncuesta == encuestaId).OrderBy(p => p.Orden).Select(p => new
                {
                    id = p.Id,
                    texto = p.Texto,
                    orden = p.Orden
                }).ToList();

                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("{id}")]
        public IActionResult GetEncuesta(int id)
        {
            var encuesta = EncuestaRepository.Get(id);
            if (encuesta == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                Id = id,
                Nombre = encuesta.Nombre,
                fechaCreacion = encuesta.FechaCreacion
            });

        }
        [HttpPost("GuardarRespuesta")]
        public IActionResult GuardarRespuesta([FromBody] RespuestaDTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.nombreRespondedor))
                    return BadRequest("El nombre es requerido");
                var aplicacion = AplicacionRepository.GetAll()
                    .FirstOrDefault(a => a.IdEncuesta == dto.encuestaId && a.FechaFin >= DateTime.UtcNow)
                    ?? new Aplicacionencuesta
                    {
                        IdEncuesta = dto.encuestaId,
                        IdUsuario = 1, // O usa el ID del usuario actual
                        FechaInicio = DateTime.UtcNow,
                        FechaFin = DateTime.UtcNow.AddYears(1)
                    };

                if (aplicacion.Id == 0)
                    AplicacionRepository.Insert(aplicacion);
                var yaRespondio = RespuestaRepository.GetAll()
                    .Any(r => r.IdAplicacion == aplicacion.Id && r.NumControlAlumno == dto.numeroControl);

                if (yaRespondio)
                {
                    return BadRequest("Este alumno ya ha respondido esta encuesta.");
                }
                var respuesta = new Respuesta
                {
                    IdAplicacion = aplicacion.Id,
                    NombreAlumno = dto.nombreRespondedor,
                    NumControlAlumno = dto.numeroControl,
                    Fecha = DateTime.UtcNow
                };
                RespuestaRepository.Insert(respuesta);

                foreach (var r in dto.respuestas)
                {
                    DetallesRespuestaRepository.Insert(new Detallerespuesta
                    {
                        IdRespuesta = respuesta.Id,
                        IdPregunta = r.preguntaId,
                        Valor = r.valor
                    });
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Post([FromBody] ListaEncuestaDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("El cuerpo de la solicitud no puede estar vacío");
                }
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                {
                    return BadRequest("El nombre de la encuesta es requerido");
                }
                if (dto.Preguntas == null || !dto.Preguntas.Any())
                {
                    return BadRequest("Debe incluir al menos una pregunta");
                }
                var validationResult = Validator.Validate(new Encuesta { Nombre = dto.Nombre });
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }
                var idUsuario = int.Parse(User.FindFirst("Id")?.Value ?? "0");
                if (idUsuario == 0)
                {
                    return Unauthorized("Usuario no autenticado");
                }
                var nuevaEncuesta = new Encuesta
                {
                    Nombre = dto.Nombre,
                    IdUsuario = idUsuario,
                    FechaCreacion = DateTime.UtcNow
                };
                EncuestaRepository.Insert(nuevaEncuesta);

                int orden = 1;
                foreach (var pregunta in dto.Preguntas.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    PreguntaRepository.Insert(new Pregunta
                    {
                        Texto = pregunta.Trim(),
                        IdEncuesta = nuevaEncuesta.Id,
                        Orden = orden++
                    });
                }
                return Ok(new { Id = nuevaEncuesta.Id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return StatusCode(500, new
                {
                    Message = "Error interno del servidor",
                    //Detail = ex.Message 
                });
            }
        }
        [HttpPut("{id}")]
        public IActionResult Put(ListaEncuestaDTO drto)
        {
            var encuestaExistente = EncuestaRepository.Get(drto.Id);
            if (encuestaExistente == null)
            {
                return NotFound("No se encontró la encuesta.");
            }

            // ✅ Validar si ya fue aplicada
            var yaAplicada = AplicacionRepository.GetAll().Any(a => a.IdEncuesta == drto.Id);
            if (yaAplicada)
            {
                return BadRequest("No se puede editar una encuesta que ya ha sido aplicada.");
            }

            var validationResult = Validator.Validate(new Encuesta
            {
                Nombre = drto.Nombre ?? ""
            });

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            if (drto.Id == null || string.IsNullOrEmpty(drto.Nombre))
            {
                return BadRequest("El ID y el Nombre son requeridos.");
            }

            encuestaExistente.Nombre = drto.Nombre;
            EncuestaRepository.Update(encuestaExistente);
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // ✅ Validar si ya fue aplicada
            var yaAplicada = AplicacionRepository.GetAll().Any(a => a.IdEncuesta == id);
            if (yaAplicada)
            {
                return BadRequest("No se puede eliminar una encuesta que ya ha sido aplicada.");
            }

            var preguntas = PreguntaRepository.GetAll().Where(p => p.IdEncuesta == id).ToList();
            foreach (var pregunta in preguntas)
            {
                PreguntaRepository.Delete(pregunta.Id);
            }

            var aplicaciones = AplicacionRepository.GetAll().Where(e => e.IdEncuesta == id).ToList();
            foreach (var apli in aplicaciones)
            {
                AplicacionRepository.Delete(apli.Id);
            }

            EncuestaRepository.Delete(id);
            return Ok();
        }
        [HttpPost("NotificarRespuesta")]
        public async Task<IActionResult> NotificarRespuesta([FromBody] int encuestaId)
        {
            try
            {
                // Contar respuestas totales para la encuesta  
                var aplicaciones = AplicacionRepository.GetAll()
                    .Where(a => a.IdEncuesta == encuestaId)
                    .Select(a => a.Id)
                    .ToList();

                var respuestas = RespuestaRepository.GetAll()
                    .Where(r => aplicaciones.Contains(r.IdAplicacion))
                    .ToList();

                int totalRespuestas = respuestas.Count; // Fix: Define and calculate totalRespuestas  
                int totalAlumnos = respuestas.Select(r => r.NumControlAlumno).Distinct().Count(); // Fix: Define and calculate totalAlumnos  

                // Enviar notificación a todos los clientes conectados  
                await _hubContext.Clients.All.SendAsync("ActualizarRespuestas", new
                {
                    encuestaId = encuestaId,
                    totalRespuestas = totalRespuestas,
                    totalAlumnos = totalAlumnos
                });

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}
