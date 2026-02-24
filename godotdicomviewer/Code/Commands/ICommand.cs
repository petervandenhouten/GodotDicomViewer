using System;
using Godot;

namespace GodotDicomViewer.Code.Commands
{
	/// <summary>
	/// Represents a command with metadata and behavior.
	/// Replaces simple string IDs with rich command objects.
	/// </summary>
	public interface ICommand
	{
		// Identity
		string CommandID { get; }
		
		// Display
		string Caption { get; }
		string Tooltip { get; }
		Texture2D Icon { get; }
		
		// Organization
		string Category { get; }  // "File", "Edit", "View", "Tools", etc.
		CommandType Type { get; }
		
		// Help
		string HelpText { get; }
		string HelpScenePath { get; }  // Optional path to help scene
		
		// State
		bool IsEnabled { get; }
		bool IsVisible { get; }
		bool? IsToggled { get; }  // null if not toggleable
		
		// Metadata
		bool RequiresUser { get; }  // Requires user to confirm
		bool RequiresSelection { get; }  // Can only run with selection
		
		// Execution
		void Execute(CommandContext context);
	}

	public enum CommandType
	{
		Action,     // Executes an action (no state)
		Toggle,     // Can be toggled on/off
		Selection   // Operates on selected items
	}
}
