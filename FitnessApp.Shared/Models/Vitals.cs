using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Shared.Models
{
    public class Vitals
    {
        public int Id { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Blood sugar level must be between 0 and 1000")]
        public decimal BloodSugarLevel { get; set; }

        [Required]
        [StringLength(10)]
        public string BloodSugarUnit { get; set; } = "mg/dL"; // Default unit

        [Required]
        [Range(50, 250, ErrorMessage = "Systolic pressure must be between 50 and 250")]
        public int SystolicPressure { get; set; }

        [Required]
        [Range(30, 150, ErrorMessage = "Diastolic pressure must be between 30 and 150")]
        public int DiastolicPressure { get; set; }
    }
}
