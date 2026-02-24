using Godot;
using Serilog;
using GodotDicomViewer.Code.Commands;
using GodotDicomViewer.GUI.Controls;

namespace GodotDicomViewer.GUI
{
	/// <summary>
	/// GUI Manager handling complex commands.
	/// Shows type-safe command handling with context passing.
	/// </summary>
	public partial class GuiManager : Node
	{
		private static readonly ILogger _log = Log.ForContext<GuiManager>();
		private MediatorV2 _mediator;
		private CommandContext _commandContext;

		public override void _Ready()
		{
			// Find the Mediator node in the scene (should be a child of GUI)
			_mediator = GetNode<MediatorV2>("Mediator");
			if (_mediator == null)
			{
				_log.Error("Mediator node not found as child of GuiManager");
				return;
			}

			_commandContext = new CommandContext()
				.WithSceneRoot(GetNode("/root") ?? this)
				.WithMediator(_mediator)
				// These would normally come from your scene/manager
				// .WithImageViewer(GetImageViewer())
				// .WithDataProvider(GetDataProvider())
				// .WithDialogManager(GetDialogManager())
				// .WithStatusBar(GetStatusBar())
			;

			// Subscribe to command events
			_mediator.command_triggered_v2 += OnCommandExecuted;

			// Log all available commands
			_log.Information("Available Commands");
			foreach (var command in _mediator.GetAllCommands())
			{
				_log.Information("  {CommandID}: {Caption} ({Category})", command.CommandID, command.Caption, command.Category);
				if (!string.IsNullOrEmpty(command.Tooltip))
				{
					_log.Debug("    Tooltip: {Tooltip}", command.Tooltip);
				}
			}
		}

		/// <summary>
		/// Central event handler for all commands.
		/// Uses pattern matching for type-safe handling.
		/// </summary>
		private void OnCommandExecuted(object sender, ICommand cmd)
		{
			_log.Information("Executing command: {CommandID}", cmd.CommandID);

			try
			{
				// Handle specific commands with their requirements
				switch (cmd)
				{
					// File operations
					case ExitCommand exitCmd:
						HandleExitCommand(exitCmd);
						break;

					// View operations
					case ZoomInCommand zoomCmd:
						HandleZoomCommand(zoomCmd);
						break;

					case AdjustWindowLevelCommand levelCmd:
						HandleWindowLevelCommand(levelCmd);
						break;

					case MeasureCommand measureCmd:
						HandleMeasureCommand(measureCmd);
						break;

					// Data operations
					case LoadStudyCommand studyCmd:
						HandleLoadStudyCommand(studyCmd);
						break;

					// Default: any other command
					default:
						HandleGenericCommand(cmd);
						break;
				}
			}
			catch (System.Exception ex)
			{
				_log.Error(ex, "Command execution failed: {CommandID}", cmd.CommandID);
			}
		}

		/// <summary>
		/// Handle exit command with confirmation.
		/// </summary>
		private void HandleExitCommand(ExitCommand cmd)
		{
			_log.Information("Exiting application");
			// You could show a confirmation dialog here
			GetTree().Quit();
		}

		/// <summary>
		/// Handle zoom command - requires image viewer.
		/// </summary>
		private void HandleZoomCommand(ZoomInCommand cmd)
		{
			if (_commandContext.CurrentImageViewer == null)
			{
				_log.Warning("Cannot zoom: No image viewer available");
				return;
			}

			// Execute through context
			cmd.Execute(_commandContext);
		}

		/// <summary>
		/// Handle window/level adjustment - requires dialog and viewer.
		/// </summary>
		private void HandleWindowLevelCommand(AdjustWindowLevelCommand cmd)
		{
			if (_commandContext.DialogManager == null)
			{
				_log.Warning("Cannot adjust window/level: No dialog manager");
				return;
			}

			cmd.Execute(_commandContext);
		}

		/// <summary>
		/// Handle measure tool toggle.
		/// </summary>
		private void HandleMeasureCommand(MeasureCommand cmd)
		{
			_log.Information("Toggling measurement tool");
			cmd.Execute(_commandContext);
		}

		/// <summary>
		/// Handle study loading - requires data provider and confirmation.
		/// </summary>
		private void HandleLoadStudyCommand(LoadStudyCommand cmd)
		{
			if (_commandContext.DataProvider == null)
			{
				_log.Warning("Cannot load study: No data provider available");
				return;
			}

			if (_commandContext.CurrentPatient == null)
			{
				_log.Warning("Cannot load study: No patient selected");
				return;
			}

			cmd.Execute(_commandContext);
		}

		/// <summary>
		/// Generic handler for unknown commands.
		/// </summary>
		private void HandleGenericCommand(ICommand cmd)
		{
			_log.Debug("Generic command: {CommandID}", cmd.CommandID);
			_log.Debug("  Caption: {Caption}", cmd.Caption);
			_log.Debug("  Category: {Category}", cmd.Category);
			_log.Debug("  Type: {Type}", cmd.Type);

			// Could execute generic command here if it doesn't need context
			// cmd.Execute(_commandContext);
		}

		/// <summary>
		/// Example: Trigger command programmatically.
		/// </summary>
		public void ExecuteCommand(string commandID)
		{
			if (string.IsNullOrEmpty(commandID))
			{
				_log.Warning("ExecuteCommand: Empty command ID");
				return;
			}

			// Get command from mediator
			var command = _mediator.GetCommand(commandID);
			if (command == null)
			{
				_log.Warning("ExecuteCommand: Command not found: {CommandID}", commandID);
				return;
			}

			// Trigger through mediator (fires event)
			_mediator.command(commandID);
		}

		/// <summary>
		/// Example: Get all commands of a category.
		/// Useful for building dynamic menus.
		/// </summary>
		public void ShowCommandsByCategory(string category)
		{
			_log.Information("Commands in category: {Category}", category);
			var commands = _mediator.GetCommandsByCategory(category);

			foreach (var cmd in commands)
			{
				_log.Information("  {Caption}", cmd.Caption);
			}
		}

		/// <summary>
		/// Manually start the GUI manager (alternative to _Ready).
		/// </summary>
		public void Start()
		{
			_log.Information("GuiManager.Start() called");
		}

		/// <summary>
		/// Add a monitor to the GUI.
		/// </summary>
		public void AddMonitor(int monitorIndex, bool hasViewer, bool hasPatient)
		{
			_log.Information("AddMonitor: Index={index}, HasViewer={hasViewer}, HasPatient={hasPatient}", 
				monitorIndex, hasViewer, hasPatient);
		}

		public override void _ExitTree()
		{
			if (_mediator != null)
			{
				_mediator.command_triggered_v2 -= OnCommandExecuted;
			}
		}
	}
}
