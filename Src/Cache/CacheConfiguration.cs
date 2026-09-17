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
  public partial class CacheConfiguration : IEnumerable<CacheSection>
  {
    internal readonly Dictionary<string, int> _indexs;
    internal readonly List<CacheSection> _sections;

    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    public CacheConfiguration()
    {
      _indexs = new Dictionary<string, int>();
      _sections = new List<CacheSection>();
    }

    /// <summary>
    /// Gets an enumerator that iterates through the configuration.
    /// </summary>
    public IEnumerator<CacheSection> GetEnumerator() => _sections.GetEnumerator();

    /// <summary>
    /// Gets an enumerator that iterates through the configuration.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Adds a section to the configuration.
    /// </summary>
    /// <param name="section">The section to add.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="section"/> is null.</exception>
    /// <exception cref="ArgumentException">When the section already exists in the configuration.</exception>
    public void Add(CacheSection section)
    {
      if (section == null)
      {
        throw new ArgumentNullException(nameof(section));
      }

      if (Contains(section))
      {
        throw new ArgumentException("The specified section already exists in the configuration.");
      }

      _sections.Add(section);
    }

    /// <summary>
    /// Adds a section with a specific name to the configuration.
    /// </summary>
    /// <param name="sectionName">The name of the section to add.</param>
    /// <returns>The added section.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="sectionName"/> is null or
    /// empty.</exception>
    public CacheSection Add(string sectionName)
    {
      var section = new CacheSection(sectionName);
      Add(section);
      return section;
    }

    /// <summary>
    /// Removes a section from the configuration by its name.
    /// If there are multiple sections with the same name, only the first section is removed.
    /// To remove all sections that have the name name, use the RemoveAllNamed() method instead.
    /// </summary>
    /// <param name="sectionName">
    /// The section name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    /// <returns>True if a section with the specified name was removed; false otherwise.</returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="sectionName"/> is null or
    /// empty.</exception>
    public bool Remove(string sectionName)
    {
      if (string.IsNullOrEmpty(sectionName))
      {
        throw new ArgumentNullException(nameof(sectionName));
      }

      var section = FindSection(sectionName);
      return section != null && Remove(section);
    }

    /// <summary>
    /// Removes a section from the configuration.
    /// </summary>
    /// <param name="section">The section to remove.</param>
    /// <returns>True if the section was removed; false otherwise.</returns>
    public bool Remove(CacheSection section) => _sections.Remove(section);

    /// <summary>
    /// Removes all sections that have a specific name.
    /// </summary>
    /// <param name="sectionName">
    /// The section name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="sectionName"/> is null or
    /// empty.</exception>
    public void RemoveAllNamed(string sectionName)
    {
      if (string.IsNullOrEmpty(sectionName))
      {
        throw new ArgumentNullException(nameof(sectionName));
      }

      while (Remove(sectionName))
      {
        // Nothing to do.
      }
    }

    /// <summary>
    /// Clears the configuration of all sections.
    /// </summary>
    public void Clear() => _sections.Clear();

    /// <summary>
    /// Determines whether a specified section is contained in the configuration.
    /// </summary>
    /// <param name="section">The section to check for containment.</param>
    /// <returns>True if the section is contained in the configuration; false otherwise.</returns>
    public bool Contains(CacheSection section) => _sections.Contains(section);

    /// <summary>
    /// Determines whether a specifically named section is contained in the configuration.
    /// </summary>
    /// <param name="sectionName">
    /// The section name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    /// <returns>True if the section is contained in the configuration; false otherwise.</returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="sectionName"/> is null or
    /// empty.</exception>
    public bool Contains(string sectionName)
    {
      return string.IsNullOrEmpty(sectionName) ? throw new ArgumentNullException(nameof(sectionName))
                                               : FindSection(sectionName) != null;
    }

    /// <summary>
    /// Determines whether a specifically named section is contained in the configuration,
    /// and whether that section in turn contains a specifically named setting.
    /// </summary>
    /// <param name="sectionName">
    /// The section name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    /// <param name="settingName">
    /// The setting name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    /// <returns>True if the section and the respective setting was found; false otherwise.</returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="sectionName"/> or <paramref
    /// name="settingName"/> is null or empty.</exception>
    public bool Contains(string sectionName, string settingName)
    {
      if (string.IsNullOrEmpty(sectionName))
      {
        throw new ArgumentNullException(nameof(sectionName));
      }

      if (string.IsNullOrEmpty(settingName))
      {
        throw new ArgumentNullException(nameof(settingName));
      }

      var section = FindSection(sectionName);

      return section != null && section.Contains(settingName);
    }

    /// <summary>
    /// Loads a CacheConfiguration from a file.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    /// <param name="encoding">The encoding applied to the contents of the file. Specify null to auto-detect
    /// the encoding.</param>
    ///
    /// <returns>
    /// The loaded <see cref="CacheConfiguration"/> object.
    /// </returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="filename"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">When the specified configuration file is not
    /// found.</exception>
    public static CacheConfiguration LoadFromFile(string filename, Encoding? encoding = null)
    {
      if (string.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException(nameof(filename));
      }

      if (!File.Exists(filename))
      {
        throw new FileNotFoundException("CacheConfiguration file not found.", filename);
      }

      return LoadFromString(
          encoding == null ? File.ReadAllText(filename) : File.ReadAllText(filename, encoding));
    }

    /// <summary>
    /// Loads a configuration from a text stream.
    /// </summary>
    ///
    /// <param name="stream">   The text stream to load the configuration from.</param>
    /// <param name="encoding"> The encoding applied to the contents of the stream. Specify null to
    /// auto-detect the encoding.</param>
    ///
    /// <returns>
    /// The loaded <see cref="CacheConfiguration"/> object.
    /// </returns>
    ///
    /// <remarks>
    /// This method does not dispose the provided <paramref name="stream"/>.
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is null.</exception>
    public static CacheConfiguration LoadFromStream(Stream stream, Encoding? encoding = null)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      using (var reader =
          encoding == null
              ? new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true)
              : new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
      {
        return LoadFromString(reader.ReadToEnd());
      }
    }

    /// <summary>
    /// Loads a configuration from text (source code).
    /// </summary>
    ///
    /// <param name="source">The text (source code) of the configuration.</param>
    ///
    /// <returns>
    /// The loaded <see cref="CacheConfiguration"/> object.
    /// </returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is null.</exception>
    public static CacheConfiguration LoadFromString(string source)
    {
      return source == null ? throw new ArgumentNullException(nameof(source))
                            : CacheConfigurationReader.ReadFromString(source);
    }

    /// <summary>
    /// Loads a configuration from a binary file using a specific <see cref="BinaryReader"/>.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    /// <param name="reader">  The reader to use. Specify null to use the default <see
    /// cref="BinaryReader"/>.</param>
    ///
    /// <returns>
    /// The loaded configuration.
    /// </returns>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="filename"/> is null or empty.</exception>
    public static CacheConfiguration LoadFromBinaryFile(string filename, BinaryReader? reader = null)
    {
      if (string.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException(nameof(filename));
      }

      using (var stream = File.OpenRead(filename))
      {
        return LoadFromBinaryStream(stream, reader);
      }
    }

    /// <summary>
    /// Loads a configuration from a binary stream, using a specific <see cref="BinaryReader"/>.
    /// </summary>
    ///
    /// <param name="stream">The stream to load the configuration from.</param>
    /// <param name="reader">The reader to use. Specify null to use the default <see
    /// cref="BinaryReader"/>.</param>
    ///
    /// <returns>
    /// The loaded configuration.
    /// </returns>
    ///
    /// <remarks>
    /// This method does not dispose the provided <paramref name="stream"/>.
    /// If <paramref name="reader"/> is supplied, it is also left open.
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is null.</exception>
    public static CacheConfiguration LoadFromBinaryStream(Stream stream, BinaryReader? reader = null)
    {
      return stream == null ? throw new ArgumentNullException(nameof(stream))
                            : CacheConfigurationReader.ReadFromBinaryStream(stream, reader);
    }

    /// <summary>
    /// Saves the configuration to a file using the default character encoding, which is UTF8.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    public void SaveToFile(string filename) => SaveToFile(filename, (Encoding?)null);

    /// <summary>
    /// Saves the configuration to a file.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    /// <param name="encoding">The character encoding to use. Specify null to use the default encoding, which
    /// is UTF8.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="filename"/> is null or empty.</exception>
    public void SaveToFile(string filename, Encoding? encoding)
    {
      if (string.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException(nameof(filename));
      }

      using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
      {
        SaveToStream(stream, encoding);
      }
    }

    /// <summary>
    /// Saves the configuration to a stream using the default character encoding, which is UTF8.
    /// </summary>
    ///
    /// <param name="stream">The stream to save the configuration to.</param>
    public void SaveToStream(Stream stream) => SaveToStream(stream, (Encoding?)null);

    /// <summary>
    /// Saves the configuration to a stream.
    /// </summary>
    ///
    /// <param name="stream">The stream to save the configuration to.</param>
    /// <param name="encoding">The character encoding to use. Specify null to use the default encoding, which
    /// is UTF8.</param>
    ///
    /// <remarks>
    /// This method does not close or dispose the provided <paramref name="stream"/>.
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is null.</exception>
    public void SaveToStream(Stream stream, Encoding? encoding)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      CacheConfigurationWriter.WriteToStreamTextual(this, stream, encoding);
    }

    /// <summary>
    /// Saves the configuration to a binary file, using the default <see cref="BinaryWriter"/>.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    public void SaveToBinaryFile(string filename) => SaveToBinaryFile(filename, (BinaryWriter?)null);

    /// <summary>
    /// Saves the configuration to a binary file, using a specific <see cref="BinaryWriter"/>.
    /// </summary>
    ///
    /// <param name="filename">The location of the configuration file.</param>
    /// <param name="writer">  The writer to use. Specify null to use the default writer.</param>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="filename"/> is null or empty.</exception>
    public void SaveToBinaryFile(string filename, BinaryWriter? writer)
    {
      if (string.IsNullOrEmpty(filename))
      {
        throw new ArgumentNullException(nameof(filename));
      }

      using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
      {
        SaveToBinaryStream(stream, writer);
      }
    }

    /// <summary>
    /// Saves the configuration to a binary stream, using the default <see cref="BinaryWriter"/>.
    /// </summary>
    ///
    /// <param name="stream">The stream to save the configuration to.</param>
    public void SaveToBinaryStream(Stream stream) => SaveToBinaryStream(stream, (BinaryWriter?)null);

    /// <summary>
    /// Saves the configuration to a binary file, using a specific <see cref="BinaryWriter"/>.
    /// </summary>
    ///
    /// <param name="stream">The stream to save the configuration to.</param>
    /// <param name="writer">The writer to use. Specify null to use the default writer.</param>
    ///
    /// <remarks>
    /// This method does not close or dispose the provided <paramref name="stream"/>.
    /// If <paramref name="writer"/> is supplied, it is also left open.
    /// </remarks>
    ///
    /// <exception cref="ArgumentNullException">When <paramref name="stream"/> is null.</exception>
    public void SaveToBinaryStream(Stream stream, BinaryWriter? writer)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      CacheConfigurationWriter.WriteToStreamBinary(this, stream, writer);
    }

    /// <summary>
    /// Saves the configuration to a string.
    /// </summary>
    public string SaveToString()
    {
      return CacheConfigurationWriter.WriteToString(this);
    }

    /// <summary>
    /// Gets the number of sections that are in the configuration.
    /// </summary>
    public int SectionCount => _sections.Count;

    /// <summary>
    /// Gets or sets a section by index.
    /// </summary>
    /// <param name="index">The index of the section in the configuration.</param>
    ///
    /// <returns>
    /// The section at the specified index.
    /// Note: no section is created when using this accessor.
    /// </returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">When the index is out of range.</exception>
    public CacheSection this[int index] => index < 0 || index >= _sections.Count
                                          ? throw new ArgumentOutOfRangeException(nameof(index))
                                          : _sections[index];

    /// <summary>
    /// Gets or sets a section by its name.
    /// If there are multiple sections with the same name, the first section is returned.
    /// If you want to obtain all sections that have the same name, use the GetSectionsNamed() method instead.
    /// </summary>
    ///
    /// <param name="name">
    /// The section name. Matching is case-insensitive using ordinal comparison.
    /// </param>
    ///
    /// <returns>
    /// The section if found, otherwise a new section with
    /// the specified name is created, added to the configuration and returned.
    /// This is a create-or-get operation.
    /// </returns>
    public CacheSection this[string name]
    {
      get
      {
        var section = FindSection(name);

        if (section == null)
        {
          section = new CacheSection(name);
          Add(section);
        }

        return section;
      }
    }

    /// <summary>
    /// Gets the default, hidden section.
    /// </summary>
    public CacheSection DefaultSection => this[CacheSection.DefaultSectionName];

    /// <summary>
    /// Gets all sections that have a specific name.
    /// </summary>
    /// <param name="name">The section name to match.</param>
    /// <param name="comparison">
    /// The string comparison to use. The default is <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </param>
    /// <returns>
    /// The found sections.
    /// Change from 3.2.9.1 to 3.3: Returns an <see cref="IEnumerable{T}"/> internally as of version 3.3.
    /// Previously returned a <see cref="List{T}"/>.
    /// </returns>
    public IEnumerable<CacheSection> GetSectionsNamed(
        string name,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
      if (name == null)
      {
        return Enumerable.Empty<CacheSection>();
      }

      return _sections.Where(section => string.Equals(section.Name, name, comparison));
    }

    // Finds a section by its name.
    private CacheSection? FindSection(string name)
    {
      return name == null ? null
                          : _sections.FirstOrDefault(
                              section => string.Equals(section.Name, name, StringComparison.OrdinalIgnoreCase));
    }
  }
}
