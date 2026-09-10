// Copyright (c) 2013-2026 Cem Dervis, MIT License.
// https://sharpconfig.org

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpConfig
{
  /// <summary>
  /// Represents a configuration.
  /// Configurations contain one or multiple sections
  /// that in turn can contain one or multiple settings.
  /// The <see cref="Configuration"/> class is designed
  /// to work with classic configuration formats such as
  /// .ini and .cfg, but is not limited to these.
  /// </summary>
  public partial class Configuration
  {
    private static CultureInfo? s_cultureInfo;
    private static char s_preferredCommentChar;
    private static char s_arrayElementSeparator;
    private static readonly Dictionary<Type, ITypeStringConverter> s_typeStringConverters;

    static Configuration()
    {
      ResetOptions();

      FallbackConverter = new FallbackStringConverter();

      // Add all stock converters.
      s_typeStringConverters = new Dictionary<Type, ITypeStringConverter> {
        { typeof(bool), new BoolStringConverter() },
        { typeof(byte), new ByteStringConverter() },
        { typeof(char), new CharStringConverter() },
        { typeof(DateTime), new DateTimeStringConverter() },
        { typeof(decimal), new DecimalStringConverter() },
        { typeof(double), new DoubleStringConverter() },
        { typeof(Enum), new EnumStringConverter() },
        { typeof(short), new Int16StringConverter() },
        { typeof(int), new Int32StringConverter() },
        { typeof(long), new Int64StringConverter() },
        { typeof(sbyte), new SByteStringConverter() },
        { typeof(float), new SingleStringConverter() },
        { typeof(string), new StringStringConverter() },
        { typeof(ushort), new UInt16StringConverter() },
        { typeof(uint), new UInt32StringConverter() },
        { typeof(ulong), new UInt64StringConverter() },
      };
    }

    /// <summary>
    /// Registers a type converter to be used for setting value conversions.
    /// </summary>
    /// <param name="converter">The converter to register.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="converter"/> is null.</exception>
    /// <exception cref="InvalidOperationException">When a converter for the converter's type is already
    /// registered.</exception>
    public static void RegisterTypeStringConverter(ITypeStringConverter converter)
    {
      if (converter == null)
      {
        throw new ArgumentNullException(nameof(converter));
      }

      var type = converter.ConvertibleType;

      if (type == null)
      {
        throw new ArgumentException("The converter's ConvertibleType cannot be null.", nameof(converter));
      }

      if (s_typeStringConverters.ContainsKey(type))
      {
        throw new InvalidOperationException($"A converter for type '{type.FullName}' is already registered.");
      }

      s_typeStringConverters.Add(type, converter);
    }

    /// <summary>
    /// Deregisters a type converter from setting value conversion.
    /// </summary>
    /// <param name="type">The type whose associated converter to deregister.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidOperationException">When no converter is registered for <paramref
    /// name="type"/>.</exception>
    public static void DeregisterTypeStringConverter(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      if (!s_typeStringConverters.ContainsKey(type))
      {
        throw new InvalidOperationException($"No converter is registered for type '{type.FullName}'.");
      }

      s_typeStringConverters.Remove(type);
    }

    /// <summary>
    /// Looks up a registered type converter for a specific type.
    /// </summary>
    /// <param name="type">The type whose converter to look up.</param>
    /// <returns>A reference to the registered converter, if found; null otherwise.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="type"/>is null.</exception>
    public static ITypeStringConverter FindTypeStringConverter(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      if (type.IsEnum)
      {
        type = typeof(Enum);
      }

      if (!s_typeStringConverters.TryGetValue(type, out ITypeStringConverter? converter))
      {
        converter = FallbackConverter;
      }

      return converter;
    }

    internal static ITypeStringConverter FallbackConverter { get; private set; }

    /// <summary>
    /// Gets or sets the CultureInfo that is used for value conversion in SharpConfig.
    /// The default value is CultureInfo.InvariantCulture.
    /// </summary>
    ///
    /// <exception cref="ArgumentNullException">When a null reference is set.</exception>
    public static CultureInfo CultureInfo
    {
      get => s_cultureInfo!;
      set => s_cultureInfo = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the array that contains all valid comment delimiting characters.
    /// The default value is { '#', ';' }.
    /// </summary>
    public static HashSet<char> ValidCommentChars { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the preferred comment char when saving configurations.
    /// The default value is '#'.
    /// </summary>
    ///
    /// <exception cref="ArgumentException">When an invalid character is set.</exception>
    public static char PreferredCommentChar
    {
      get => s_preferredCommentChar;
      set
      {
        if (!ValidCommentChars.Contains(value))
        {
          throw new ArgumentException($"The specified char '{value}' is not allowed as a comment char.");
        }

        s_preferredCommentChar = value;
      }
    }

    /// <summary>
    /// Gets or sets the array element separator character for settings.
    /// The default value is ','.
    /// NOTE: remember that after you change this value while <see cref="Setting"/> instances exist,
    /// to expect their ArraySize and other array-related values to return different values.
    /// </summary>
    ///
    /// <exception cref="ArgumentException">When a zero-character ('\0') is set.</exception>
    public static char ArrayElementSeparator
    {
      get => s_arrayElementSeparator;
      set
      {
        if (value == '\0')
        {
          throw new ArgumentException("Zero-character is not allowed.");
        }

        s_arrayElementSeparator = value;
      }
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
    /// Gets or sets a value indicating whether inline-comments
    /// should be ignored when parsing a configuration.
    /// </summary>
    public static bool IgnoreInlineComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether pre-comments
    /// should be ignored when parsing a configuration.
    /// </summary>
    public static bool IgnorePreComments { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether space between
    /// equals should be added when creating a configuration.
    /// </summary>
    public static bool SpaceBetweenEquals { get; set; }

    /// <summary>
    /// Resets all global configuration options to their defaults.
    /// </summary>
    public static void ResetOptions()
    {
      // For now, clone the invariant culture so that the
      // deprecated DateTimeFormat/NumberFormat properties still work,
      // but without modifying the real invariant culture instance.
      s_cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();

      ValidCommentChars = new HashSet<char> { '#', ';' };
      s_preferredCommentChar = '#';
      s_arrayElementSeparator = ',';
      OutputRawStringValues = false;
      IgnoreInlineComments = false;
      IgnorePreComments = false;
      SpaceBetweenEquals = false;
    }
    
  }
}
