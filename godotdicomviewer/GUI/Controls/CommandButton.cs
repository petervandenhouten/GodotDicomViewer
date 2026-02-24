using Godot;
using System;
using Serilog;
using GodotDicomViewer.GUI.Controls;
using GodotDicomViewer.Code.Commands;

public partial class CommandButton : Button, ICommandControl
{
	[Export]
	string ButtonID = "X";

	// [Export]
	// Node CommandNode;
	// or
	// string nodeName; Name of parent node to use for
	// command , like "ImageControl" or "SeriesControl"
	
	private static readonly ILogger _log = Log.ForContext<CommandButton>();
	private IMediator? _mediator = null;
	private MediatorV2? _mediatorV2 = null;
	private ICommand? _command = null;
	
	public override async void _Ready()
	{
		// special way to connect to mediator, we have to wait until it is ready
		// maybe make mediator autoload?
		var find_group_nodes = GetTree().GetNodesInGroup("Mediator");
		if ( find_group_nodes.Count == 0  ) _log.Error("No Mediator node found.");
		var mediator_node = find_group_nodes[0];
		await ToSignal(mediator_node, "ready");
		_mediator = mediator_node as IMediator;
		_mediatorV2 = mediator_node as MediatorV2;
		if ( _mediator == null ) _log.Error("No Mediator script found.");

		// Get the command object and apply its metadata
		apply_command_metadata();
	}

	/// <summary>
	/// Apply command metadata (tooltip, caption, icon) to the button
	/// </summary>
	private void apply_command_metadata()
	{
		if (_mediator == null)
		{
			_log.Warning("Cannot apply command metadata - mediator is null");
			mark_as_invalid("No mediator found");
			return;
		}

		// Get command object from mediator (cast to MediatorV2)
		var mediatorV2 = _mediator as MediatorV2;
		if (mediatorV2 == null)
		{
			_log.Warning("Mediator is not MediatorV2, cannot get command metadata");
			mark_as_invalid("Using legacy Mediator");
			return;
		}

		_command = mediatorV2.GetCommand(ButtonID);
		if (_command == null)
		{
			_log.Error("Command not found in registry: {id}", ButtonID);
			mark_as_invalid($"Command '{ButtonID}' not registered");
			return;
		}

		// Apply tooltip
		if (!string.IsNullOrEmpty(_command.Tooltip))
		{
			TooltipText = _command.Tooltip;
		//	_log.Debug("Set tooltip for {id}: {tooltip}", ButtonID, _command.Tooltip);
		}

		// Apply Caption as button text (optional)
		if (!string.IsNullOrEmpty(_command.Caption))
		{
			Text = _command.Caption;
		//	_log.Debug("Set caption for {id}: {caption}", ButtonID, _command.Caption);
		}

		// Apply icon (if provided)
		if (_command.Icon != null)
		{
			Icon = _command.Icon;
		//	_log.Debug("Set icon for {id}", ButtonID);
		}

		// Set button as toggleable if command is a toggle type
		if (_command.Type == CommandType.Toggle)
		{
			ToggleMode = true;
		//	_log.Debug("Set button to toggle mode: {id}", ButtonID);
		}

		// Disable button if command is disabled
		if (!_command.IsEnabled)
		{
			Disabled = true;
			_log.Debug("Disabled button: {id}", ButtonID);
		}

		// Hide button if command is not visible
		if (!_command.IsVisible)
		{
			Visible = false;
			_log.Debug("Hidden button: {id}", ButtonID);
		}
	}

	/// <summary>
	/// Mark button as invalid state (missing command, configuration error, etc)
	/// </summary>
	private void mark_as_invalid(string reason)
	{
		Disabled = true;
		TooltipText = $"⚠️ Configuration Error: {reason}";
		_log.Error("CommandButton marked invalid: {reason}", reason);
	}
	
	private void button_toggled(bool toggled_on) 
	{
		_log.Information("{id} toggle {toggle}", ButtonID, toggled_on);
		GD.Print($"{ButtonID} toggle {toggled_on}");
		
		if (_mediatorV2 != null)
		{
			_mediatorV2.command(ButtonID, toggled_on, this);
		}
	}

	private void button_pressed() 
	{
		_log.Information("{id} pressed", ButtonID);
		GD.Print($"{ButtonID} pressed");
		
		if (_mediatorV2 != null)
		{
			_mediatorV2.command(ButtonID, this);
		}
	}

}
