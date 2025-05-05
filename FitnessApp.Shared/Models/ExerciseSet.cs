using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FitnessApp.Shared.Models
{
    public class ExerciseSet
    {
        public int Id { get; set; }

        [Required]
        public int SetNumber { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Reps must be between 0 and 1000")]
        public int Reps { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Weight must be between 0 and 10000")]
        public decimal Weight { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        [JsonIgnore]
        [ForeignKey("ExerciseId")]
        public Exercise Exercise { get; set; }
    }
}
