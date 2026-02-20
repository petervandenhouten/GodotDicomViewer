using System;

//public delegate void command_triggered_delegate(string id);

public interface IMediator
{
	abstract bool command(string id, bool active);
	
	abstract bool command(string id);
	
	//event command_triggered_delegate command_triggered;
	event EventHandler<string> command_triggered;
	
	// X wont compile, use a delegate?
	// event EventHandler<string, bool> command_active_changed;

	// command( ... , node(=parent of button) )
	
	// make activate_command, deactivate_command...
	
	// abstract public bool command(string id, bool active, string parameter);
}
