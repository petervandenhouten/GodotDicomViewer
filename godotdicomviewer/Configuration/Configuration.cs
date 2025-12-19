using Godot;
using GodotDicomViewer.Code.Interfaces;
using GodotDicomViewer.Configuration;

public partial class Configuration : Node
{
	private IConfiguration m_config;

	public Configuration()
	{
		m_config = new TestConfiguration();
	}

	#region Main settings
	[Export] public int NumberOfMonitors
	{
		get => m_config.NumberOfMonitors;
		set => m_config.NumberOfMonitors = value;
	}
	#endregion

	public override void _Ready()
	{
		
	}
}
