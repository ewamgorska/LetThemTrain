using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.SeedWork;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.Repositories
{
    public interface IExerciseRepository : IRepository<Exercise>
    {
        Task<Dictionary<string, List<Exercise>>> GetExercisesGroupedByMuscleGroupAsync();
        Task<List<Exercise>> SearchExercisesByNameAsync(string name);
        Task<List<Exercise>> SearchExercisesByMuscleGroupAsync(string bodyPart);
        Task<List<Exercise>> SearchExercisesByKeywordAsync(string keyword);
        Task<List<Exercise>> FindAllByBodyPartAsync(string bodypart);
        Task CreateAsync(Exercise exercise);
        Task EditAsync(Exercise exercise);
        Task DeleteAsync(Exercise exercise);

    }
}
