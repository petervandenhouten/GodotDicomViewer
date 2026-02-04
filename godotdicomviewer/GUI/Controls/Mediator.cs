using Godot;
using System;
using Serilog;

public partial class Mediator : Node, IMediator
{
	private static readonly ILogger _log = Log.ForContext<Mediator>();

	// todo: but the command object related top the id in this event
	
	// Godot’s C# source generator creates an event from your delegate
	// name by removing EventHandler. 
	//  Without that suffix, there’s a naming conflict and the signal isn’t registered. 
	[Signal] 
	public delegate void godot_command_triggeredEventHandler(string id);
	// Interface event (from C# IMediator)
	public event EventHandler<string> command_triggered;
	
	public override void _Ready()
	{
	}
		
	public bool command(string id, bool active) 
	{
		_log.Information("command {id}: {active}", id, active);
		return false;
	}
	
	public bool command(string id)
	{
		_log.Information("command {id}: triggered", id);
		// Raise the C# interface event
		command_triggered.Invoke(this, id);
		// Emit the GODOT signal
		// maybe the command data could configure if we want to emit a godot signal
		// EmitSignal(SignalName.godot_command_triggered, id);
		return true;
	}
}
