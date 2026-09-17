using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConfig;

namespace SharpConfig
{
  /// <summary>
  /// 
  /// </summary>
  public class CacheValueConfiguration
  {
    // private SharpConfig.Configuration _values;
    // private Dictionary<string, KeyValueSetting> _strings;
    // private List<KeyValueSetting> _keyValues;

    private Dictionary<string, CacheValueSetting> _settings;
    private CacheConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    public CacheValueConfiguration()
    {
      _settings = new Dictionary<string, CacheValueSetting>();
      _configuration = new CacheConfiguration();
    }

    /// <summary>
    /// 
    /// </summary>
    public List<CacheValueSetting> Settings => _settings.Values.ToList();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CacheValueSetting Get(KeySetting key)
    {
      return _settings.TryGetValue(key.VerCode, out var value) ? value : throw new Exception($"{key.VerCode} 不存在");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public CacheValueSetting<T> Get<T>(KeySetting key)
    {
      var setting = Get(key);
      return new CacheValueSetting<T>(setting);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CacheValueSetting Get(string code, string ver = "0")
    {
      return _settings.TryGetValue(KeySetting.GetVerCode(code, ver), out var value) ? value : throw new Exception($"{KeySetting.GetVerCode(code, ver)} 不存在");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="code"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
    public CacheValueSetting<T> Get<T>(string code, string ver = "0")
    {
      var setting = Get(code, ver);
      return new CacheValueSetting<T>(setting);
    }

    private void InternalAdd(CacheValueSetting setting)
    {
      _settings.Add(setting.KeySetting.VerCode, setting);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    public CacheValueSetting Add(CacheValueSetting setting)
    {
      InternalAdd(setting);
      return setting;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setting"></param>
    /// <returns></returns>
    public CacheValueSetting<T> Add<T>(CacheValueSetting setting)
    {
      InternalAdd(setting);
      return new CacheValueSetting<T>(setting);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="defaultLevel"></param>
    /// <param name="defaultValue"></param>
    /// <param name="unit"></param>
    /// <param name="text"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CacheValueSetting Add(string code, int defaultLevel, string defaultValue, string unit, string text, string ver = "0")
    {
      if (!_settings.ContainsKey(code))
        throw new Exception("code 已存在");

      var key = new KeySetting(code, ver);
      key.Text = text;
      key.Unit = unit;
      key.RawLevel = 0;
      key.DefaultLevel = defaultLevel;

      var setting = new CacheValueSetting(key, defaultValue, defaultValue);
      return Add(setting);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="code"></param>
    /// <param name="defaultLevel"></param>
    /// <param name="defaultValue"></param>
    /// <param name="unit"></param>
    /// <param name="text"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public CacheValueSetting<T> Add<T>(string code, int defaultLevel, T defaultValue, string unit, string text, string ver = "0")
    {
      if (!_settings.ContainsKey(code))
        throw new Exception("code 已存在");

      var key = new KeySetting(code, ver);
      key.Text = text;
      key.Unit = unit;
      key.RawLevel = 0;
      key.DefaultLevel = defaultLevel;

      var setting = new CacheValueSetting(key, defaultValue!, defaultValue!);
      return Add<T>(setting);
    }

    /// <summary>
    /// 获取设置列表
    /// </summary>
    /// <param name="level"></param>
    /// <param name="clone">true 返回副本</param>
    /// <returns></returns>
    public List<CacheValueSetting> SelectList(int level, bool clone = true)
    {
      if (clone)
      {
        // 返回副本
        return _settings.Values.Where(o => o.Level <= level).Select(o => new CacheValueSetting(o.KeySetting, o.DefaultSetting, o.RawValue)).ToList();
      }
      else
      {
        return _settings.Values.Where(o => o.Level <= level).ToList();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int SelectCount(int level)
    {
      return _settings.Values.Where(o => o.Level <= level).Count();
    }

    /// <summary>
    /// 写入到当前设置中
    /// 
    /// CommitChanges
    /// UpdateChanges
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public int CommitChanges(IEnumerable<CacheValueSetting> settings)
    {
      int count = 0;
      foreach (var item in settings)
      {
        if (item.IsChanged)
        {
          if (_settings.TryGetValue(item.Key, out var setting))
          {
            if (!item.EqualsGuid(setting))
            {
              // 副本更新到正本上的条件和方式
              // 有可能覆盖全局当前值
              setting.RawLevel = item.RawLevel; // ?? 是否需要跟默认Level 判断然后写入0
              setting.RawValue = item.RawValue;

              setting.AcceptChange();
            }
          }

          item.AcceptChange();
          count++;
        }
      }
      return count;
    }

    /// <summary>
    /// 把全局值同步到设置中
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public int ResetChnages(IEnumerable<CacheValueSetting> settings)
    {
      int count = 0;
      foreach (var item in settings)
      {
        if (_settings.TryGetValue(item.VerCode, out var setting))
        {
          item.ResetChange(setting);
        }
        else
        {
          item.ResetChange();
        }
      }
      return count;
    }
  }

  // public interface ICacheSetting
  // {
  // 
  // }

  // public class CacheSetting : ConfigurationElement
  // {
  //     /// <summary>
  //     /// Initializes a new instance of the <see cref="Setting"/> class.
  //     /// </summary>
  //     public CacheSetting(string name) : this(name, string.Empty)
  //     {
  //     }
  // 
  //     /// <summary>
  //     /// Initializes a new instance of the <see cref="Setting"/> class.
  //     /// </summary>
  //     ///
  //     /// <param name="name"> The name of the setting.</param>
  //     /// <param name="value">The value of the setting.</param>
  //     public CacheSetting(string name, object value) : base(name)
  //     {
  //     }
  // }
  // 
  // public class CacheSection 
  // {
  //     
  // }

  // public class CacheSettingXXX
  // {
  //     public static void XXX()
  //     {
  //         var cfg = new CacheValueConfiguration();
  // 
  //         // cfg.Add("0001001", 1, "474698.10804", "脉冲/mm", "X轴脉冲当量");  // 默认 string 类型
  //         // cfg.Add<float>("0001001", 1, 474698.10804f, "脉冲/mm", "X轴脉冲当量");
  //         var xxx1 = cfg.Get("0001001");
  //         var xxx2 = cfg.Get<float>("0001001");
  //         var xxx3 = cfg.Add("0001001", 1, "474698.10804", "脉冲/mm", "X轴脉冲当量");  // 默认 string 类型
  //         CacheValueSetting<float> xxx4 = cfg.Add<float>("0001001", 1, 474698.10804f, "脉冲/mm", "X轴脉冲当量");
  // 
  //         var x = xxx4.Value;
  // 
  //         // var list = cfg.SelectList(); // Section
  //         List<CacheValueSetting> list = cfg.SelectList(7); // Section clone
  //         bool f1 = list.HasChanges();
  //         List<CacheValueSetting> ls = list.GetChanges();  // list
  // 
  //         int count = cfg.CommitChanges(list);
  //     }
  // 
  // }
}
