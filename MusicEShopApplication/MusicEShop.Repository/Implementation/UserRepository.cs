using Microsoft.EntityFrameworkCore;
using MusicEShop.Domain.Identity;
using MusicEShop.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<MusicEShopUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<MusicEShopUser>();
        }

        public void Delete(MusicEShopUser entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MusicEShopUser> GetAll()
        {
            throw new NotImplementedException();
        }

        public MusicEShopUser GetById(string? id)
        {
            throw new NotImplementedException();
        }

        public void Insert(MusicEShopUser entity)
        {
            throw new NotImplementedException();
        }

        public void Update(MusicEShopUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
