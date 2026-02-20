using Godot;
using System;
using Serilog;

public partial class ImageControl : Control
{
	private static readonly ILogger _log = Log.ForContext<ImageControl>();
	private IMediator? _mediator = null;
	
	public override void _Ready()
	{
		connect_to_mediator();
	}
	
	protected void connect_to_mediator()
	{
		_mediator = Mediator.FindMediatorNode(this);
		if (_mediator != null)
		{
			_mediator.command_triggered += on_command;
		}
	}

	protected void on_command(object? sender, string id)
	{
		_log.Information("on_command {id} ", id);
		
		if ( id == "Zoom") start_zoom();
	}
	
	protected void start_zoom()
	{
		_log.Information("start_zoom");
		
		// subscribe to the update event of the command passed to here
		// the mediator will make the command update because of mouse/key events?
	}

}
