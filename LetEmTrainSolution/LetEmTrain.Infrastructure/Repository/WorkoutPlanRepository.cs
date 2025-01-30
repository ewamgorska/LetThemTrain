using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using LetEmTrain.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LetEmTrain.Infrastructure.Repository
{
    public class WorkoutPlanRepository : Repository<WorkoutPlan>, IWorkoutPlanRepository
    {
        public WorkoutPlanRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }

        public async Task<List<WorkoutPlan>> FindAllByNameWithTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return await _dbcontext.WorkoutPlans.ToListAsync();

            return await _dbcontext.WorkoutPlans
                .Where(e => EF.Functions.Like(e.Name, $"%{text}%") ||
                            EF.Functions.Like(e.Description, $"%{text}%"))
                .ToListAsync();
        }

        public async Task<List<WorkoutPlan>> FindAllUserWorkoutPlans(int userId)
        {
            return await _dbcontext.WorkoutPlans
                .Where(plan => plan.UserId == userId)
                .ToListAsync();

        }


        public async Task<WorkoutPlan> FindUserLastAsync(int userId)
        {
            return await _dbcontext.WorkoutPlans
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreateDate)
                .FirstOrDefaultAsync();

        }

        public async Task UpdateExerciseSet(int id,ExerciseSet set)
        {
            WorkoutPlan plan = await _dbcontext.WorkoutPlans.FindAsync(id);
            plan.ExerciseSets.Add(set);
        }
    }
}
