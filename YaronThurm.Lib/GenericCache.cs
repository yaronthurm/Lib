using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YaronThurm.Lib
{
    /// <summary>
    /// There are only 4 requirements for implementations:
    /// 1. Inherit the class and set the generics types
    /// 2. Create a private static member of the derived class and instantiate it.
    /// 3. Override the 'GetExpiration' abstarct mathod
    /// 4. Override the 'LoadItemImpl' abstarct mathod
    /// 
    /// After that, the implementation will not be usefull without exposing the functionality out, 
    /// so you might want to expose a "Get" and "Revoke" methods, that will rely on the underlying class
    /// 
    /// Implementation example:
    /// public class SitesCache : GenericCache<ulong, Site>                 // Fullfill requierment #1
    /// {
    ///     private static SitesCache s_instance = new SitesCache();        // Fullfill requierment #2
    ///     
    ///     protected override DateTime GetExpiration()                     // Fullfill requierment #3
    ///     {
    ///         return ExpirationData.Sites;
    ///     }
    ///     
    ///     protected override Site LoadItemImpl(ulong key)                 // Fullfill requierment #4
    ///     {
    ///         return Site.Load(key);
    ///     }
    ///     
    ///     public static Site GetSite(ulong siteID)                        // Exposing functionality for "Get"
    ///     {
    ///         return s_instance.GetItem(siteID);
    ///     }
    ///     
    ///     public static void RevokeSite(ulong siteID)                     // Exposing functionality for "Revoke"
    ///     {
    ///         s_instance.RevokeItem(siteID);
    ///     }
    /// }
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public abstract class GenericCache<TKey, TItem>
    {
        private Dictionary<TKey, CacheInformation<TItem>> _cache = new Dictionary<TKey, CacheInformation<TItem>>();
        private ReaderWriterLockSlim _instanceLock = new ReaderWriterLockSlim();
        private int _maxCacheSize = int.MaxValue;

        /// <summary>
        /// Holds information about the expiration of an item
        /// </summary>
        private class ExpirationInformation
        {
            private DateTime _expirationDate;
            private bool _expiredManually;

            public ExpirationInformation(DateTime expiration)
            {
                this._expirationDate = expiration.ToUniversalTime();
            }

            public void SetToExpire()
            {
                this._expiredManually = true;
            }

            public bool IsExpired
            {
                get
                {
                    bool ret = this._expiredManually || this._expirationDate < DateTime.UtcNow;
                    return ret;
                }
            }
        }

        /// <summary>
        /// Encapsulates an item to be cached and information about its expiration
        /// </summary>
        private class CacheInformation<T>
        {
            public T Item { get; set; }
            public ExpirationInformation ExpirationInfo { get; set; }
        }


        protected abstract TItem LoadItemImpl(TKey key);
        protected abstract DateTime GetExpiration(TKey key);


        protected TItem GetItem(TKey key)
        {
            #region Look in cache

            try
            {
                _instanceLock.EnterReadLock();

                CacheInformation<TItem> cache;
                if (_cache.TryGetValue(key, out cache) && cache != null)
                {
                    if (cache.Item != null && cache.ExpirationInfo != null && !cache.ExpirationInfo.IsExpired)
                        return cache.Item;
                }
            }
            finally
            {
                _instanceLock.ExitReadLock();
            }
            #endregion

            // Item wasn't found in cache, or it neeeds refreshing

            #region Load item, store to cache and return it

            TItem ret = this.LoadItemImpl(key);
            DateTime expiration = this.GetExpiration(key);
            this.SetItem(key, ret, expiration);
            return ret;

            #endregion
        }

        protected void SetItem(TKey key, TItem item, DateTime expiration)
        {
            CacheInformation<TItem> cache = new CacheInformation<TItem>
            {
                Item = item,
                ExpirationInfo = new ExpirationInformation(expiration)
            };

            try
            {
                _instanceLock.EnterWriteLock();
                _cache[key] = cache;

                // Cache Size Controrl
                if (_cache.Count > this.MaxCacheSize)
                {
                    _cache.Clear(); // Not very sophisticated but does the job of keeping a limit on the cache
                    _cache[key] = cache;
                }
            }
            finally
            {
                _instanceLock.ExitWriteLock();
            }
        }

        protected void RevokeItem(TKey key)
        {
            try
            {
                _instanceLock.EnterWriteLock();

                // Mark item as expired - it will cause a re-load next time it will be requested
                CacheInformation<TItem> cache;
                if (_cache.TryGetValue(key, out cache) && cache != null)
                {
                    if (cache.ExpirationInfo == null)
                        cache.ExpirationInfo = new ExpirationInformation(DateTime.MinValue);

                    cache.ExpirationInfo.SetToExpire();
                }
            }
            finally
            {
                _instanceLock.ExitWriteLock();
            }
        }

        protected void RevokeAll()
        {
            try
            {
                _instanceLock.EnterWriteLock();
                _cache.Clear();
            }
            finally
            {
                _instanceLock.ExitWriteLock();
            }
        }

        protected int MaxCacheSize
        {
            get { return _maxCacheSize; }
            set { _maxCacheSize = value; }
        }
    }
}
