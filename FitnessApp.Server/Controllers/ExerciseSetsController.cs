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
    public class ExerciseSetsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ExerciseSetsController> _logger;

        public ExerciseSetsController(ApplicationDbContext context, ILogger<ExerciseSetsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseSet>>> GetExerciseSets()
        {
            try
            {
                _logger.LogInformation("Retrieving all exercise sets");
                return await _context.ExerciseSets.Include(s => s.Exercise).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise sets");
                return StatusCode(500, "Internal server error while retrieving exercise sets");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseSet>> GetExerciseSet(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving exercise set with ID: {Id}", id);
                var exerciseSet = await _context.ExerciseSets
                    .Include(s => s.Exercise)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (exerciseSet == null)
                {
                    _logger.LogWarning("Exercise set with ID: {Id} not found", id);
                    return NotFound();
                }

                return exerciseSet;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise set with ID: {Id}", id);
                return StatusCode(500, "Internal server error while retrieving exercise set");
            }
        }

        [HttpGet("byExercise/{exerciseId}")]
        public async Task<ActionResult<IEnumerable<ExerciseSet>>> GetExerciseSetsByExercise(int exerciseId)
        {
            try
            {
                _logger.LogInformation("Retrieving exercise sets for exercise ID: {ExerciseId}", exerciseId);
                var sets = await _context.ExerciseSets
                    .Where(s => s.ExerciseId == exerciseId)
                    .OrderBy(s => s.SetNumber)
                    .ToListAsync();

                return sets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exercise sets for exercise ID: {ExerciseId}", exerciseId);
                return StatusCode(500, "Internal server error while retrieving exercise sets");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ExerciseSet>> PostExerciseSet(ExerciseSet exerciseSet)
        {
            try
            {
                _logger.LogInformation("Creating new exercise set for exercise ID: {ExerciseId}", exerciseSet.ExerciseId);
                
                // Verify the exercise exists
                var exercise = await _context.Exercises.FindAsync(exerciseSet.ExerciseId);
                if (exercise == null)
                {
                    _logger.LogWarning("Cannot create exercise set: Exercise with ID {ExerciseId} not found", exerciseSet.ExerciseId);
                    return BadRequest($"Exercise with ID {exerciseSet.ExerciseId} does not exist");
                }

                _context.ExerciseSets.Add(exerciseSet);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created exercise set with ID: {Id}", exerciseSet.Id);
                return CreatedAtAction(nameof(GetExerciseSet), new { id = exerciseSet.Id }, exerciseSet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exercise set");
                return StatusCode(500, "Internal server error while creating exercise set");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutExerciseSet(int id, ExerciseSet exerciseSet)
        {
            if (id != exerciseSet.Id)
            {
                _logger.LogWarning("Exercise set update failed: ID mismatch. Route ID: {RouteId}, Entity ID: {EntityId}", id, exerciseSet.Id);
                return BadRequest("ID mismatch");
            }

            try
            {
                _logger.LogInformation("Updating exercise set with ID: {Id}", id);
                _context.Entry(exerciseSet).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated exercise set with ID: {Id}", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ExerciseSetExists(id))
                {
                    _logger.LogWarning("Exercise set update failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating exercise set with ID: {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating exercise set with ID: {Id}", id);
                return StatusCode(500, "Internal server error while updating exercise set");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExerciseSet(int id)
        {
            try
            {
                _logger.LogInformation("Deleting exercise set with ID: {Id}", id);
                var exerciseSet = await _context.ExerciseSets.FindAsync(id);
                if (exerciseSet == null)
                {
                    _logger.LogWarning("Exercise set delete failed: Record with ID: {Id} not found", id);
                    return NotFound();
                }

                _context.ExerciseSets.Remove(exerciseSet);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted exercise set with ID: {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting exercise set with ID: {Id}", id);
                return StatusCode(500, "Internal server error while deleting exercise set");
            }
        }

        private bool ExerciseSetExists(int id)
        {
            return _context.ExerciseSets.Any(e => e.Id == id);
        }
    }
}
