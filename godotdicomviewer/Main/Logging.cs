using Godot;
using GodotDicomViewer.StudySources.Test;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Collections.Generic;

public class GodotSink : ILogEventSink
{
	public void Emit(LogEvent logEvent)
	{
		var message = $"[{logEvent.Timestamp:HH:mm:ss}] [{logEvent.Level}] [{logEvent.Properties.GetValueOrDefault("SourceContext")}] {logEvent.RenderMessage()}";
		GD.Print(message);
	}
}

public partial class Logging : Node
{
	private static readonly ILogger _log = Log.ForContext<Logging>();
	public override void _Ready()
	{
		Log.Logger = new LoggerConfiguration()
						.MinimumLevel.Debug()
						.Enrich.FromLogContext()
						.WriteTo.Sink(new GodotSink())
						.WriteTo.File("user://gdv_log.txt", outputTemplate: "outputTemplate: \"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{SourceContext}] {Message}{NewLine}{Exception}{NewLine}{Properties}\"")
						.CreateLogger();

		_log.Information("Godot Dicom Viewer from Serilog!");
	}
}
