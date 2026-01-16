using Godot;
using System;

namespace GodotDicomViewer.Configuration
{
	[Tool]
	public partial class FileConfiguration : BaseConfiguration, IConfiguration
	{
		public override void _Ready()
		{
			GD.Print("Ready Enter", config.NumberOfMonitors);
			config.NumberOfMonitors = 2;
			GD.Print("Ready Leave", config.NumberOfMonitors);
			
		}
	}
}
