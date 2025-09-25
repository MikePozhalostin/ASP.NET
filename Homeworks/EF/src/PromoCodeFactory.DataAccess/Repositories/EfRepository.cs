using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
using PromoCodeFactory.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _dataContext;
        public EfRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<T> CreateAsync(T entity)
        {
            var res = await _dataContext.Set<T>().AddAsync(entity);
            await _dataContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            var existEntity = await GetByIdAsync(id);

            if (existEntity != null)
            {
                _dataContext.Set<T>().Remove(existEntity);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Entity not found");
            }
        }

        public async Task UpdateAsync(T entity)
        {
            _dataContext.Set<T>().Update(entity);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dataContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dataContext.Set<T>().FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
