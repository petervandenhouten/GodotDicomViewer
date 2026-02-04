using Godot;
using System;
using Serilog;

public partial class CommandButton : Button, ICommandControl
{
	[Export]
	string ButtonID = "Zoom";

	// [Export]
	// Node CommandNode;
	// or
	// string nodeName; Name of parent node to use for
	// command , like "ImageControl" or "SeriesControl"
	
	private static readonly ILogger _log = Log.ForContext<CommandButton>();
	private IMediator _mediator = null;
	
	public override async void _Ready()
	{
		var find_group_nodes = GetTree().GetNodesInGroup("Mediator");
		if ( find_group_nodes.Count == 0  ) _log.Error("No Mediator node found.");
		
		var mediator_node = find_group_nodes[0];
		await ToSignal(mediator_node, "ready");
		_mediator = mediator_node as IMediator;
		if ( _mediator == null ) _log.Error("No Mediator script found.");

	}
		
	private void button_toggled(bool toggled_on) 
	{
		_log.Information("{id} toggle {toggle}", ButtonID, toggled_on);
		GD.Print($"{ButtonID} toggle {toggled_on}");
		
		if (_mediator != null)
		{
			_mediator.command(ButtonID,toggled_on);
		}
	}

	private void button_pressed() 
	{
		_log.Information("{id} pressed", ButtonID);
		GD.Print($"{ButtonID} pressed");
		
		if (_mediator != null)
		{
			_mediator.command(ButtonID);
		}
	}

}
