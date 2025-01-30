using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.Repositories
{
    public interface IExerciseSetRepository : IRepository<ExerciseSet>
    {
        Task<List<ExerciseSet>> FindWorkoutPlanAllAsync(int id);
    }
}
