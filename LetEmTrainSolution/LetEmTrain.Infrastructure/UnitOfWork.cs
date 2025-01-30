using LetEmTrain.Domain;
using LetEmTrain.Domain.Repositories;
using LetEmTrain.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {

        private AppDbContext _dbContext;
        public UnitOfWork()
        {
            _dbContext = new AppDbContext();

            //Create database only if it doesn't exists
            _dbContext.Database.EnsureCreated();

            //Apply migrations
            //_dbContext.Database.Migrate();


        }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IUserRepository UserRepository 
            =>  new UserRepository(_dbContext);

        public IWorkoutPlanRepository WorkoutPlanRepository 
            => new WorkoutPlanRepository(_dbContext);

        public IExerciseSetRepository ExerciseSetRepository 
            => new ExerciseSetRepository(_dbContext);

        public IProgressRepository ProgressRepository 
            => new ProgressRepository(_dbContext);
        public IAdminRepository AdminRepository
            => new AdminRepository(_dbContext);
        public IExerciseRepository ExerciseRepository =>
        new ExerciseRepository(_dbContext);


        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public string GetDbPath()
        {
            return _dbContext.DbPath;
        }
    }
}
