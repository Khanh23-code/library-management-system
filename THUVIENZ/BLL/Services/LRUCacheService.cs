using System;
using System.Collections.Generic;
using System.Linq;

namespace THUVIENZ.BLL.Services
{
    /// <summary>
    /// Dịch vụ In-process Caching sử dụng chính sách LRU (Least Recently Used).
    /// Giúp tối ưu hiệu năng truy xuất các dữ liệu ít thay đổi như Tham số hệ thống.
    /// </summary>
    /// <typeparam name="K">Kiểu dữ liệu của Khóa</typeparam>
    /// <typeparam name="V">Kiểu dữ liệu của Giá trị</typeparam>
    public class LRUCacheService<K, V> where K : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<K, LinkedListNode<CacheItem>> _cacheMap;
        private readonly LinkedList<CacheItem> _lruList;

        public LRUCacheService(int capacity = 100)
        {
            _capacity = capacity;
            _cacheMap = new Dictionary<K, LinkedListNode<CacheItem>>(capacity);
            _lruList = new LinkedList<CacheItem>();
        }

        public V? Get(K key)
        {
            if (!_cacheMap.ContainsKey(key))
                return default;

            var node = _cacheMap[key];
            _lruList.Remove(node);
            _lruList.AddFirst(node); // Đưa lên đầu danh sách vì vừa được sử dụng

            return node.Value.Value;
        }

        public void Set(K key, V value)
        {
            if (_cacheMap.ContainsKey(key))
            {
                var node = _cacheMap[key];
                _lruList.Remove(node);
                _cacheMap.Remove(key);
            }

            if (_cacheMap.Count >= _capacity)
            {
                RemoveLeastRecentlyUsed();
            }

            var newNode = new LinkedListNode<CacheItem>(new CacheItem(key, value));
            _lruList.AddFirst(newNode);
            _cacheMap.Add(key, newNode);
        }

        private void RemoveLeastRecentlyUsed()
        {
            var lastNode = _lruList.Last;
            if (lastNode != null)
            {
                _lruList.RemoveLast();
                _cacheMap.Remove(lastNode.Value.Key);
            }
        }

        public void Clear()
        {
            _cacheMap.Clear();
            _lruList.Clear();
        }

        private class CacheItem
        {
            public K Key { get; }
            public V Value { get; }
            public CacheItem(K key, V value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
