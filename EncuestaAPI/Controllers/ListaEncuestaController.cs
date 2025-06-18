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
                if (string.IsNullOrWhiteSpace(dto.numeroControl))
                    return BadRequest("El número de control es requerido");
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
                foreach (var pregunta in dto.Preguntas)
                {
                    if (pregunta == null || string.IsNullOrWhiteSpace(dto.Nombre))
                    {
                        return BadRequest("Cada pregunta debe tener un texto válido");
                    }
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
                });
            }
        }
        [HttpPut("{id}")]
        public IActionResult Put(int id, ListaEncuestaDTO drto)
        {
            try
            {
                if (string.IsNullOrEmpty(drto.Nombre))
                {
                    return BadRequest("El Nombre es requerido.");
                }
                var encuestaExistente = EncuestaRepository.Get(drto.Id);
                if (encuestaExistente == null)
                {
                    return NotFound("No se encontro la encuesta.");
                }
                var conPreguntas = AplicacionRepository.GetAll().Any(a => a.IdEncuesta == drto.Id && RespuestaRepository.GetAll()
                                       .Any(r => r.IdAplicacion == a.Id));
                if (conPreguntas)
                {
                    return BadRequest(new
                    {
                        message = "No se puede editar una encuesta que ya ha sido aplicada."
                    });
                }
                var validationResult = Validator.Validate(new Encuesta
                {
                    Nombre = drto.Nombre ?? ""
                });

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
                }

                encuestaExistente.Nombre = drto.Nombre;
                EncuestaRepository.Update(encuestaExistente);

                var preguExistentes = PreguntaRepository.GetAll().Where(p => p.IdEncuesta == id).ToList();
                preguExistentes.ForEach(p => PreguntaRepository.Delete(p.Id));

                int orden = 1;
                foreach (var textoPregunta in drto.Preguntas ?? Enumerable.Empty<string>())
                {
                    if (!string.IsNullOrWhiteSpace(textoPregunta))
                    {
                        PreguntaRepository.Insert(new Pregunta
                        {
                            Texto = textoPregunta.Trim(),
                            IdEncuesta = id,
                            Orden = orden++
                        });
                    }
                }
                return Ok(new { message = "Encuesta actualizada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error interno al procesar la solicitud",
                    detalle = ex.Message
                });
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var aplicaciones = AplicacionRepository.GetAll().Where(e => e.IdEncuesta == id).ToList();
            var borrarEncuesta = EncuestaRepository.Get(id);
            if (borrarEncuesta == null)
            {
                return NotFound("La encuesta que busca no se encuentra ^^");
            }
            foreach (var a in aplicaciones)
            {
                var conPreguntas = RespuestaRepository.GetAll().Any(c => c.IdAplicacion == a.Id);
                if (conPreguntas)
                {
                    return BadRequest(new
                    {
                        message = "No se puede eliminar una encuesta que ya ha sido aplicada."
                    });
                }
            }
            var preguntas = PreguntaRepository.GetAll().Where(p => p.IdEncuesta == id).ToList();
            foreach (var pregunta in preguntas)
            {
                PreguntaRepository.Delete(pregunta.Id);
            }

            foreach (var apli in aplicaciones)
            {
                AplicacionRepository.Delete(apli.Id);
            }
            EncuestaRepository.Delete(id);
            return Ok(new { message = "Encuesta eliminada correctamente!" });
        }
        [HttpPost("NotificarRespuesta")]
        public async Task<IActionResult> NotificarRespuesta([FromBody] int encuestaId)
        {
            try
            {
                var aplicacionesAll = AplicacionRepository.GetAll() ?? new List<Aplicacionencuesta>();
                var respuestasAll = RespuestaRepository.GetAll() ?? new List<Respuesta>();

                var aplicaciones = aplicacionesAll.Where(a => a.IdEncuesta == encuestaId).Select(a => a.Id).ToList();

                var respuestas = respuestasAll.Where(r => aplicaciones.Contains(r.IdAplicacion)).ToList();

                int totalRespuestas = respuestas.Count;
                int totalAlumnos = respuestas.Where(r => !string.IsNullOrEmpty(r.NumControlAlumno)).Select(r => r.NumControlAlumno).Distinct().Count();

                // notisss 
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
        [HttpGet("UltimasRespuestas")]
        public IActionResult UltimasRespuestas()
        {
            var respuestas = RespuestaRepository.GetAll().OrderByDescending(r => r.Fecha).Take(30).Select(r => new
            {
                nombreEncuesta = r.IdAplicacionNavigation != null && r.IdAplicacionNavigation.IdEncuestaNavigation != null
                    ? r.IdAplicacionNavigation.IdEncuestaNavigation.Nombre : "Sin nombre",
                nombreAlumno = r.NombreAlumno ?? "Desconocido",
                numeroControl = r.NumControlAlumno ?? "Desconocido",
                fecha = r.Fecha
            }).ToList();

            return Ok(respuestas);
        }
        [HttpGet("Estadisticas")]
        public IActionResult Estadisticas()
        {
            var totalEncuestas = EncuestaRepository.GetAll().Count();
            var totalAlumnos = RespuestaRepository.GetAll().Select(r => r.NumControlAlumno).Distinct().Count();
            var totalRespuestas = RespuestaRepository.GetAll().Count();

            return Ok(new
            {
                totalEncuestas,
                totalAlumnos,
                totalRespuestas
            });
        }
    }
}
