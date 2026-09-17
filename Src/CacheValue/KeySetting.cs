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
  public class KeySetting
  {
    /// <summary>
    /// 
    /// </summary>
    public string Code { get; }
    /// <summary>
    /// 
    /// </summary>
    public string Ver { get; }
    /// <summary>
    /// 
    /// </summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public string Unit { get; set; } = string.Empty;
    /// <summary>
    /// 
    /// </summary>
    public int RawLevel { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int DefaultLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Key { get; set; } = string.Empty;


    // public T DefaultValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string VerCode => this.Ver == null ? this.Code : $"{this.Ver ?? string.Empty}:{this.Code}";

    /// <summary>
    /// 
    /// </summary>
    public int Level => this.RawLevel == 0 ? this.DefaultLevel : this.RawLevel;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="ver"></param>
    /// <returns></returns>
    public static string GetVerCode(string code, string ver = "0")
    {
      return ver == null ? code : $"{ver ?? string.Empty}:{code}";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="ver"></param>
    public KeySetting(string code, string ver = "0")
    {
      this.Code = code;
      this.Ver = ver;
    }
  }
}
