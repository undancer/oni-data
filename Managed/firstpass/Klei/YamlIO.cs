using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace Klei
{
	public static class YamlIO
	{
		public struct Error
		{
			public enum Severity
			{
				Fatal,
				Recoverable
			}

			public FileHandle file;

			public string message;

			public Exception inner_exception;

			public string text;

			public Severity severity;
		}

		public delegate void ErrorHandler(Error error, bool force_log_as_warning);

		private delegate void ErrorLogger(string format, params object[] args);

		private const bool verbose_errors = false;

		public static void Save<T>(T some_object, string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			using StreamWriter writer = new StreamWriter(filename);
			SerializerBuilder serializerBuilder = new SerializerBuilder();
			if (tagMappings != null)
			{
				foreach (Tuple<string, Type> tagMapping in tagMappings)
				{
					serializerBuilder = serializerBuilder.WithTagMapping(tagMapping.first, tagMapping.second);
				}
			}
			serializerBuilder.Build().Serialize(writer, some_object);
		}

		public static void SaveOrWarnUser<T>(T some_object, string filename, List<Tuple<string, Type>> tagMappings = null)
		{
			FileUtil.DoIODialog(delegate
			{
				Save(some_object, filename, tagMappings);
			}, filename);
		}

		public static T LoadFile<T>(FileHandle filehandle, ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			return Parse<T>(FileSystem.ConvertToText(filehandle.source.ReadBytes(filehandle.full_path)), filehandle, handle_error, tagMappings);
		}

		public static T LoadFile<T>(string filename, ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			FileHandle filehandle = FileSystem.FindFileHandle(filename);
			if (filehandle.source == null)
			{
				throw new FileNotFoundException("YamlIO tried loading a file that doesn't exist: " + filename);
			}
			return LoadFile<T>(filehandle, handle_error, tagMappings);
		}

		public static void LogError(Error error, bool force_log_as_warning)
		{
			ErrorLogger errorLogger = ((force_log_as_warning || error.severity == Error.Severity.Recoverable) ? new ErrorLogger(Debug.LogWarningFormat) : new ErrorLogger(Debug.LogErrorFormat));
			if (error.inner_exception == null)
			{
				errorLogger("{0} parse error in {1}\n{2}", error.severity, error.file.full_path, error.message);
			}
			else
			{
				errorLogger("{0} parse error in {1}\n{2}\n{3}", error.severity, error.file.full_path, error.message, error.inner_exception.Message);
			}
		}

		public static T Parse<T>(string readText, FileHandle debugFileHandle, ErrorHandler handle_error = null, List<Tuple<string, Type>> tagMappings = null)
		{
			try
			{
				if (handle_error == null)
				{
					handle_error = LogError;
				}
				readText = readText.Replace("\t", "    ");
				Action<string> unmatchedLogFn = delegate(string error)
				{
					handle_error(new Error
					{
						file = debugFileHandle,
						text = readText,
						message = error,
						severity = Error.Severity.Recoverable
					}, force_log_as_warning: false);
				};
				DeserializerBuilder deserializerBuilder = new DeserializerBuilder();
				deserializerBuilder.IgnoreUnmatchedProperties(unmatchedLogFn);
				if (tagMappings != null)
				{
					foreach (Tuple<string, Type> tagMapping in tagMappings)
					{
						deserializerBuilder = deserializerBuilder.WithTagMapping(tagMapping.first, tagMapping.second);
					}
				}
				Deserializer deserializer = deserializerBuilder.Build();
				StringReader input = new StringReader(readText);
				return deserializer.Deserialize<T>(input);
			}
			catch (Exception ex)
			{
				handle_error(new Error
				{
					file = debugFileHandle,
					text = readText,
					message = ex.Message,
					inner_exception = ex.InnerException,
					severity = Error.Severity.Fatal
				}, force_log_as_warning: false);
			}
			return default(T);
		}
	}
}
