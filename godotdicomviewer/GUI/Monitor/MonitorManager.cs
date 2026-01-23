using Godot;
using System;
using Serilog;

public partial class MonitorManager : Node
{
	[Export] public int MonitorId { get; set; } = 0;
	[Export] public bool HasPatientWindow { get; set; } = false;
	[Export] public bool HasViewerWindow { get; set; } = true;

	private static readonly ILogger _log = Log.ForContext<MonitorManager>();
	private static readonly PackedScene viewer_window = GD.Load<PackedScene>("res://GUI//ViewerWindow/viewer/viewer_window.tscn");
	private static readonly PackedScene patient_window = GD.Load<PackedScene>("res://GUI//PatientWindow/patient_window.tscn");

	public override void _Ready()
	{
		if ( MonitorId<0 ) return;

		if ( viewer_window is null ) _log.Error("No scene for viewer window");
		if ( patient_window is null ) _log.Error("No scene for patient window");
		
		GetViewport().SetEmbeddingSubwindows(false);
		if ( HasViewerWindow ) InstantiateViewerWindow();
		if ( HasPatientWindow ) InstantiatePatientWindow();
		
		// todo: hide viewer windows if they are at the same monitor as a patient window
	}
	
	protected void InstantiatePatientWindow()
	{
		var window = patient_window.Instantiate() as Window;
		if ( window == null )
		{
			_log.Error("Cannot instantiate a patient window");
			return;
		}
		// add the viewer as a child of this node (note that this is not the main control)
		AddChild(window);
		_log.Information("Create patient window on monitor {x}", MonitorId);
		window.Visible 	= true;
		window.Position = DisplayServer.ScreenGetPosition(MonitorId);
		window.Title 	= "Patients";
		window.Size  	= DisplayServer.ScreenGetSize(MonitorId);
	}
	
	protected void InstantiateViewerWindow()
	{
		var viewer = viewer_window.Instantiate() as Window;
		if ( viewer == null )
		{
			_log.Error("Cannot instantiate a viewer window");
			return;
		}
		// add the viewer as a child of this node (note that this is not the main control)
		AddChild(viewer);
		_log.Information("Create viewer window on monitor {x}", MonitorId);
		viewer.Visible 	= true;
		viewer.Position = DisplayServer.ScreenGetPosition(MonitorId);
		viewer.Title 	= "Viewer " + MonitorId;
		viewer.Size  	= DisplayServer.ScreenGetSize(MonitorId); // - new Vector2I(8,4);
	}
}
