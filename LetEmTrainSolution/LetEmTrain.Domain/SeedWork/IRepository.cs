using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.SeedWork
{
        public interface IRepository<T> where T : Entity
        {
            void Create(T entity);
            void Update(T entity);
            void Delete(T entity);
            Task<T> FindByIdAsync(int id);
            Task<List<T>> FindAllAsync();
        }
}
