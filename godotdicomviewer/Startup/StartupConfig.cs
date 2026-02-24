using Godot;
using System;
using Serilog;
using GodotDicomViewer.Configuration;
using GodotDicomViewer.GUI;

// Startup with configation, add monitors and datasources as configured

public partial class StartupConfig : Node
{
	private static readonly ILogger _log = Log.ForContext<StartupConfig>();
	
	public override async void _Ready()
	{
		_log.Information("Start up configuration mode");
		
		if ( !HasNode("/root/Main/GUI") )
		{
			_log.Error("No GUI node found");
		}
				
		var configNodesInGroup = GetTree().GetNodesInGroup("Configuration");
		if ( configNodesInGroup.Count == 0  )
		{
			_log.Error("No Configuration node found");
		}

		var config_node = configNodesInGroup[0];
		await ToSignal(config_node, "ready");

		var gui = GetNode<GuiManager>("/root/Main/GUI");
		
		var config = config_node as IConfiguration;
		if ( config != null )
		{
			_log.Information("Config NumberOfMonitors: {x}", config.NumberOfMonitors);
			_log.Information("Config Patients monitor: {x}", config.PatientsMonitor);

			for ( int monitor = 0 ; monitor<config.NumberOfMonitors; monitor++)
			{
				bool has_viewer = true;
				bool has_patient = (monitor == config.PatientsMonitor);
				gui.AddMonitor(monitor, has_viewer, has_patient);
			}
		}
		gui.Start();
	}
}
