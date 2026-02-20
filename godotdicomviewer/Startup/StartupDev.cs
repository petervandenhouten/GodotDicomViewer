using Godot;
using System;
using Serilog;
using GodotDicomViewer.Configuration;

// Startup in dev mode, does not add monitor, data or datasources, but use the 
// scene graph as designed by the developer
public partial class StartupDev : Node
{
	private static readonly ILogger _log = Log.ForContext<StartupDev>();
	
	public override void _Ready()
	{
		_log.Information("Start up developmer mode");
		
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
		var config = config_node as IConfiguration;
		if (config != null)
		{
			_log.Information("Config NumberOfMonitors: {x}", config.NumberOfMonitors);
		}
		
		var gui = GetNode<GuiManager>("/root/Main/GUI");
		gui.Start();
	}
}
