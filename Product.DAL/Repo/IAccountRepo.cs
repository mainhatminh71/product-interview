using Product.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Product.DAL.Repo
{
    public interface IAccountRepo
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
