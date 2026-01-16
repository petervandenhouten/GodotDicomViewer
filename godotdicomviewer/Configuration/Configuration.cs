using Godot;
using GodotDicomViewer.Configuration;

namespace GodotDicomViewer.Configuration
{
	public class Configuration : IConfiguration
	{
		public int NumberOfMonitors { get; set; } = 1;
		public int PatientsMonitor { get; set; } = 0;
	}
}
