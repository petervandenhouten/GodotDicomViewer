using Godot;
using System;
using Serilog;

public partial class Mediator : Node, IMediator
{
	private static readonly ILogger _log = Log.ForContext<Mediator>();
	private string m_active_command;
	
	// when the mediator receives ad ID with start/stop from the GUI
	// - it can find the corresponding command, and generate an event for the listeners
	// - we can define an ordened stack (layer) of listeners
	// - the command data can define to
	//   * send it as C# or godot event (maybe for other guis)
	//   * send the event globally or only to children, or only to peers
	//   * what parts of the GUI has to be invalidated/updates
	//   * check the user rights of the command
	
	
	// todo: but the command object related top the id in this event
	
	// Godot’s C# source generator creates an event from your delegate
	// name by removing EventHandler. 
	//  Without that suffix, there’s a naming conflict and the signal isn’t registered. 
	//[Signal] 
	//public delegate void godot_command_triggeredEventHandler(string id);
	// Interface event (from C# IMediator)
	public event EventHandler<string> command_triggered;
//	public event EventHandler<string, bool> command_active_changed;	
	
	public override void _Ready()
	{
	}
		
	protected void set_active_command(string id)
	{
		_log.Information("set_active_command {id}:", id);
		m_active_command = id;
	}

	protected void reset_active_command()
	{
		_log.Information("reset_active_command");
		m_active_command = "";
	}
	// Raise a command with a state, allow observers to connect to updates 
	// of the command, like mouse movevements,  mouse clicks and key presses
	public bool command(string id, bool active) 
	{
		_log.Information("command {id}: {active}", id, active);
		
		if (active)
		{
			set_active_command(id);
		}
		else
		{
			reset_active_command();
		}
//		command_triggered.Invoke(this, id,active);
		command_triggered.Invoke(this, id);
		return true;
	}
	
	// Rais a one time command
	public bool command(string id)
	{
		_log.Information("command {id}: triggered", id);
		
		set_active_command(id);
		// Raise the C# interface event
		command_triggered.Invoke(this, id);
		// Emit the GODOT signal
		// maybe the command data could configure if we want to emit a godot signal
		// EmitSignal(SignalName.godot_command_triggered, id);
		reset_active_command();
		return true;
	}
	
	public static IMediator? FindMediatorNode(Node calling_node)
	{
		if (calling_node==null) return null;
		var find_group_nodes = calling_node.GetTree().GetNodesInGroup("Mediator");
		if ( find_group_nodes.Count == 0  ) _log.Error("No Mediator node found.");
		var mediator_node = find_group_nodes[0];
		var mediator = mediator_node as IMediator;
		if ( mediator == null ) _log.Error("No Mediator script found.");
		return mediator;
	}
}
