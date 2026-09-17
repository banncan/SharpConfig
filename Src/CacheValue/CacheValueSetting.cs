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
  public interface ICacheValueSetting
  {
    /// <summary>
    /// 
    /// </summary>
    KeySetting KeySetting { get; }
    /// <summary>
    /// 
    /// </summary>
    CacheValueSetting ValueSetting { get; }
  }

  /// <summary>
  /// 
  /// </summary>
  public interface ICacheSetting
  {
    /// <summary>
    /// 
    /// </summary>
    KeySetting KeySetting { get; }

    /// <summary>
    /// 
    /// </summary>
    string VerCode { get; }
    /// <summary>
    /// 
    /// </summary>
    string Code { get; }
    /// <summary>
    /// 
    /// </summary>
    string Ver { get; }
    /// <summary>
    /// 
    /// </summary>
    string Text { get; }
    /// <summary>
    /// 
    /// </summary>
    string Unit { get; }

    /// <summary>
    /// 
    /// </summary>
    int RawLevel { get; }

    /// <summary>
    /// 
    /// </summary>
    string Key { get; }

    /// <summary>
    /// 
    /// </summary>
    string RawValue { get; }
    /// <summary>
    /// 
    /// </summary>
    string DefaultRawValue { get; }


    /// <summary>
    /// 
    /// </summary>
    bool IsChanged { get; }

    /// <summary>
    /// 
    /// </summary>
    void AcceptChange();
    /// <summary>
    /// 
    /// </summary>
    void ResetChange();
  }

  /// <summary>
  /// 
  /// </summary>
  public class CacheValueSetting : CacheSetting, ICacheSetting
  {
    private KeySetting _keySetting;
    // private CacheSetting _valueSetting;
    private CacheSetting _defaultSetting;

    // private TbSetting _itemg;


    /// <summary>
    /// 
    /// </summary>
    public KeySetting KeySetting { get => _keySetting; }
    // public CacheSetting ValueSetting { get => _valueSetting; }
    /// <summary>
    /// 
    /// </summary>
    public CacheSetting DefaultSetting { get => _defaultSetting; }

    // public TbSetting Item { get => _itemg; set => _itemg = value; }

    /// <summary>
    /// 
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string VerCode { get => _keySetting.VerCode; }
    /// <summary>
    /// 
    /// </summary>
    public string Code { get => _keySetting.Code; }
    /// <summary>
    /// 
    /// </summary>
    public string Ver { get => _keySetting.Ver; }
    /// <summary>
    /// 
    /// </summary>
    public string Text { get => _keySetting.Text; set => _keySetting.Text = value; }
    /// <summary>
    /// 
    /// </summary>
    public string Unit { get => _keySetting.Unit; set => _keySetting.Unit = value; }

    /// <summary>
    /// 
    /// </summary>
    public int Level { get => _keySetting.Level; }
    /// <summary>
    /// 
    /// </summary>
    public int RawLevel { get => _keySetting.RawLevel; set => _keySetting.RawLevel = value; }
    /// <summary>
    /// 
    /// </summary>
    public int DefaultLevel { get => _keySetting.DefaultLevel; set => _keySetting.DefaultLevel = value; }

    /// <summary>
    /// 
    /// </summary>
    public string Key { get => _keySetting.Key; set => _keySetting.Key = value; }

    /// <summary>
    /// 
    /// </summary>
    public string DefaultRawValue { get => _defaultSetting.RawValue; set => _defaultSetting.RawValue = value; }

    /// <summary>
    /// 
    /// </summary>
    public string Value { get => this.RawValue; set => this.RawValue = value; }
    /// <summary>
    /// 
    /// </summary>
    public string DefaultValue { get => _defaultSetting.RawValue; set => _defaultSetting.RawValue = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <param name="value"></param>
    public CacheValueSetting(KeySetting key, CacheSetting defaultValue, object value) : base(key.Code, value)
    {
      this._keySetting = key;
      this._defaultSetting = defaultValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    public CacheValueSetting(KeySetting key, CacheSetting defaultValue) : this(key, defaultValue, defaultValue.RawValue)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    public CacheValueSetting(KeySetting key, object defaultValue, object value) : this(key, new CacheSetting(key.VerCode, defaultValue), defaultValue)
    {
    }

    // /// <summary>
    // /// 
    // /// </summary>
    // /// <param name="code"></param>
    // /// <param name="value"></param>
    // /// <param name="defaultValue"></param>
    // public CacheValueSetting(string code, object defaultValue, object value) : this(new KeySetting(code), new CacheSetting(code, defaultValue), defaultValue)
    // {
    // }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class CacheValueSetting<T> : ICacheSetting, ICacheValueSetting
  {
    private CacheValueSetting _setting;

    /// <summary>
    /// 
    /// </summary>
    public KeySetting KeySetting { get => _setting.KeySetting; }
    /// <summary>
    /// 
    /// </summary>
    public CacheSetting DefaultSetting { get => _setting.DefaultSetting; }

    /// <summary>
    /// 
    /// </summary>
    public CacheValueSetting ValueSetting { get => _setting; }

    /// <summary>
    /// 
    /// </summary>
    public string VerCode { get => _setting.KeySetting.VerCode; }
    /// <summary>
    /// 
    /// </summary>
    public string Code { get => _setting.KeySetting.Code; }
    /// <summary>
    /// 
    /// </summary>
    public string Ver { get => _setting.KeySetting.Ver; }
    /// <summary>
    /// 
    /// </summary>
    public string Text { get => _setting.KeySetting.Text; set => _setting.KeySetting.Text = value; }
    /// <summary>
    /// 
    /// </summary>
    public string Unit { get => _setting.KeySetting.Unit; set => _setting.KeySetting.Unit = value; }

    /// <summary>
    /// 
    /// </summary>
    public int Level { get => _setting.KeySetting.Level; }
    /// <summary>
    /// 
    /// </summary>
    public int RawLevel { get => _setting.KeySetting.RawLevel; set => _setting.KeySetting.RawLevel = value; }
    /// <summary>
    /// 
    /// </summary>
    public int DefaultLevel { get => _setting.KeySetting.DefaultLevel; set => _setting.KeySetting.DefaultLevel = value; }

    /// <summary>
    /// 
    /// </summary>
    public string Key { get => _setting.KeySetting.Key; set => _setting.KeySetting.Key = value; }

    /// <summary>
    /// 
    /// </summary>
    public string RawValue { get => _setting.RawValue; set => _setting.RawValue = value; }
    /// <summary>
    /// 
    /// </summary>
    public string DefaultRawValue { get => _setting.DefaultSetting.RawValue; set => _setting.DefaultSetting.RawValue = value; }

    /// <summary>
    /// 
    /// </summary>
    public T Value { get => _setting.GetValueCache<T>(); set => _setting.SetValue(value); }
    /// <summary>
    /// 
    /// </summary>
    public T DefaultValue { get => _setting.DefaultSetting.GetValueCache<T>(); set => _setting.DefaultSetting.SetValue(value); }

    /// <summary>
    /// 
    /// </summary>
    public bool IsChanged => _setting.IsChanged;

    /// <summary>
    /// 
    /// </summary>
    public void AcceptChange() => _setting.AcceptChange();
    /// <summary>
    /// 
    /// </summary>
    public void ResetChange() => _setting.ResetChange();
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="setting"></param>
    public CacheValueSetting(CacheValueSetting setting)
    {
      this._setting = setting;
    }
  }
}
