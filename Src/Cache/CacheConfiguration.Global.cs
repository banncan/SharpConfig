using System;
using System.Collections.Generic;
using System.Text;

namespace SharpConfig
{
  public partial class CacheConfiguration
  {
    static CacheConfiguration()
    {
      ResetOptions();
    }

    /// <summary>
    /// Gets or sets a value indicating whether string values are written
    /// without quotes, but including everything in between.
    /// Example:
    /// The following setting value
    ///     MySetting=" Example value"
    /// is written to a file in the following manner
    ///     MySetting= Example value
    /// </summary>
    public static bool OutputRawStringValues { get; set; }

    /// <summary>
    /// Resets all global configuration options to their defaults.
    /// </summary>
    public static void ResetOptions()
    {
      OutputRawStringValues = true;
    }


  }
}
