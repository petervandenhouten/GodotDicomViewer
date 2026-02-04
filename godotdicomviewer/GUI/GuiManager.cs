using Godot;
using System;
using System.Collections.Generic;
using Serilog;

public partial class GuiManager : Node
{
	private static readonly ILogger _log = Log.ForContext<GuiManager>();
	private static readonly PackedScene monitor_scene = GD.Load<PackedScene>("res://GUI//Monitor/monitor.tscn");
	private IMediator _mediator = null;
	
	protected void connect_to_mediator()
	{
		var find_group_nodes = GetTree().GetNodesInGroup("Mediator");
		if ( find_group_nodes.Count == 0  ) _log.Error("No Mediator node found.");
		var mediator_node = find_group_nodes[0];
		_mediator = mediator_node as IMediator;
		if ( _mediator == null ) _log.Error("No Mediator script found.");
		
		// Subscribe to the interface event (pure C#)
		_mediator.command_triggered += on_command_interface;
		// subscribe to the Godot-generated C# event
		var mediator = mediator_node as Mediator;
		mediator.godot_command_triggered += on_command_godot;
		
		if (_mediator != null) _log.Information("GUI manager connected to Mediator.");
	}
	
	private void on_command_interface(object? sender, string id)
	{
		_log.Information("on_command (C#) {id} ", id);
		on_command(id);
	}
		
	private void on_command_godot(string id) 
	{
		_log.Information("on_command (godot) {id} ", id);
		on_command(id);
	}
	
	public void Start()
	{
		_log.Information("Start");
		var monitors = GetMonitorsFromChildNodes();
		_log.Information("Number of monitor nodes found {x}", monitors.Count);
		connect_to_mediator();
	}
	
	public void AddMonitor(int id, bool has_viewer, bool has_patient)
	{
		_log.Information("AddMonitor {id} viewer={v} patient={p}", id, has_viewer, has_patient);
		
		var monitor = monitor_scene.Instantiate<MonitorManager>();
		if ( monitor == null )
		{
			_log.Error("Cannot instantiate a monitor scene node");
			return;
		}

		// TODO: Replace with initialize function
		monitor.MonitorId 		 = id;
		monitor.HasPatientWindow = has_patient;
		monitor.HasViewerWindow  = has_viewer;
		
		// add the viewer as a child of this node (note that this is not the main control)
		AddChild(monitor);
		_log.Information("Create monitor node for {x}", id);
	}
	
	protected List<MonitorManager> GetMonitorsFromChildNodes()
	{
		var list = new List<MonitorManager>();	
		foreach (Node child in GetChildren())
		{
			if (child.IsInGroup("Monitor"))
			{
				list.Add(child as MonitorManager);
			}
		}
		return list;
	}
	
	public void exit_application()
	{
		_log.Information("exit_application");
		GetTree().Quit();	
	}
	
	protected void on_command(string id) 
	{
		_log.Information("on_command {id} ", id);
		
		if ( id == "Exit") exit_application();
	}
}
