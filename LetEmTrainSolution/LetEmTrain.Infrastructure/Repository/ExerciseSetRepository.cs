using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using LetEmTrain.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Infrastructure.Repository
{
    public class ExerciseSetRepository : Repository<ExerciseSet>, IExerciseSetRepository
    {
        public ExerciseSetRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }

        public async Task<List<ExerciseSet>> FindWorkoutPlanAllAsync(int id)
        {
            return await _dbcontext.ExerciseSets
                .Where(s => s.WorkoutPlanId == id).ToListAsync();
        }

    }
}
