using FitnessApp.Server.Data;
using FitnessApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FitnessApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VitalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VitalsController> _logger;

        public VitalsController(ApplicationDbContext context, ILogger<VitalsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vitals>>> GetVitals()
        {
            try
            {
                _logger.LogInformation("Retrieving all vitals records");
                return await _context.Vitals.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vitals records");
                return StatusCode(500, "Internal server error while retrieving vitals records");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vitals>> GetVitals(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving vitals record with ID: {Id}", id);
                var vitals = await _context.Vitals.FindAsync(id);

                if (vitals == null)
                {
                    _logger.LogWarning("Vitals record with ID: {Id} not found", id);
                    return NotFound();
                }

                return vitals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vitals record with ID: {Id}", id);
                return StatusCode(500, "Internal server error while retrieving vitals record");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Vitals>> PostVitals(Vitals vitals)
        {
            try
            {
                _logger.LogInformation("Creating new vitals record");
                
                if (vitals.Timestamp == default)
                {
                    vitals.Timestamp = DateTime.Now;
                }

                _context.Vitals.Add(vitals);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created vitals record with ID: {Id}", vitals.Id);
                return CreatedAtAction(nameof(GetVitals), new { id = vitals.Id }, vitals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vitals record");
                return StatusCode(500, "Internal server error while creating vitals record");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVitals(int id, Vitals vitals)
        {
            if (id != vitals.Id)
            {
                _logger.LogWarning("Vitals update failed: ID mismatch. Route ID: {RouteId}, Entity ID: {EntityId}", id, vitals.Id);
                return BadRequest("ID mismatch");
            }

            try
            {
                _logger.LogInformation("Updating vitals record with ID: {Id}", id);
                _context.Entry(vitals).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated vitals record with ID: {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VitalsExists(id))
                {
                    _logger.LogWarning("Vitals update failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating vitals record with ID: {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating vitals record with ID: {Id}", id);
                return StatusCode(500, "Internal server error while updating vitals record");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVitals(int id)
        {
            try
            {
                _logger.LogInformation("Deleting vitals record with ID: {Id}", id);
                var vitals = await _context.Vitals.FindAsync(id);
                if (vitals == null)
                {
                    _logger.LogWarning("Vitals delete failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }

                _context.Vitals.Remove(vitals);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted vitals record with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vitals record with ID: {Id}", id);
                return StatusCode(500, "Internal server error while deleting vitals record");
            }
        }

        private bool VitalsExists(int id)
        {
            return _context.Vitals.Any(e => e.Id == id);
        }
    }
}
