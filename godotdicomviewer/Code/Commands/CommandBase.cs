using System;
using Godot;

namespace GodotDicomViewer.Code.Commands
{
	/// <summary>
	/// Base class for implementing commands with sensible defaults.
	/// </summary>
	public abstract partial class CommandBase : ICommand
	{
		public virtual string CommandID => GetType().Name;
		public virtual string Caption => CommandID;
		public virtual string Tooltip => "";
		public virtual Texture2D Icon => null;
		public virtual string Category => "General";
		public virtual CommandType Type => CommandType.Action;
		public virtual string HelpText => "";
		public virtual string HelpScenePath => "";
		public virtual bool IsEnabled => true;
		public virtual bool IsVisible => true;
		public virtual bool? IsToggled => null;
		public virtual bool RequiresUser => false;
		public virtual bool RequiresSelection => false;

		/// <summary>
		/// Override this method to implement command logic.
		/// Access models, images, and UI through the context.
		/// </summary>
		public virtual void Execute(CommandContext context)
		{
			// To be implemented by subclasses
		}
	}
}
