using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using THUVIENZ.DAL.Base;

namespace THUVIENZ.BLL.Base
{
    /// <summary>
    /// Interface Generic cho tầng Business Logic (BLL).
    /// </summary>
    public interface IBaseService<T> where T : class
    {
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(object id);
    }

    /// <summary>
    /// Lớp cơ sở triển khai Business Service.
    /// Toàn bộ chú thích được viết bằng tiếng Việt theo yêu cầu.
    /// </summary>
    public abstract class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IRepository<T> _repository;

        protected BaseService(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<T?> GetByIdAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Delete(entity);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
