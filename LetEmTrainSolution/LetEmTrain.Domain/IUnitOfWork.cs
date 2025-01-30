using LetEmTrain.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IWorkoutPlanRepository WorkoutPlanRepository { get; }
        IExerciseSetRepository ExerciseSetRepository { get; }
        IProgressRepository ProgressRepository { get; }
        IAdminRepository AdminRepository { get; }

        Task SaveAsync();
    }
}
