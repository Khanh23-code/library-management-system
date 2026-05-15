using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using THUVIENZ.BLL.Base;
using THUVIENZ.DAL;
using THUVIENZ.DAL.Base;
using THUVIENZ.Models;
using THUVIENZ.BLL.Services;

namespace THUVIENZ.BLL
{
    /// <summary>
    /// Service quản lý các quy định (Tham số) của thư viện.
    /// Tích hợp cơ chế Cache LRU để tối ưu tốc độ truy xuất.
    /// </summary>
    public class LibrarySettingsService : BaseService<ThamSo>
    {
        private readonly LRUCacheService<string, double> _paramCache;

        public LibrarySettingsService() : this(new BaseRepository<ThamSo>(new LmsDbContext()))
        {
        }

        public LibrarySettingsService(IRepository<ThamSo> repository) : base(repository)
        {
            _paramCache = new LRUCacheService<string, double>(50);
        }

        /// <summary>
        /// Lấy giá trị tham số theo tên. Ưu tiên lấy từ Cache.
        /// </summary>
        public async Task<double> GetValueAsync(string paramName)
        {
            var cachedValue = _paramCache.Get(paramName);
            if (cachedValue != default)
            {
                return cachedValue;
            }

            var param = await _repository.GetByIdAsync(paramName);
            if (param != null)
            {
                _paramCache.Set(paramName, param.GiaTri);
                return param.GiaTri;
            }

            throw new KeyNotFoundException($"Không tìm thấy tham số: {paramName}");
        }

        /// <summary>
        /// Cập nhật giá trị tham số và làm mới Cache.
        /// </summary>
        public async Task UpdateParamAsync(string name, double newValue)
        {
            var param = await _repository.GetByIdAsync(name);
            if (param == null)
            {
                param = new ThamSo { TenThamSo = name, GiaTri = newValue };
                await _repository.AddAsync(param);
            }
            else
            {
                param.GiaTri = newValue;
                _repository.Update(param);
            }

            await _repository.SaveChangesAsync();
            _paramCache.Set(name, newValue); // Cập nhật Cache ngay lập tức
        }

        /// <summary>
        /// Tải toàn bộ tham số vào Cache (Warm-up).
        /// </summary>
        public async Task WarmUpCacheAsync()
        {
            var allParams = await _repository.GetAllAsync();
            foreach (var p in allParams)
            {
                _paramCache.Set(p.TenThamSo, p.GiaTri);
            }
        }
    }
}
