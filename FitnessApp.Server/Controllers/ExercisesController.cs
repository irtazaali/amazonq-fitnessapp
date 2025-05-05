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
    public class ExercisesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExercisesController> _logger;

        public ExercisesController(ApplicationDbContext context, ILogger<ExercisesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Exercise>>> GetExercises()
        {
            try
            {
                _logger.LogInformation("Retrieving all exercises");
                return await _context.Exercises
                    .Include(e => e.Sets)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercises");
                return StatusCode(500, "Internal server error while retrieving exercises");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Exercise>> GetExercise(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving exercise with ID: {Id}", id);
                var exercise = await _context.Exercises
                    .Include(e => e.Sets)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (exercise == null)
                {
                    _logger.LogWarning("Exercise with ID: {Id} not found", id);
                    return NotFound();
                }

                return exercise;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise with ID: {Id}", id);
                return StatusCode(500, "Internal server error while retrieving exercise");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Exercise>> PostExercise(Exercise exercise)
        {
            try
            {
                _logger.LogInformation("Creating new exercise");
                
                if (exercise.CreatedAt == default)
                {
                    exercise.CreatedAt = DateTime.Now;
                }

                _context.Exercises.Add(exercise);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created exercise with ID: {Id}", exercise.Id);
                return CreatedAtAction(nameof(GetExercise), new { id = exercise.Id }, exercise);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise");
                return StatusCode(500, "Internal server error while creating exercise");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExercise(int id, Exercise exercise)
        {
            if (id != exercise.Id)
            {
                _logger.LogWarning("Exercise update failed: ID mismatch. Route ID: {RouteId}, Entity ID: {EntityId}", id, exercise.Id);
                return BadRequest("ID mismatch");
            }

            try
            {
                _logger.LogInformation("Updating exercise with ID: {Id}", id);
                _context.Entry(exercise).State = EntityState.Modified;
                
                // Handle sets separately to avoid tracking issues
                var existingSets = await _context.ExerciseSets.Where(s => s.ExerciseId == id).ToListAsync();
                var setsToDelete = existingSets.Where(s => !exercise.Sets.Any(es => es.Id == s.Id)).ToList();
                
                foreach (var set in setsToDelete)
                {
                    _context.ExerciseSets.Remove(set);
                }

                foreach (var set in exercise.Sets)
                {
                    if (set.Id == 0)
                    {
                        // New set
                        set.ExerciseId = id;
                        _context.ExerciseSets.Add(set);
                    }
                    else
                    {
                        // Existing set
                        _context.Entry(set).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated exercise with ID: {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ExerciseExists(id))
                {
                    _logger.LogWarning("Exercise update failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating exercise with ID: {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise with ID: {Id}", id);
                return StatusCode(500, "Internal server error while updating exercise");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExercise(int id)
        {
            try
            {
                _logger.LogInformation("Deleting exercise with ID: {Id}", id);
                var exercise = await _context.Exercises.FindAsync(id);
                if (exercise == null)
                {
                    _logger.LogWarning("Exercise delete failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }

                _context.Exercises.Remove(exercise);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted exercise with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise with ID: {Id}", id);
                return StatusCode(500, "Internal server error while deleting exercise");
            }
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercises.Any(e => e.Id == id);
        }
    }
}
