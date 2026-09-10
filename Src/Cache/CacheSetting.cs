// Copyright (c) 2013-2026 Cem Dervis, MIT License.
// https://sharpconfig.org

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SharpConfig
{
  /// <summary>
  /// Represents a setting in a <see cref="Configuration"/>.
  /// Settings are always stored in a <see cref="Section"/>.
  /// </summary>
  public class CacheSetting : ConfigurationElement
  {
    private static readonly int _flagInt16;
    private static readonly int _flagInt32;
    private static readonly int _flagInt64;
    private static readonly int _flagUInt16;
    private static readonly int _flagUInt32;
    private static readonly int _flagUInt64;
    private static readonly int _flagBool;
    private static readonly int _flagByte;
    private static readonly int _flagSByte;
    private static readonly int _flagChar;
    private static readonly int _flagFloat;
    private static readonly int _flagDouble;
    private static readonly int _flagDecimal;
    private static readonly int _flagDateTime;
    private static readonly int _flagString;
    private static readonly int _flagRaw;

    static CacheSetting()
    {
      _flagInt16 = BitVector32.CreateMask();
      _flagInt32 = BitVector32.CreateMask(_flagInt16);
      _flagInt64 = BitVector32.CreateMask(_flagInt32);
      _flagUInt16 = BitVector32.CreateMask(_flagInt64);
      _flagUInt32 = BitVector32.CreateMask(_flagUInt16);
      _flagUInt64 = BitVector32.CreateMask(_flagUInt32);
      _flagBool = BitVector32.CreateMask(_flagUInt64);
      _flagByte = BitVector32.CreateMask(_flagBool);
      _flagSByte = BitVector32.CreateMask(_flagByte);
      _flagChar = BitVector32.CreateMask(_flagSByte);
      _flagFloat = BitVector32.CreateMask(_flagChar);
      _flagDouble = BitVector32.CreateMask(_flagFloat);
      _flagDecimal = BitVector32.CreateMask(_flagDouble);
      _flagDateTime = BitVector32.CreateMask(_flagDecimal);
      _flagString = BitVector32.CreateMask(_flagDateTime);
      _flagRaw = BitVector32.CreateMask(_flagString);
    }

    private BitVector32 _flag;
    private Int16 _cacheInt16;
    private Int32 _cacheInt32;
    private Int64 _cacheInt64;
    private UInt16 _cacheUInt16;
    private UInt32 _cacheUInt32;
    private UInt64 _cacheUInt64;
    private bool _cacheBool;
    private byte _cacheByte;
    private sbyte _cacheSByte;
    private char _cacheChar;
    private float _cacheFloat;
    private double _cacheDouble;
    private decimal _cacheDecimal;
    private DateTime _cacheDateTime;
    private string _cacheString = string.Empty;
    private string _cacheRaw = string.Empty;

    private int _cachedArraySize;
    private bool _shouldCalculateArraySize;
    private char _cachedArrayElementSeparator;

    /// <summary>
    /// Initializes a new instance of the <see cref="Setting"/> class.
    /// </summary>
    public CacheSetting(string name) : this(name, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Setting"/> class.
    /// </summary>
    ///
    /// <param name="name"> The name of the setting.</param>
    /// <param name="value">The value of the setting.</param>
    public CacheSetting(string name, object value) : base(name)
    {
      SetValue(value);
      _cachedArrayElementSeparator = Configuration.ArrayElementSeparator;
    }

    /// <summary>
    /// Gets a value indicating whether this setting's value is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrEmpty(RawValue);

    /// <summary>
    /// Gets or sets the raw value of this setting.
    /// </summary>
    public string RawValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="string"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public string StringValue
    {
      // get
      // {
      //   if (Configuration.OutputRawStringValues)
      //   {
      //     return GetValue<string>();
      //   }
      // 
      //   // If the value is a multiline value, we don't want to trim quotes,
      //   // as they are part of the verbatim content.
      //   if (RawValue.StartsWith("[[") && RawValue.EndsWith("]]"))
      //   {
      //     return GetValue<string>();
      //   }
      // 
      //   return GetValue<string>().Trim('\"');
      // }
      get => GetValueString();
      set => SetValue(Configuration.OutputRawStringValues ? value : value.Trim('\"'));
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="string"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public string[] StringValueArray
    {
      get => GetValueArray<string>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public UInt16 UInt16Value
    {
      get => GetValueUInt16();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public UInt32 UInt32Value
    {
      get => GetValueUInt32();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public UInt64 UInt64Value
    {
      get => GetValueUInt64();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public Int16 Int16Value
    {
      get => GetValueInt16();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public Int64 Int64Value
    {
      get => GetValueInt64();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public int IntValue
    {
      get => GetValueInt32();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as an <see cref="int"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public int[] IntValueArray
    {
      get => GetValueArray<int>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="float"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public float FloatValue
    {
      get => GetValueFloat();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="float"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public float[] FloatValueArray
    {
      get => GetValueArray<float>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="double"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public double DoubleValue
    {
      get => GetValueDouble();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="double"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public double[] DoubleValueArray
    {
      get => GetValueArray<double>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="decimal"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public decimal DecimalValue
    {
      get => GetValueDecimal();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="decimal"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public decimal[] DecimalValueArray
    {
      get => GetValueArray<decimal>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="bool"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public bool BoolValue
    {
      get => GetValueBool();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="bool"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public bool[] BoolValueArray
    {
      get => GetValueArray<bool>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this settings as a <see cref="DateTime"/>.
    /// Note: this is a shortcut to GetValue and SetValue.
    /// </summary>
    public DateTime DateTimeValue
    {
      get => GetValueDateTime();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="DateTime"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public DateTime[] DateTimeValueArray
    {
      get => GetValueArray<DateTime>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="byte"/>.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public byte ByteValue
    {
      get => GetValueByte();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="byte"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public byte[] ByteValueArray
    {
      get => GetValueArray<byte>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="sbyte"/>.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public sbyte SByteValue
    {
      get => GetValueSByte();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="sbyte"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public sbyte[] SByteValueArray
    {
      get => GetValueArray<sbyte>() ?? throw new InvalidOperationException("Setting is not an array.");
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="char"/>.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public char CharValue
    {
      get => GetValueChar();
      set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the value of this setting as a <see cref="char"/> array.
    /// Note: this is a shortcut to GetValueArray and SetValue.
    /// </summary>
    public char[] CharValueArray
    {
      get
      {
        // Decode the bytes back to chars.
        byte[] bytes = ByteValueArray;
        return Encoding.UTF8.GetChars(bytes);
      }
      set
      {
        if (value != null)
        {
          // Encode the chars to bytes, because writing raw chars such as
          // '\0' can mess up the configuration file and the parser.
          ByteValueArray = Encoding.UTF8.GetBytes(value);
        }
        else
        {
          SetEmptyValue();
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether this setting is an array.
    /// </summary>
    public bool IsArray => (ArraySize >= 0);

    /// <summary>
    /// Gets the size of the array that this setting represents.
    /// If this setting is not an array, -1 is returned.
    /// </summary>
    public int ArraySize
    {
      get
      {
        // If the user changed the array element separator during the lifetime
        // of this setting, we have to recalculate the array size.
        if (_cachedArrayElementSeparator != Configuration.ArrayElementSeparator)
        {
          _cachedArrayElementSeparator = Configuration.ArrayElementSeparator;
          _shouldCalculateArraySize = true;
        }

        if (_shouldCalculateArraySize)
        {
          _cachedArraySize = CalculateArraySize();
          _shouldCalculateArraySize = false;
        }

        return _cachedArraySize;
      }
    }

    private int CalculateArraySize()
    {
      var size = 0;
      var enumerator = new SettingArrayEnumerator(RawValue, false);

      while (enumerator.Next())
      {
        ++size;
      }

      return (enumerator.IsValid ? size : -1);
    }

    /// <summary>
    /// Gets this setting's value as a specific type.
    /// </summary>
    ///
    /// <param name="type">The type of the object to retrieve.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidOperationException">When <paramref name="type"/> is an array type.</exception>
    /// <exception cref="InvalidOperationException">When the setting represents an array.</exception>
    public object GetValue(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      if (type.IsArray)
      {
        throw new InvalidOperationException(
            "To obtain an array value, use GetValueArray() instead of GetValue().");
      }

      if (IsArray)
      {
        throw new InvalidOperationException(
            "The setting represents an array. Use GetValueArray() to obtain its value.");
      }

      return CreateObjectFromString(RawValue, type)!;
    }

    /// <summary>
    /// Gets this setting's value as an array of a specific type.
    /// Note: this only works if the setting represents an array. If it is not, then null is returned.
    /// </summary>
    /// <param name="elementType">
    ///     The type of elements in the array. All values in the array are going to be converted to objects of
    ///     this type. If the conversion of an element fails, an exception is thrown.
    /// </param>
    /// <returns>The values of this setting as an array, or null if the setting is not an array.</returns>
    public object[]? GetValueArray(Type elementType)
    {
      if (elementType.IsArray)
      {
        throw CreateJaggedArraysNotSupportedEx(elementType);
      }

      var myArraySize = this.ArraySize;

      if (myArraySize < 0)
      {
        return null;
      }

      var values = new object[myArraySize];

      if (myArraySize > 0)
      {
        var enumerator = new SettingArrayEnumerator(RawValue, true);
        var elementIndex = 0;

        while (enumerator.Next())
        {
          values[elementIndex] = CreateObjectFromString(enumerator.Current!, elementType)!;
          ++elementIndex;
        }
      }

      return values;
    }

    private Int16 GetValueInt16()
    {
      if (!this._flag[_flagInt16])
      {
        _cacheInt16 = GetValue<Int16>();
        this._flag[_flagInt16] = true;
      }
      return _cacheInt16;
    }

    private Int32 GetValueInt32()
    {
      if (!this._flag[_flagInt32])
      {
        _cacheInt32 = GetValue<Int32>();
        this._flag[_flagInt32] = true;
      }
      return _cacheInt32;
    }

    private Int64 GetValueInt64()
    {
      if (!this._flag[_flagInt64])
      {
        _cacheInt64 = GetValue<Int64>();
        this._flag[_flagInt64] = true;
      }
      return _cacheInt64;
    }

    private UInt16 GetValueUInt16()
    {
      if (!this._flag[_flagUInt16])
      {
        _cacheUInt16 = GetValue<UInt16>();
        this._flag[_flagUInt16] = true;
      }
      return _cacheUInt16;
    }

    private UInt32 GetValueUInt32()
    {
      if (!this._flag[_flagUInt32])
      {
        _cacheUInt32 = GetValue<UInt32>();
        this._flag[_flagUInt32] = true;
      }
      return _cacheUInt32;
    }

    private UInt64 GetValueUInt64()
    {
      if (!this._flag[_flagUInt64])
      {
        _cacheUInt64 = GetValue<UInt64>();
        this._flag[_flagUInt64] = true;
      }
      return _cacheUInt64;
    }

    private bool GetValueBool()
    {
      if (!this._flag[_flagBool])
      {
        _cacheBool = GetValue<bool>();
        this._flag[_flagBool] = true;
      }
      return _cacheBool;
    }

    private byte GetValueByte()
    {
      if (!this._flag[_flagByte])
      {
        _cacheByte = GetValue<byte>();
        this._flag[_flagByte] = true;
      }
      return _cacheByte;
    }

    private sbyte GetValueSByte()
    {
      if (!this._flag[_flagSByte])
      {
        _cacheSByte = GetValue<sbyte>();
        this._flag[_flagSByte] = true;
      }
      return _cacheSByte;
    }

    private char GetValueChar()
    {
      if (!this._flag[_flagChar])
      {
        _cacheChar = GetValue<char>();
        this._flag[_flagChar] = true;
      }
      return _cacheChar;
    }

    private float GetValueFloat()
    {
      if (!this._flag[_flagFloat])
      {
        _cacheFloat = GetValue<float>();
        this._flag[_flagFloat] = true;
      }
      return _cacheFloat;
    }

    private double GetValueDouble()
    {
      if (!this._flag[_flagDouble])
      {
        _cacheDouble = GetValue<double>();
        this._flag[_flagDouble] = true;
      }
      return _cacheDouble;
    }

    private decimal GetValueDecimal()
    {
      if (!this._flag[_flagDecimal])
      {
        _cacheDecimal = GetValue<decimal>();
        this._flag[_flagDecimal] = true;
      }
      return _cacheDecimal;
    }

    private DateTime GetValueDateTime()
    {
      if (!this._flag[_flagDateTime])
      {
        _cacheDateTime = GetValue<DateTime>();
        this._flag[_flagDateTime] = true;
      }
      return _cacheDateTime;
    }

    private string GetValueString()
    {
      if (Configuration.OutputRawStringValues)
      {
        if (!this._flag[_flagRaw])
        {
          _cacheRaw = GetValue<string>();
          this._flag[_flagRaw] = true;
        }
        return _cacheRaw;
      }

      if (!this._flag[_flagString])
      {
        // If the value is a multiline value, we don't want to trim quotes,
        // as they are part of the verbatim content.
        if (RawValue.StartsWith("[[") && RawValue.EndsWith("]]"))
        {
          _cacheString = GetValue<string>();
        }
        else
        {
          _cacheString = GetValue<string>().Trim('\"');
        }
        this._flag[_flagString] = true;
      }

      return _cacheString;
    }

    /// <summary>
    /// Gets this setting's value as a specific type.
    /// </summary>
    ///
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    ///
    /// <exception cref="InvalidOperationException">When <typeparamref name="T"/> is an array
    /// type.</exception> <exception cref="InvalidOperationException">When the setting represents an
    /// array.</exception>
    public T GetValue<T>()
    {
      var type = typeof(T);

      if (type.IsArray)
      {
        throw new InvalidOperationException(
            "To obtain an array value, use GetValueArray() instead of GetValue().");
      }

      if (IsArray)
      {
        throw new InvalidOperationException(
            "The setting represents an array. Use GetValueArray() to obtain its value.");
      }

      return (T)CreateObjectFromString(RawValue, type)!;
    }

    /// <summary>
    /// Gets this setting's value as an array of a specific type.
    /// Note: this only works if the setting represents an array. If it is not, then null is returned.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of elements in the array. All values in the array are going to be converted to objects of
    ///     this type. If the conversion of an element fails, an exception is thrown.
    /// </typeparam>
    /// <returns>The values of this setting as an array, or null if the setting is not an array.</returns>
    public T[]? GetValueArray<T>()
    {
      var type = typeof(T);

      if (type.IsArray)
      {
        throw CreateJaggedArraysNotSupportedEx(type);
      }

      var myArraySize = ArraySize;

      if (myArraySize < 0)
      {
        return null;
      }

      var values = new T[myArraySize];

      if (myArraySize > 0)
      {
        var enumerator = new SettingArrayEnumerator(RawValue, true);
        var elementIndex = 0;

        while (enumerator.Next())
        {
          values[elementIndex] = (T)CreateObjectFromString(enumerator.Current!, type)!;
          ++elementIndex;
        }
      }

      return values;
    }

    /// <summary>
    /// Gets this setting's value as a specific type, or a specified default value
    /// if casting the setting to the type fails.
    /// </summary>
    /// <param name="defaultValue">
    /// Default value if casting the setting to the specified type fails.
    /// </param>
    /// <param name="setDefault">
    /// If true, and casting the setting to the specified type fails, <paramref name="defaultValue"/> is set
    /// as this setting's new value.
    /// </param>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    public T GetValueOrDefault<T>(T defaultValue, bool setDefault = false)
    {
      var type = typeof(T);

      if (type.IsArray)
      {
        throw new InvalidOperationException("GetValueOrDefault<T> cannot be used with arrays.");
      }

      if (IsArray)
      {
        throw new InvalidOperationException(
            "The setting represents an array. Use GetValueArray() to obtain its value.");
      }

      var result = CreateObjectFromString(RawValue, type, true);

      if (result != null)
      {
        return (T)result;
      }

      if (setDefault)
      {
        SetValue(defaultValue);
      }

      return defaultValue;
    }

    // Converts the value of a single element to a desired type.
    private static object? CreateObjectFromString(string value, Type dstType, bool tryConvert = false)
    {
      var underlyingType = Nullable.GetUnderlyingType(dstType);

      if (underlyingType != null)
      {
        if (string.IsNullOrEmpty(value))
        {
          return null; // Returns Nullable<T>().
        }

        // Otherwise, continue with our conversion using
        // the underlying type of the nullable.
        dstType = underlyingType;
      }

      // If the value is a multiline value, strip the delimiters.
      if (value.StartsWith("[[") && value.EndsWith("]]"))
      {
        value = value.Substring(2, value.Length - 4);

        // Strip leading and trailing newlines.
        if (value.StartsWith("\r\n"))
        {
          value = value.Substring(2);
        }
        else if (value.StartsWith("\n"))
        {
          value = value.Substring(1);
        }

        if (value.EndsWith("\r\n"))
        {
          value = value.Substring(0, value.Length - 2);
        }
        else if (value.EndsWith("\n"))
        {
          value = value.Substring(0, value.Length - 1);
        }

        if (dstType == typeof(string))
        {
          return value;
        }
      }

      var converter = Configuration.FindTypeStringConverter(dstType);
      var obj = converter.TryConvertFromString(value, dstType);

      if (obj == null && !tryConvert)
      {
        throw SettingValueCastException.Create(value, dstType, null);
      }

      return obj;
    }

    /// <summary>
    /// Sets the value of this setting via an object.
    /// </summary>
    ///
    /// <param name="value">The value to set. Can be null to set an empty value.</param>
    public void SetValue(object? value)
    {
      if (value == null)
      {
        SetEmptyValue();
        return;
      }

      var type = value.GetType();

      if (type.IsArray)
      {
        var elementType = type.GetElementType();

        if (elementType != null && elementType.IsArray)
        {
          throw CreateJaggedArraysNotSupportedEx(elementType);
        }

        if (value is Array values)
        {
          var strings = new string[values.Length];

          for (int i = 0; i < values.Length; i++)
          {
            var elemValue = values.GetValue(i)!;
            var converter = Configuration.FindTypeStringConverter(elemValue.GetType());

            strings[i] = GetValueForOutput(converter.ConvertToString(elemValue));
          }

          RawValue = $"{{{string.Join(Configuration.ArrayElementSeparator.ToString(), strings)}}}";
          _flag[-1] = false;
          _cachedArraySize = values.Length;
        }

        _shouldCalculateArraySize = false;
      }
      else
      {
        var converter = Configuration.FindTypeStringConverter(type);
        RawValue = converter.ConvertToString(value);
        _flag[-1] = false;
        _shouldCalculateArraySize = true;
      }
    }

    private void SetEmptyValue()
    {
      RawValue = string.Empty;
      _flag[-1] = false;
      _cachedArraySize = -1;
      _shouldCalculateArraySize = false;
    }

    private static string GetValueForOutput(string rawValue)
    {
      if (Configuration.OutputRawStringValues)
      {
        return rawValue;
      }

      if (rawValue.StartsWith("{") && rawValue.EndsWith("}"))
      {
        return rawValue;
      }

      if (rawValue.StartsWith("\"") && rawValue.EndsWith("\""))
      {
        return rawValue;
      }

      if (rawValue.StartsWith("[[") && rawValue.EndsWith("]]"))
      {
        return rawValue;
      }

      bool isAnyCommentCharInRawValue() => rawValue.Any(c => Configuration.ValidCommentChars.Contains(c));

      if (rawValue.IndexOf(" ", StringComparison.Ordinal) >= 0 ||
          (isAnyCommentCharInRawValue() && !Configuration.IgnoreInlineComments))
      {
        rawValue = "\"" + rawValue + "\"";
      }

      return rawValue;
    }

    /// <summary>
    /// Gets the element's expression as a string.
    /// An example for a section would be "[Section]".
    /// </summary>
    /// <returns>The element's expression as a string.</returns>
    protected override string GetStringExpression()
    {
      return Configuration.SpaceBetweenEquals ? $"{Name} = {GetValueForOutput(RawValue)}"
                                              : $"{Name}={GetValueForOutput(RawValue)}";
    }

    private static ArgumentException CreateJaggedArraysNotSupportedEx(Type type)
    {
      // Determine the underlying element type.
      var elementType = type.GetElementType();

      while (elementType != null && elementType.IsArray)
      {
        elementType = elementType.GetElementType();
      }

      throw new ArgumentException(
          $"Jagged arrays are not supported. The type you have specified is '{type.Name}', but '{elementType?.Name}' was expected.");
    }
  }
}

