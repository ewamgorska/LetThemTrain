using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetEmTrain.Domain.Models
{
   public class ExerciseSet : Entity
    {
        public int Reps { get; set; }
        public int Sets { get; set; }
        public int ExerciseId { get; set; }
        public int WorkoutPlanId { get; set; }
        public Exercise Exercise { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }    

        public ExerciseSet() { }

        public ExerciseSet(int reps, int sets, int exerciseId, int workoutPlanId)
        {
            Reps = reps;
            Sets = sets;
            ExerciseId = exerciseId;
            WorkoutPlanId = workoutPlanId;
        }
    }
}
