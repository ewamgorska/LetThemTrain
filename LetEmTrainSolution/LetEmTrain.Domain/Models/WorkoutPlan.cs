using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LetEmTrain.Domain.Models
{
    public class WorkoutPlan : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<ExerciseSet> ExerciseSets { get; set; } = new List<ExerciseSet>();
        public WorkoutPlan() { }
        public WorkoutPlan(int userId, string name, string description)
        {
            this.Name = name;
            this.Description = description;
            this.CreateDate = DateTime.Now;
            this.UserId = userId;

        }
    }
}
