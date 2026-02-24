using Godot;

namespace GodotDicomViewer.Code.Commands
{
	/// <summary>
	/// Example command: Exit application
	/// </summary>
	public partial class ExitCommand : CommandBase
	{
		public override string CommandID => "Exit";
		public override string Caption => "Exit";
		public override string Tooltip => "Exit the application";
		public override string Category => "File";
		public override CommandType Type => CommandType.Action;
		public override string HelpText => "Closes the application and exits.";
		public override bool RequiresUser => true;  // Should confirm before exiting
	}
}
