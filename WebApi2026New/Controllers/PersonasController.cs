using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi2026New.Data;
using WebApi2026New.DTOs;
using WebApi2026New.Models;

namespace WebApi2026New.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // todos los endpoints de este controlador exigen token
    public class PersonasController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PersonasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/Personas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPersonas()
        {
            return await _context.Personas.Select(p => new
            {
                idPersona = p.idPersona,
                nombre = p.nombre,
                apellido = p.apellido,
                correo = p.correo,
                edad = p.edad,
                totalDirecciones = p.Direcciones.Count
            }).ToListAsync();
        }

        // GET: api/Personas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPersona(int id)
        {
            var persona = await _context.Personas
                .Where(p => p.idPersona == id)
                .Select(p => new
                {
                    idPersona = p.idPersona,
                    nombre = p.nombre,
                    apellido = p.apellido,
                    correo = p.correo,
                    edad = p.edad,
                    direcciones = p.Direcciones.Select(d => new
                    {
                        idDireccion = d.idDireccion,
                        calle = d.calle,
                        ciudad = d.ciudad,
                        departamento = d.departamento,
                        codigopostal = d.codigoPostal,
                        pais = d.pais
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (persona == null)
            {
                return NotFound();
            }

            return persona;
        }

        // PUT: api/Personas/5 -> recibe un DTO, no la entidad (así no se sobrescribe el password)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersona(int id, PersonaUpdateDTO dto)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
                return NotFound();

            persona.nombre = dto.nombre;
            persona.apellido = dto.apellido;
            persona.edad = dto.edad;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Personas
        [HttpPost]
        public async Task<IActionResult> PostPersona(RegistroDTO dto)
        {
            var persona = new Persona
            {
                nombre = dto.nombre,
                apellido = dto.apellido,
                correo = dto.correo,
                edad = dto.edad,
                password = BCrypt.Net.BCrypt.HashPassword(dto.password) // Hash seguro
            };

            _context.Personas.Add(persona);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                idPersona = persona.idPersona,
                nombre = persona.nombre,
                apellido = persona.apellido,
                correo = persona.correo,
                edad = persona.edad
            });
        }

        // DELETE: api/Personas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersona(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }

            _context.Personas.Remove(persona);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}