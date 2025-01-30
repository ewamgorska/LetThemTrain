using LetEmTrain.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Infrastructure.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : Entity
    {
        protected readonly AppDbContext _dbcontext;

        public Repository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public void Create(T entity)
        {
            _dbcontext.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbcontext.Remove(entity);
        }

        public Task<List<T>> FindAllAsync()
        {
            return _dbcontext.Set<T>().ToListAsync();
        }

        public Task<T> FindByIdAsync(int id)
        {
            return _dbcontext.Set<T>().SingleAsync(x => x.Id == id);

        }

        public void Update(T entity)
        {
            _dbcontext.Update(entity);
        }
    }
}
