using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Uno.Extensions;

namespace ApplicationTemplate;

/// <summary>
/// This class is used for logging configuration.
/// - Configures the logger filters (see appsettings.json).
/// - Configures the loggers.
/// </summary>
public static class LoggingConfiguration
{
	public static void ConfigureLogging(HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder, bool isAppLogging)
	{
		var logFilesProvider = new LogFilesProvider();
		var serilogConfiguration = new LoggerConfiguration();

		serilogConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
//-:cnd:noEmit
#if __WINDOWS__
		serilogConfiguration.Enrich.With(new ThreadIdEnricher());
#endif
//+:cnd:noEmit

		var options = hostBuilderContext.Configuration
			.GetSection("LoggingOutput")
			.Get<LoggingOutputOptions>();

		if (options.IsConsoleLoggingEnabled)
		{
			AddConsoleLogging(serilogConfiguration);
		}

		if (options.IsFileLoggingEnabled)
		{
			AddFileLogging(serilogConfiguration, logFilesProvider.GetLogFilePath(isAppLogging));
		}

		var logger = serilogConfiguration.CreateLogger();

		if (isAppLogging)
		{
			// The logs coming from Uno will be sent to the app logger and not the host logger.
			LogExtensionPoint.AmbientLoggerFactory.AddSerilog(logger);
		}

		loggingBuilder.AddSerilog(logger);
		loggingBuilder.Services.AddSingleton<ILogFilesProvider>(logFilesProvider);

#if __ANDROID__ || __IOS__
		global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "False positive: These are message template. The brackets are expected.")]
	private static LoggerConfiguration AddConsoleLogging(LoggerConfiguration configuration)
	{
		return configuration
//-:cnd:noEmit
#if __ANDROID__
			// The native Android logs capture some information by default, which means we don't have to print everything in the message itself.
#if DEBUG
			// In debug however, we want to add the log level so that the VSColor extension properly colorizes our output.
			.WriteTo.AndroidLog(outputTemplate: "{Level:u1}/ {Message:lj} {Exception}");
#else
			.WriteTo.AndroidLog(outputTemplate: "{Message:lj} {Exception}");
#endif
#elif __IOS__
			.WriteTo.NSLog(outputTemplate: "{Level:u1}/{SourceContext}: {Message:lj} {Exception}");
#else
			.WriteTo.Debug(outputTemplate: "{Timestamp:HH:mm:ss.fff} Thread:{ThreadId} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}");
#endif
//+:cnd:noEmit
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "False positive: These are message template. The brackets are expected.")]
	private static LoggerConfiguration AddFileLogging(LoggerConfiguration configuration, string logFilePath)
	{
//-:cnd:noEmit
#if __ANDROID__ || __IOS__
		return configuration
			.WriteTo.File(logFilePath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fffzzz} [{Platform}] {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}", fileSizeLimitBytes: 10485760) // 10mb
#if __ANDROID__
			.Enrich.WithProperty("Platform", "Android");
#elif __IOS__
			.Enrich.WithProperty("Platform", "iOS");
#endif
#elif __WINDOWS__
		return configuration
			.WriteTo.File(logFilePath, outputTemplate: "{Timestamp:MM-dd HH:mm:ss.fffzzz} [{Platform}] Thread:{ThreadId} {Level:u1}/{SourceContext}: {Message:lj} {Exception}{NewLine}")
			.Enrich.WithProperty("Platform", "UWP");

#else
		return configuration;
#endif
//+:cnd:noEmit
	}

	public class LogFilesProvider : ILogFilesProvider
	{
		public void DeleteLogFiles()
		{
			foreach (var path in GetLogFilesPaths())
			{
				File.Delete(path);
			}
		}

		public string GetLogFilePath(bool isAppLogging = true)
		{
			var logDirectory = GetLogFilesDirectory();

			return isAppLogging
				? Path.Combine(logDirectory, "ApplicationTemplate.log")
				: Path.Combine(logDirectory, "ApplicationTemplate.host.log");
		}

		public static string GetLogFilesDirectory()
		{
#if __WINDOWS__
			return Windows.Storage.ApplicationData.Current.LocalFolder.Path;
#else
			return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
		}

		public string[] GetLogFilesPaths()
		{
			return new[]
			{
				GetLogFilePath(),
				GetLogFilePath(isAppLogging: false),
			};
		}
	}
}
