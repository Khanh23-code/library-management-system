using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace THUVIENZ.DAL.Base
{
    /// <summary>
    /// Interface Generic cho tầng Data Access (DAL).
    /// Toàn bộ chú thích được viết bằng tiếng Việt theo yêu cầu của Tech Lead.
    /// </summary>
    /// <typeparam name="T">Thực thể (Entity) trong Database</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> SaveChangesAsync();
    }

    /// <summary>
    /// Lớp cơ sở triển khai Generic Repository sử dụng EF Core 8.
    /// </summary>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly LmsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(LmsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
