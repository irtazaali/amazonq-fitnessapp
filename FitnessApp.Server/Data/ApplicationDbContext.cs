using FitnessApp.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vitals> Vitals { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseSet> ExerciseSets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<ExerciseSet>()
                .HasOne(es => es.Exercise)
                .WithMany(e => e.Sets)
                .HasForeignKey(es => es.ExerciseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Vitals
            modelBuilder.Entity<Vitals>().HasData(
                new Vitals
                {
                    Id = 1,
                    Timestamp = new DateTime(2025, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                    BloodSugarLevel = 95,
                    BloodSugarUnit = "mg/dL",
                    SystolicPressure = 120,
                    DiastolicPressure = 80
                },
                new Vitals
                {
                    Id = 2,
                    Timestamp = new DateTime(2025, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                    BloodSugarLevel = 5.2m,
                    BloodSugarUnit = "mmol/L",
                    SystolicPressure = 118,
                    DiastolicPressure = 75
                }
            );

            // Seed Exercises
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Name = "Bench Press",
                    CreatedAt = new DateTime(2025, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Squat",
                    CreatedAt =new DateTime(2025, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                }
            );

            // Seed Exercise Sets
            modelBuilder.Entity<ExerciseSet>().HasData(
                new ExerciseSet
                {
                    Id = 1,
                    ExerciseId = 1,
                    SetNumber = 1,
                    Reps = 10,
                    Weight = 135
                },
                new ExerciseSet
                {
                    Id = 2,
                    ExerciseId = 1,
                    SetNumber = 2,
                    Reps = 8,
                    Weight = 155
                },
                new ExerciseSet
                {
                    Id = 3,
                    ExerciseId = 2,
                    SetNumber = 1,
                    Reps = 12,
                    Weight = 185
                }
            );
        }
    }
}
