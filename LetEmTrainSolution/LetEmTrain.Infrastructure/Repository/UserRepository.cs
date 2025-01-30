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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }

        public async Task<List<User>> FindAllWithUsernameStartedWithAsync(string text)
        {
            return await _dbcontext.Users
                .Where(user => user.Username.StartsWith(text))
                .ToListAsync();
        }


        public async Task<User> FindByEmailAndPasswordAsync(string email, string password)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            return user;
        }
        public async Task<User> FindWithUsernameAsync(string name)
        {
                return await _dbcontext.Users
                    .FirstOrDefaultAsync(user => user.Username == name);
        }

        public async Task<User> FindByIdAsync(int userId)
        {
            return await _dbcontext.Users.FindAsync(userId);
        }

        public async Task<User> UpdateAsync(User entity)
        {
            var existingUser = await _dbcontext.Users.FindAsync(entity.Id);

            if (existingUser == null)
            {
                throw new ArgumentException($"User with ID {entity.Id} not found.");
            }

            // Update the entity
            _dbcontext.Entry(existingUser).CurrentValues.SetValues(entity);

            // Save changes to the database
            await _dbcontext.SaveChangesAsync();

            // Return the updated entity
            return existingUser;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbcontext.Users.ToListAsync(); // Retrieves all users from the database
        }

        public async Task DeactivateUserAsync(int userId)
        {
            var user = await _dbcontext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            user.IsActive = false; // Set IsActive to false
            await _dbcontext.SaveChangesAsync();
        }

        public async Task ActivateUserAsync(int userId)
        {
            var user = await _dbcontext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            user.IsActive = true;
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _dbcontext.Users
                .FirstOrDefaultAsync(u => EF.Functions.Like(u.Email, email));
        }
        public async Task UpdateProfilePictureAsync(int userId, byte[] profilePicture)
        {
            var user = await _dbcontext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException($"User with ID {userId} not found.");
            }

            user.ProfilePicture = profilePicture;
            await _dbcontext.SaveChangesAsync();
        }

        public async Task<List<User>> SearchUsersByEmailAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return await _dbcontext.Users.ToListAsync();

            return await _dbcontext.Users
                .Where(e => EF.Functions.Like(e.Email, $"%{text}%"))
                .ToListAsync();
        }
    }
}
