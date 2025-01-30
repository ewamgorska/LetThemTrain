using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Infrastructure.Repository
{
    public class ProgressRepository : Repository<Progress>, IProgressRepository
    {
        public ProgressRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }


        public async Task<List<Progress>> FindAllByTimePeriodAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbcontext.ProgressRecords
                .Where(p => p.UserId == userId && p.Date >= startDate && p.Date <= endDate)
                .ToListAsync();
        }

        public async Task<List<Progress>> FindAllUserProgressAsync(int userId)
        {
            return await _dbcontext.ProgressRecords
                .Where(p => p.UserId == userId).ToListAsync();
        }


        public async Task<Progress> FindLastAsync(int userId)
        {
            return await _dbcontext.ProgressRecords
                .Where(p => p.Date <= DateTime.Now && p.UserId == userId)
                .OrderByDescending(p => p.Date) 
                .FirstOrDefaultAsync(); 
        }

    }
}
