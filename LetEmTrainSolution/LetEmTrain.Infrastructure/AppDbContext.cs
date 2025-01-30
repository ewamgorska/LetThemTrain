using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using LetEmTrain.Domain.Models;
using System.Collections.Generic;
using LetEmTrain.Domain.SeedWork;

namespace LetEmTrain.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<ExerciseSet> ExerciseSets { get; set; }
        public DbSet<Progress> ProgressRecords { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }

        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Combine(path, "LetEmTrain.db");
        }

        //LOCAL DB PATH
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");


        /*  CLOUD
         *  protected override void OnConfiguring(DbContextOptionsBuilder options)
              => options.UseSqlServer("Server=tcp:letthemtrainserver.database.windows.net,1433;" +
                  "Initial Catalog=LetThemTrainDb;Persist Security Info=False;User ID=LetThemTrainAdmin;" +
                  "Password=LetThemTrain2024@;MultipleActiveResultSets=False;" +
                  "Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");*/


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Restrictions
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(40);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<User>()
                .Property(u => u.Gender)
                .HasMaxLength(1);

            modelBuilder.Entity<User>()
                .Property(u => u.DietType)
                .HasMaxLength(45);

            modelBuilder.Entity<User>()
                .Property(u => u.FitnessGoals)
                .HasMaxLength(45);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Progress Restrictions
            modelBuilder.Entity<Progress>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Progress>()
                .Property(p => p.Date)
                .IsRequired();

            // WorkoutPlan Restrictions
            modelBuilder.Entity<WorkoutPlan>()
                .HasKey(wp => wp.Id);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(wp => wp.Name)
                .IsRequired()
                .HasMaxLength(45);

            modelBuilder.Entity<WorkoutPlan>()
                .Property(wp => wp.Description)
                .HasMaxLength(500);

            // ExerciseSet Restrictions
            modelBuilder.Entity<ExerciseSet>()
                .HasKey(es => es.Id);

            modelBuilder.Entity<ExerciseSet>()
                .Property(es => es.Reps)
                .IsRequired();

            modelBuilder.Entity<ExerciseSet>()
                .Property(es => es.Sets)
                .IsRequired();

            // Exercise Restrictions
            modelBuilder.Entity<Exercise>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(45);

            modelBuilder.Entity<Exercise>()
                .Property(e => e.MainBodyPart)
                .HasMaxLength(45);
        
            modelBuilder.Entity<Exercise>()
                .Property(e => e.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<Exercise>()
                .HasIndex(e => e.Name)
                .IsUnique();

            //  Admin Restrictions
            modelBuilder.Entity<Admin>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Admin>()
                .Property(a => a.Login)
                .IsRequired()
                .HasMaxLength(45);

            modelBuilder.Entity<Admin>()
                .Property(a => a.Password)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Admin>()
                .Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();


            // Adding Admin and Exercises to default database
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Login = "admin",
                    Email = "admin@gmail.com",
                    Password = PasswordHasher.HashPassword("Admin1234@"),
                });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "user1",
                    Email = "user1@gmail.com",
                    Password = PasswordHasher.HashPassword("User1234@"),
                    Height = 180,
                    Gender = 'm',
                    Age = 22,
                    ActivityLevel = 3,
                    DietType = "keto",
                    IsActive = true,
                    FitnessGoals = 3,
                    MemberSince = DateTime.Now
                });

            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Name = "Bench Press",
                    Description = "Lie flat on a bench with feet firmly on the ground. Grip the bar slightly wider than " +
                      "shoulder-width. Squeeze shoulder blades, engage core and glutes. Lower the bar to your " +
                      "chest, then press it back up until arms are fully extended.",
                    MainBodyPart = "Chest",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 2,
                    Name = "Push-Up",
                    Description = "Start in a plank with hands slightly wider than shoulders. Keep core tight, back " +
                                  "straight, and glutes engaged. Lower your body until your chest is near the floor, then " +
                                  "push back up to the starting position.",
                    MainBodyPart = "Chest",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 3,
                    Name = "Squat",
                    Description = "Stand with feet shoulder-width apart. Engage your core and keep chest up. Lower hips " +
                                  "as if sitting in a chair, keeping knees over toes. Push through heels to return to " +
                                  "standing position.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 4,
                    Name = "Leg Press",
                    Description = "Sit on the machine with back flat against the pad. Place feet shoulder-width apart on " +
                                  "the platform. Lower the platform by bending knees to 90 degrees, then push it back up " +
                                  "without locking knees.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 5,
                    Name = "Deadlift",
                    Description = "Stand with feet hip-width apart and the barbell over your mid-foot. Hinge at hips and " +
                                  "bend knees to grip the bar. Keep your back straight and lift by extending hips and knees. " +
                                  "Lower the bar under control.",
                    MainBodyPart = "Back",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 6,
                    Name = "Pull-Up",
                    Description = "Grip the bar slightly wider than shoulders. Engage core, pull yourself up until your " +
                                  "chin is above the bar. Lower yourself back down in a controlled manner, fully extending " +
                                  "your arms.",
                    MainBodyPart = "Back",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 7,
                    Name = "Bicep Curl",
                    Description = "Stand upright with dumbbells in hands, palms facing forward. Keep elbows close to torso. " +
                                  "Curl dumbbells up, squeezing biceps, then lower back down slowly.",
                    MainBodyPart = "Arms",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 8,
                    Name = "Tricep Dips",
                    Description = "Position yourself on parallel bars with straight arms. Lower by bending elbows until " +
                                  "upper arms are parallel to the floor. Push back up to the starting position.",
                    MainBodyPart = "Arms",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 9,
                    Name = "Plank",
                    Description = "Get into a forearm plank with elbows under shoulders. Keep your body straight from head " +
                                  "to heels. Engage core, glutes, and legs. Hold this position without letting hips drop.",
                    MainBodyPart = "Core",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 10,
                    Name = "Crunches",
                    Description = "Lie on your back with knees bent, feet flat. Place hands behind your head. Engage core, " +
                                  "lift upper body toward knees, then lower slowly back to the ground.",
                    MainBodyPart = "Core",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 11,
                    Name = "Incline Dumbbell Press",
                    Description = "Lie on an incline bench with dumbbells in each hand. Press dumbbells upward until " +
                          "arms are fully extended. Lower them slowly back to chest level.",
                    MainBodyPart = "Chest",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 12,
                    Name = "Chest Fly",
                    Description = "Lie on a bench with dumbbells in hands. Extend arms above chest, then open arms wide " +
                                  "until they are parallel to the ground. Bring them back up with control.",
                    MainBodyPart = "Chest",
                    Image = new byte[0] 
                },
                new Exercise
                {
                    Id = 13,
                    Name = "Bulgarian Split Squat",
                    Description = "Stand with one foot on a bench behind you. Lower into a squat by bending the front knee. " +
                                  "Push through the front heel to return to standing.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 14,
                    Name = "Lunges",
                    Description = "Step forward with one leg and lower your body until both knees are at 90-degree angles. " +
                                  "Push back up to the starting position and switch legs.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 15,
                    Name = "Leg Curl",
                    Description = "Lie face down on a leg curl machine. Hook your ankles under the padded bar and curl your " +
                                  "legs towards your glutes. Lower with control.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 16,
                    Name = "Calf Raise",
                    Description = "Stand with feet shoulder-width apart. Raise your heels as high as possible, squeezing your " +
                                  "calves, then lower back down slowly.",
                    MainBodyPart = "Legs",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 17,
                    Name = "Seated Row",
                    Description = "Sit on a rowing machine with feet secured. Pull the handle towards your torso while keeping " +
                                  "your back straight and squeezing your shoulder blades. Return to the starting position.",
                    MainBodyPart = "Back",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 18,
                    Name = "Lat Pulldown",
                    Description = "Sit at a lat pulldown machine and grip the bar wider than shoulders. Pull the bar down to " +
                                  "your chest while keeping your back straight. Slowly release it back.",
                    MainBodyPart = "Back",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 19,
                    Name = "Barbell Row",
                    Description = "Stand with feet hip-width apart and bend at the hips. Grip the barbell with hands wider than " +
                                  "shoulders. Pull the bar towards your waist, then lower it under control.",
                    MainBodyPart = "Back",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 20,
                    Name = "Overhead Press",
                    Description = "Stand with feet shoulder-width apart, holding a barbell at shoulder height. Press the bar " +
                                  "overhead until arms are fully extended. Lower it back to shoulder height.",
                    MainBodyPart = "Arms",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 21,
                    Name = "Tricep Kickbacks",
                    Description = "Hold dumbbells in both hands, hinge forward slightly. Keep elbows close to your torso and " +
                                  "extend your arms back until straight. Return to the start.",
                    MainBodyPart = "Arms",
                    Image = new byte[0] 
                },
                new Exercise
                {
                    Id = 22,
                    Name = "Hammer Curls",
                    Description = "Hold dumbbells with palms facing each other. Curl the dumbbells up, keeping elbows close " +
                                  "to your torso. Lower them back down slowly.",
                    MainBodyPart = "Arms",
                    Image = new byte[0] 
                },
                new Exercise
                {
                    Id = 23,
                    Name = "Side Plank",
                    Description = "Lie on one side with your legs stacked. Prop yourself up on your forearm and keep your " +
                                  "body in a straight line. Hold this position.",
                    MainBodyPart = "Core",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 24,
                    Name = "Russian Twists",
                    Description = "Sit on the ground with your knees bent and feet lifted. Twist your torso from side to side, " +
                                  "touching the ground on each side.",
                    MainBodyPart = "Core",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 25,
                    Name = "Hanging Leg Raises",
                    Description = "Hang from a pull-up bar with arms fully extended. Lift your legs straight up to a 90-degree " +
                                  "angle, then lower them back down.",
                    MainBodyPart = "Core",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 26,
                    Name = "Hip Thrust",
                    Description = "Sit with your upper back against a bench and a barbell across your hips. Drive your hips " +
                                  "upwards, squeezing your glutes at the top. Lower back down slowly.",
                    MainBodyPart = "Glutes",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 27,
                    Name = "Cable Kickbacks",
                    Description = "Attach a cable to your ankle. Stand facing the machine and kick your leg back while keeping " +
                                  "your upper body still. Return slowly.",
                    MainBodyPart = "Glutes",
                    Image = new byte[0]
                },
                new Exercise
                {
                    Id = 28,
                    Name = "Glute Bridge",
                    Description = "Lie on your back with knees bent and feet flat on the floor. Lift your hips by squeezing " +
                                  "your glutes, forming a straight line from shoulders to knees. Lower slowly.",
                    MainBodyPart = "Glutes",
                    Image = new byte[0]
                });

        }



       
}
}
