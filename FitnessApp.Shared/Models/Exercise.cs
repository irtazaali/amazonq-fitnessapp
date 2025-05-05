using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Shared.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Exercise name cannot exceed 100 characters")]
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<ExerciseSet> Sets { get; set; } = new List<ExerciseSet>();
    }
}
