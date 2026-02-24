using Godot;
using System;
using Serilog;
using System.Collections.Generic;
using GodotDicomViewer.Code.Commands;

namespace GodotDicomViewer.GUI.Controls
{
	/// <summary>
	/// Enhanced Mediator that translates button IDs to command objects.
	/// Automatically discovers services in the scene using node groups and builds CommandContext.
	/// </summary>
	public partial class MediatorV2 : Node, IMediator
	{
		private static readonly ILogger _log = Log.ForContext<MediatorV2>();
		private CommandRegistry _commandRegistry;
		private string m_active_command;

		// Enhanced event that passes the full command object instead of just ID
		public event EventHandler<ICommand> command_triggered_v2;

		// Legacy event for backwards compatibility
		public event EventHandler<string> command_triggered;

		public override void _Ready()
		{
			_commandRegistry = new CommandRegistry();
			AddToGroup("Mediator");
			_log.Information("Mediator initialized with command registry");
		}

		/// <summary>
		/// Execute a command by ID (implements IMediator interface).
		/// </summary>
		public bool command(string id)
		{
			return CommandInternal(id, null);
		}

		/// <summary>
		/// Execute a command by ID with a caller node for context discovery.
		/// </summary>
		public bool command(string id, Node caller)
		{
			return CommandInternal(id, caller);
		}

		/// <summary>
		/// Execute a toggleable command with state (implements IMediator interface).
		/// </summary>
		public bool command(string id, bool active)
		{
			return CommandToggleInternal(id, active, null);
		}

		/// <summary>
		/// Execute a toggleable command with state and caller node for context discovery.
		/// </summary>
		public bool command(string id, bool active, Node caller)
		{
			return CommandToggleInternal(id, active, caller);
		}

		/// <summary>
		/// Internal implementation for executing commands.
		/// </summary>
		private bool CommandInternal(string id, Node caller)
		{
			var cmd = _commandRegistry.GetCommand(id);
			if (cmd == null)
			{
				_log.Warning("Command not found: {id}", id);
				return false;
			}

			_log.Information("Command triggered: {id} (Caption: {caption})", id, cmd.Caption);

			if (!cmd.IsEnabled)
			{
				_log.Warning("Command is disabled: {id}", id);
				return false;
			}

			if (cmd.RequiresUser)
			{
				_log.Information("Command requires user confirmation: {id}", id);
				//Could trigger a confirmation dialog here
			}

			//Build command context with discovered services
			var context = BuildCommandContext(caller);

			//Execute command with context
			cmd.Execute(context);

			//Trigger new event with full command object
			command_triggered_v2?.Invoke(this, cmd);

			//Also trigger legacy event for backwards compatibility
			command_triggered?.Invoke(this, id);

			return true;
		}

		/// <summary>
		/// Internal implementation for executing toggle commands.
		/// </summary>
		private bool CommandToggleInternal(string id, bool active, Node caller)
		{
			var cmd = _commandRegistry.GetCommand(id);
			if (cmd == null)
			{
				_log.Warning("Command not found: {id}", id);
				return false;
			}

			if (cmd.Type != CommandType.Toggle)
			{
				_log.Warning("Command is not toggleable: {id}", id);
				return false;
			}

			_log.Information("Command toggled: {id} -> {state}", id, active);

			//Build command context with discovered services
			var context = BuildCommandContext(caller);

			//Execute command with context
			cmd.Execute(context);

			//Trigger new event with full command object
			command_triggered_v2?.Invoke(this, cmd);

			//Also trigger legacy event
			command_triggered?.Invoke(this, id);

			return true;
		}

		/// <summary>
		/// Build command context by discovering services in the scene using node groups.
		/// </summary>
		private CommandContext BuildCommandContext(Node caller = null)
		{
			var context = new CommandContext()
				.WithSceneRoot(GetTree().Root)
				.WithMediator(this)
				.WithCallerNode(caller);

			// Discover and attach services using node groups
			var imageViewer = GetTree().GetFirstNodeInGroup("image_viewer") as IImageViewer;
			if (imageViewer != null)
			{
				context.WithImageViewer(imageViewer);
				_log.Debug("Discovered image_viewer service");
			}

			var dataProvider = GetTree().GetFirstNodeInGroup("data_provider") as IDataProvider;
			if (dataProvider != null)
			{
				context.WithDataProvider(dataProvider);
				_log.Debug("Discovered data_provider service");
			}

			var dialogManager = GetTree().GetFirstNodeInGroup("dialog_manager") as IDialogManager;
			if (dialogManager != null)
			{
				context.WithDialogManager(dialogManager);
				_log.Debug("Discovered dialog_manager service");
			}

			var statusBar = GetTree().GetFirstNodeInGroup("status_bar") as IStatusBar;
			if (statusBar != null)
			{
				context.WithStatusBar(statusBar);
				_log.Debug("Discovered status_bar service");
			}

			return context;
		}

		/// <summary>
		/// Get a command object directly (optional, for advanced use).
		/// </summary>
		public ICommand? GetCommand(string id)
		{
			return _commandRegistry.GetCommand(id);
		}

		/// <summary>
		/// Get all available commands (useful for UI generation).
		/// </summary>
		public IEnumerable<ICommand> GetAllCommands()
		{
			return _commandRegistry.GetAllCommands();
		}

		/// <summary>
		/// Get commands by category (for organizing menus).
		/// </summary>
		public IEnumerable<ICommand> GetCommandsByCategory(string category)
		{
			return _commandRegistry.GetCommandsByCategory(category);
		}

		public static IMediator? FindMediatorNode(Node calling_node)
		{
			if (calling_node == null) return null;
			var find_group_nodes = calling_node.GetTree().GetNodesInGroup("Mediator");
			if (find_group_nodes.Count == 0)
			{
				Log.ForContext<MediatorV2>().Error("No Mediator node found.");
				return null;
			}
			var mediator_node = find_group_nodes[0];
			var mediator = mediator_node as IMediator;
			if (mediator == null)
			{
				Log.ForContext<MediatorV2>().Error("No Mediator script found.");
			}
			return mediator;
		}
	}
}
