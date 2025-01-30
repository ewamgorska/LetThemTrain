using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Domain.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> FindWithUsernameAsync(string name);
        Task<List<User>> FindAllWithUsernameStartedWithAsync(string text);

        Task<User> FindByEmailAndPasswordAsync(string email, string password);
        Task<User> FindByEmailAsync(string email);
        Task<List<User>> SearchUsersByEmailAsync(string text);
        Task<User> UpdateAsync(User entity);
        Task<List<User>> GetAllAsync();
        Task ActivateUserAsync(int userId);
        Task DeactivateUserAsync(int userId);

        Task UpdateProfilePictureAsync(int userId, byte[] profilePicture);
        Task<User> FindByIdAsync(int userId);

    }
}
