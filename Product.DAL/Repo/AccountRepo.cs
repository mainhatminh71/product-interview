using Microsoft.EntityFrameworkCore;
using Product.DAL.Data;
using Product.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.DAL.Repo
{
    public class AccountRepo : IAccountRepo
    {
        private readonly ProductDbContext _ctx;
        public AccountRepo(ProductDbContext ctx)
        {
            _ctx = ctx;
        } 
        public async Task<User?> GetByEmailAsync(string email) 
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            email = email.Trim().ToLowerInvariant();

            return await _ctx.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _ctx.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task CreateUserAsync(Entities.User user)
        {
            _ctx.Add(user);
            _ctx.SaveChanges();
        }
        public async Task UpdateUserAsync(Entities.User user)
        {
            _ctx.Update(user);
            _ctx.SaveChanges();
        }
    }
}
