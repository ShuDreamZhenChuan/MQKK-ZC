
using System;
using System.Collections;
using System.Web;
using System.Web.Caching;

namespace Moqikaka.GamerReturn
{
    /// <summary>
    /// 操作缓存数据的类
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 获取当前应用程序指定cacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">
        /// <returns></returns>
        public static object GetCache(string cacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[cacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定cacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">
        /// <param name="objObject">
        public static void SetCache(string cacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定cacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey">
        /// <param name="objObject">
        public static void SetCache(string cacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>  
        /// 设置数据缓存  
        /// </summary> 
        public static void SetCache(string cacheKey, object objObject, int timeout = 3600)
        {
            try
            {
                if (objObject == null) return;
                var objCache = HttpRuntime.Cache;
               
                //相对过期  
                //objCache.Insert(cacheKey, objObject, null, DateTime.MaxValue, timeout, CacheItemPriority.NotRemovable, null);  
                
                //绝对过期时间                
                objCache.Insert(cacheKey, objObject, null, DateTime.Now.AddSeconds(timeout), TimeSpan.Zero, CacheItemPriority.High, null);
            }
            catch (Exception)
            {
                //throw;  
            }
        }

        /// <summary>
        /// 清除单一键缓存
        /// </summary>
        /// <param name="key">
        public static void RemoveKeyCache(string cacheKey)
        {
            try
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                objCache.Remove(cacheKey);
            }
            catch { }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
            if (_cache.Count > 0)
            {
                ArrayList al = new ArrayList();
                while (CacheEnum.MoveNext())
                {
                    al.Add(CacheEnum.Key);
                }
                foreach (string key in al)
                {
                    _cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// 获取所有缓存项
        /// </summary>
        /// <returns></returns>
        public static ArrayList ShowAllCache()
        {
            ArrayList al = new ArrayList();
            System.Web.Caching.Cache _cache = HttpRuntime.Cache;
            if (_cache.Count > 0)
            {
                IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();

                while (CacheEnum.MoveNext())
                {
                    al.Add(CacheEnum.Key);
                }
            }
            return al;
        }
    }

}
