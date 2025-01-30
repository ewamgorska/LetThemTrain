using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LetEmTrain.Infrastructure.Repository
{
    public class ExerciseRepository : Repository<Exercise>, IExerciseRepository
    {
        public ExerciseRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }

        public async Task<List<Exercise>> FindAllByBodyPartAsync(string bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart))
            {
                throw new ArgumentException("Body part cannot be null or empty.", nameof(bodyPart));
            }

            return await _dbcontext.Exercises
                .Where(e => e.MainBodyPart.ToLower() == bodyPart.ToLower())
                .ToListAsync();
        }

        public async Task<Exercise> FindAllByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Exercise name cannot be null or empty.", nameof(name));
            }

            return await _dbcontext.Exercises
                .FirstOrDefaultAsync(e => e.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Exercise>> SearchExercisesByMuscleGroupAsync(string bodyPart)
        {
            return await _dbcontext.Exercises
                .Where(e => e.MainBodyPart == bodyPart)
                .ToListAsync();
        }

        public async Task<List<Exercise>> FindAllByNameWithTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Search text cannot be null or empty.", nameof(text));
            }

            return await _dbcontext.Exercises
                .Where(e => EF.Functions.Like(e.Name, $"%{text}%") ||
                            EF.Functions.Like(e.Description, $"%{text}%"))
                .ToListAsync();
        }

        public async Task<Dictionary<string, List<Exercise>>> GetExercisesGroupedByMuscleGroupAsync()
        {
            return await _dbcontext.Exercises
                .GroupBy(e => e.MainBodyPart)
                .ToDictionaryAsync(group => group.Key, group => group.ToList());
        }   

        public async Task<List<Exercise>> SearchExercisesByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await _dbcontext.Exercises.ToListAsync();

            return await _dbcontext.Exercises
                .Where(e => EF.Functions.Like(e.Name, $"%{name}%"))
                .ToListAsync();
        }

        public async Task<List<Exercise>> SearchExercisesByKeywordAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _dbcontext.Exercises.ToListAsync();

            return await _dbcontext.Exercises
                .Where(e => EF.Functions.Like(e.Name, $"%{keyword}%") ||
                            EF.Functions.Like(e.Description, $"%{keyword}%") ||
                            EF.Functions.Like(e.MainBodyPart, $"%{keyword}%"))
                .ToListAsync();
        }

        public async Task EditAsync(Exercise exercise)
        {
            var existingExercise = await _dbcontext.Exercises.FirstOrDefaultAsync(e => e.Id == exercise.Id);
            if (existingExercise == null)
            {
                throw new KeyNotFoundException("Exercise not found.");
            }

            existingExercise.Name = exercise.Name;
            existingExercise.MainBodyPart = exercise.MainBodyPart;
            existingExercise.Image = exercise.Image;
            existingExercise.Description = exercise.Description;

            _dbcontext.Exercises.Update(existingExercise);
            await _dbcontext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Exercise exercise)
        {
            var existingExercise = await _dbcontext.Exercises.FirstOrDefaultAsync(e => e.Id == exercise.Id);
            if (existingExercise == null)
            {
                throw new KeyNotFoundException("Exercise not found.");
            }

            _dbcontext.Exercises.Remove(existingExercise);
            await _dbcontext.SaveChangesAsync();
        }


        async Task<List<Exercise>> IRepository<Exercise>.FindAllAsync()
        {
            return await _dbcontext.Exercises.ToListAsync();
        }


        public async Task CreateAsync(Exercise exercise)
        {
            _dbcontext.Exercises.Add(exercise);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
