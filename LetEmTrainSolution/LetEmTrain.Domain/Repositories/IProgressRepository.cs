using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.Repositories
{
    public interface IProgressRepository : IRepository<Progress>
    {
        Task<List<Progress>> FindAllByTimePeriodAsync(int userId, DateTime startDate, DateTime endDate);

        Task<List<Progress>> FindAllUserProgressAsync(int userId);

        Task <Progress> FindLastAsync(int userId);
    }
}