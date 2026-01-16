using Godot;
using System;
using GodotDicomViewer.Configuration;

public partial class BaseConfiguration : Node
{
	protected readonly Configuration config;

	[Export]
	public int NumberOfMonitors
	{ 
		get => config.NumberOfMonitors;
		set { config.NumberOfMonitors = value; }
	}

	[Export]
	public int PatientsMonitor
	{ 
		get => config.PatientsMonitor;
		set { config.PatientsMonitor = value; }
	}
	
	protected BaseConfiguration()
	{
		config = new Configuration();
	}
}
