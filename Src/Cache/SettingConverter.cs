using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpConfig
{
  /// <summary>
  /// 
  /// </summary>
  public static class SettingConverter
  {
    /// <summary>
    /// Gets this setting's value as a specific type.
    /// </summary>
    ///
    /// <param name="rawValue">The raw value.</param>
    /// 
    /// <param name="type">The type of the object to retrieve.</param>
    /// 
    /// <param name="isArray">The array flag.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="type"/> is null.</exception>
    /// <exception cref="InvalidOperationException">When <paramref name="type"/> is an array type.</exception>
    /// <exception cref="InvalidOperationException">When the setting represents an array.</exception>
    public static object GetValueFromString(this string rawValue, Type type, bool isArray)
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

      if (isArray)
      {
        throw new InvalidOperationException(
            "The setting represents an array. Use GetValueArray() to obtain its value.");
      }

      return CreateObjectFromString(rawValue, type)!;
    }

    /// <summary>
    /// Gets this setting's value as a T type.
    /// </summary>
    /// 
    /// <param name="rawValue">The raw value.</param>
    /// 
    /// <param name="isArray">The array flag.</param>
    /// 
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T GetValueFromString<T>(this string rawValue, bool isArray)
    {
      var type = typeof(T);

      if (type.IsArray)
      {
        throw new InvalidOperationException(
            "To obtain an array value, use GetValueArray() instead of GetValue().");
      }

      if (isArray)
      {
        throw new InvalidOperationException(
            "The setting represents an array. Use GetValueArray() to obtain its value.");
      }

      return (T)CreateObjectFromString(rawValue, type)!;
    }


    /// <summary>
    /// Sets the value of this setting via an object.
    /// </summary>
    ///
    /// <param name="value">The value to set. Can be null to set an empty value.</param>
    public static string GetStringFromValue(object? value)
    {
      // if (value == null)
      // {
      //   SetEmptyValue();
      //   return;
      // }

      string rawValue = string.Empty;

      if (value == null) return rawValue;

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

          rawValue = $"{{{string.Join(Configuration.ArrayElementSeparator.ToString(), strings)}}}";
          // _cachedArraySize = values.Length;
        }

        // shouldCalculateArraySize = false;
      }
      else
      {
        var converter = Configuration.FindTypeStringConverter(type);
        rawValue = converter.ConvertToString(value);
        // shouldCalculateArraySize = true;
      }

      return rawValue;
    }


    /// <summary>
    /// Converts the value of a single element to a desired type.
    /// </summary>
    /// <param name="rawValue">raw</param>
    /// <param name="dstType"></param>
    /// <param name="tryConvert"></param>
    /// <returns></returns>
    public static object? CreateObjectFromString(this string rawValue, Type dstType, bool tryConvert = false)
    {
      var underlyingType = Nullable.GetUnderlyingType(dstType);

      if (underlyingType != null)
      {
        if (string.IsNullOrEmpty(rawValue))
        {
          return null; // Returns Nullable<T>().
        }

        // Otherwise, continue with our conversion using
        // the underlying type of the nullable.
        dstType = underlyingType;
      }

      // If the value is a multiline value, strip the delimiters.
      if (rawValue.StartsWith("[[") && rawValue.EndsWith("]]"))
      {
        rawValue = rawValue.Substring(2, rawValue.Length - 4);

        // Strip leading and trailing newlines.
        if (rawValue.StartsWith("\r\n"))
        {
          rawValue = rawValue.Substring(2);
        }
        else if (rawValue.StartsWith("\n"))
        {
          rawValue = rawValue.Substring(1);
        }

        if (rawValue.EndsWith("\r\n"))
        {
          rawValue = rawValue.Substring(0, rawValue.Length - 2);
        }
        else if (rawValue.EndsWith("\n"))
        {
          rawValue = rawValue.Substring(0, rawValue.Length - 1);
        }

        if (dstType == typeof(string))
        {
          return rawValue;
        }
      }

      var converter = Configuration.FindTypeStringConverter(dstType);
      var obj = converter.TryConvertFromString(rawValue, dstType);

      if (obj == null && !tryConvert)
      {
        // throw SettingValueCastException.Create(rawValue, dstType, null);
        string msg = $"Failed to convert value '{rawValue}' to type {dstType.FullName}.";
        throw new Exception(msg);
      }

      return obj;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rawValue"></param>
    /// <returns></returns>
    public static string GetValueForOutput(string rawValue)
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
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static ArgumentException CreateJaggedArraysNotSupportedEx(Type type)
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

