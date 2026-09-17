using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpConfig
{
  /// <summary>
  /// 
  /// </summary>
  public static class CacheConfigurationExtensions
  {
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    public static bool HasChanges(this IEnumerable<CacheSetting> section)
    //    {
    //        foreach (var item in section)
    //        {
    //            if (item.IsChanged) return true;
    //        }
    //        return false;
    //    }
    //    
    //    public static List<T> GetChanges<T>(this IEnumerable<T> section) where T : CacheSetting
    //    {
    //        return section.Where(o => o.IsChanged).ToList();
    //    }
    //    
    //    public static void AcceptChanges<T>(this IEnumerable<T> section) where T : CacheSetting
    //    {
    //        foreach (var item in section)
    //        {
    //            item.AcceptChange();
    //        }
    //    }
    //    
    //    public static void ResetChanges<T>(this IEnumerable<T> section) where T : CacheSetting
    //    {
    //        foreach(var item in section)
    //        {
    //            item.ResetChange();
    //        }
    //    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool HasChanges(this IEnumerable<ICacheSetting> section)
    {
      foreach (var item in section)
      {
        if (item.IsChanged)
          return true;
      }
      return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="section"></param>
    /// <returns></returns>
    public static List<T> GetChanges<T>(this IEnumerable<T> section) where T : ICacheSetting
    {
      return section.Where(o => o.IsChanged).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="section"></param>
    public static void AcceptChanges<T>(this IEnumerable<T> section) where T : ICacheSetting
    {
      foreach (var item in section)
      {
        item.AcceptChange();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="section"></param>
    public static void ResetChanges<T>(this IEnumerable<T> section) where T : ICacheSetting
    {
      foreach (var item in section)
      {
        item.ResetChange();
      }
    }


  }
}
