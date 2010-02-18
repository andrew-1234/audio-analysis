using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Linq;

namespace TowseyLib
{
	public class Configuration
	{
		Dictionary<string, string> table;
        public string Source { get; set; }
        
        public Configuration()
		{
			table = new Dictionary<string, string>();
		}

		public Configuration(params string[] files)
		{
			if (files == null || files.Length == 0)
				throw new ArgumentNullException("files must be supplied and contain entries.");

			Source = files[files.Length - 1]; // Take last file as filename
			table = new Dictionary<string, string>();
			foreach (var file in files)
                foreach (var item in FileTools.ReadPropertiesFile(file))
                {
                    table[item.Key] = item.Value;
                    //if (item.Key.StartsWith("VERBOSITY")) Console.WriteLine("VERBOSITY = " + item.Value);
                }
        }


		public string ResolvePath(string path)
		{
			if (path == null)
				return null;
			if (!Path.IsPathRooted(path))
			{
				if (Source == null)
					throw new InvalidOperationException("Configuration was not loaded from a file. Relative paths can not be resolved.");
				return Path.Combine(Path.GetDirectoryName(Source), path);
			}
			return path;
		}

        /// <summary>
        /// adds key-value pairs to a properties table.
        /// Removes existing pair if it has same key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetPair(string key, string value)
        {
            if (table.ContainsKey(key)) table.Remove(key);
            table.Add(key, value);
        }

		public bool ContainsKey(string key)
		{
			return table.ContainsKey(key);
		}

        public Dictionary<string, string> GetTable()
        {
            return table;
        }

		public string GetString(string key)
		{
			string value;
			return table.TryGetValue(key, out value) ? value : null;
		}

		public string GetPath(string key)
		{
			return ResolvePath(GetString(key));
		}

		public int GetInt(string key)
		{
			if (!table.ContainsKey(key))
				return -Int32.MaxValue;

			string value = this.table[key].ToString();
			if (value == null)
				return -Int32.MaxValue;

			int int32;
			if (int.TryParse(value, out int32))
				return int32;

            Log.WriteLine("Configuration.GetInt(): ERROR READING PROPERTIES FILE");
            Log.WriteLine("INVALID VALUE=" + value);
			return -Int32.MaxValue;
		}

		public int? GetIntNullable(string key)
		{
            if (!table.ContainsKey(key))
            {
                //Log.WriteIfVerbose("Configuration.GetIntNullable(): ERROR READING PROPERTIES FILE\n\t" + key + "=value NOT FOUND");
                return null;
            }

			string value = this.table[key].ToString();
			if (value == null)
				return null;

			int int32;
			if (int.TryParse(value, out int32))
				return int32;

            Log.WriteLine("ERROR READING PROPERTIES FILE");
            Log.WriteLine("INVALID VALUE=" + value);
			return null;
		}

		public double GetDouble(string key)
		{
			if (!table.ContainsKey(key))
				return -Double.MaxValue;

			string value = table[key].ToString();
			if (value == null)
				return -Double.MaxValue;

			double d;
			if (double.TryParse(value, out d))
				return d;

            Log.WriteLine("ERROR READING PROPERTIES FILE");
            Log.WriteLine("INVALID VALUE=" + value);
			return -Double.MaxValue;
		}

		public double? GetDoubleNullable(string key)
		{
			if (!table.ContainsKey(key))
				return null;

			string value = table[key].ToString();
			if (value == null)
				return null;

			double d;
			if (double.TryParse(value, out d))
				return d;

			System.Console.WriteLine("ERROR READING PROPERTIES FILE");
			System.Console.WriteLine("INVALID VALUE=" + value);
			return null;
		}

		public bool GetBoolean(string key)
		{
			bool keyExists = this.table.ContainsKey(key);
			if (!keyExists) return false;
			bool b = false;
			string value = this.table[key].ToString();
			if (value == null) return b;
			try
			{
				b = Boolean.Parse(value);
			}
			catch (System.FormatException ex)
			{
				System.Console.WriteLine("ERROR READING PROPERTIES FILE");
				System.Console.WriteLine("INVALID VALUE=" + value);
				System.Console.WriteLine(ex);
				return false;
			}
			return b;
		} //end getBoolean
	} // end of class Configuration

    //#####################################################################################################################################

    /// <summary>
    /// NOTE: This is an extension class
    /// All its methods are extensions for the Configuraiton class.
    /// These methods can be called with unusual syntax!
    /// i.e. can call thus:- writer.WriteConfigPath(string basePath, string key, string value)
    /// where var writer is type TextWriter.
    /// </summary>
	public static class ConfigurationExtensions
	{
		public static void WriteConfigValue(this TextWriter writer, string key, object value)
		{
            if (value == null)
            {
                Log.WriteLine("WriteConfigValue() WARNING!!!! NULL VALUE for KEY=" + key);
                return;
            }
			writer.WriteLine(key + "=" + value.ToString());
		}

        /// <summary>
        /// NOTE: This is an extension method
        /// i.e. can call this thus:- writer.WriteConfigPath(string basePath, string key, string value)
        /// where var writer is type TextWriter.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="basePath"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
		public static void WriteConfigPath(this TextWriter writer, string basePath, string key, string value)
		{
			//var relValue = RelativePathTo(basePath, value);
            var relValue = basePath +"\\"+ value;
			writer.WriteConfigValue(key, relValue);
		}

		public static void WriteConfigArray(this TextWriter writer, string keyPattern, object[] values)
		{
            if (values == null)
            {
            }

			for (int i = 0; i < values.Length; i++)
				writer.WriteConfigValue(string.Format(keyPattern, i + 1), values[i]);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="basePath">the output directory</param>
        /// <param name="keyPattern">the key as reg exp</param>
        /// <param name="values"></param>
		public static void WriteConfigPathArray(this TextWriter writer, string basePath, string keyPattern, string[] values)
		{
            //Log.WriteLine("WriteConfigPathArray(): keyPattern=" + keyPattern, 1);
            if (keyPattern == null)
            {
                Log.WriteLine("WriteConfigPathArray() WARNING!!!! NULL VALUE for keyPattern");
                return;
            }
            if (values == null)
            {
                Log.WriteLine("WriteConfigPathArray() WARNING!!!! NULL ARRAY for KEY=" + keyPattern, '?');
                return;
            }
            for (int i = 0; i < values.Length; i++)
                writer.WriteConfigPath(basePath, string.Format(keyPattern, i + 1), values[i]);
		}


		public static string RelativePathTo(string fromDirectory, string toPath)
		{
			if (fromDirectory == null)
				throw new ArgumentNullException("fromDirectory");

			if (toPath == null)
				throw new ArgumentNullException("toPath");

			bool isRooted = Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(toPath);
			if (isRooted)
			{
				bool isDifferentRoot = string.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(toPath), true) != 0;
				if (isDifferentRoot)
					return toPath;
			}

			var relativePath = new List<string>();
			string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);
			string[] toDirectories = toPath.Split(Path.DirectorySeparatorChar);

			int length = Math.Min(fromDirectories.Length, toDirectories.Length);
			int lastCommonRoot = -1;

			// find common root
			for (int x = 0; x < length; x++)
			{
				if (string.Compare(fromDirectories[x], toDirectories[x], true) != 0)
					break;
				lastCommonRoot = x;
			}

			if (lastCommonRoot == -1)
				return toPath;

			// add relative folders in from path
			for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
				if (fromDirectories[x].Length > 0)
					relativePath.Add("..");

			// add to folders to path
			for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
				relativePath.Add(toDirectories[x]);

			return string.Join(Path.DirectorySeparatorChar.ToString(), relativePath.ToArray());
        } //end of method RelativePathTo(string fromDirectory, string toPath)

    } //end of static class ConfigurationExtensions
}