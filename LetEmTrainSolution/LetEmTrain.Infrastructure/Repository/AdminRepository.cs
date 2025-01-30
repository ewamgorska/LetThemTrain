using LetEmTrain.Domain.Models;
using LetEmTrain.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetEmTrain.Infrastructure.Repository
{
    public class AdminRepository : Repository<Admin>, IAdminRepository
    {
        public AdminRepository(AppDbContext dbcontext) : base(dbcontext)
        {

        }


        public async Task<Admin> FindEmailAndPasswordAsync(string email, string password)
        {
            
            var admin = await _dbcontext.Admins.FirstOrDefaultAsync(u => u.Email == email);

            if (admin == null)
            {
                return null;
            }

            bool isPasswordValid = PasswordHasher.VerifyPassword(password, admin.Password);

            if (!isPasswordValid)
            {
                return null;
            }

            return admin;
        }
    }
}
