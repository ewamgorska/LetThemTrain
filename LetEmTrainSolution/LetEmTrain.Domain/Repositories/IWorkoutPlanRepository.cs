using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.Repositories
{
    public interface IWorkoutPlanRepository :IRepository <WorkoutPlan>
    {

        Task<List<WorkoutPlan>> FindAllByNameWithTextAsync(string text);
        Task<WorkoutPlan> FindUserLastAsync(int userId);
        Task<List<WorkoutPlan>> FindAllUserWorkoutPlans(int userId);
        Task UpdateExerciseSet(int id,ExerciseSet set);

    }
}
