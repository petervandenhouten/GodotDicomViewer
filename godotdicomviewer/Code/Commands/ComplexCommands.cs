using Godot;

namespace GodotDicomViewer.Code.Commands
{
	/// <summary>
	/// Complex command: Zoom image
	/// Requires access to: ImageViewer, context
	/// </summary>
	public partial class ZoomInCommand : CommandBase
	{
		private const float ZOOM_FACTOR = 1.2f;

		public override string CommandID => "ZoomIn";
		public override string Caption => "Zoom In";
		public override string Tooltip => "Zoom in on the current image";
		public override string Category => "View";
		public override CommandType Type => CommandType.Toggle;
		public override string HelpText => "Increases the magnification level of the displayed image by 20%.";

		public override void Execute(CommandContext context)
		{
			if (context.CurrentImageViewer == null)
			{
				context.Cancel("No image viewer available");
				return;
			}

			if (context.CurrentImage == null)
			{
				context.Cancel("No image loaded");
				return;
			}

			try
			{
				float currentZoom = context.CurrentImageViewer.GetZoomLevel();
				float newZoom = currentZoom * ZOOM_FACTOR;
				
				context.CurrentImageViewer.SetZoomLevel(newZoom);
				context.SetStatus($"Zoom: {newZoom:F1}x");
			}
			catch (System.Exception ex)
			{
				context.Cancel($"Zoom failed: {ex.Message}");
			}
		}
	}

	/// <summary>
	/// Complex command: Load DICOM study
	/// Requires access to: Data provider, current patient, image viewer, dialog manager
	/// </summary>
	public partial class LoadStudyCommand : CommandBase
	{
		public override string CommandID => "LoadStudy";
		public override string Caption => "Load Study";
		public override string Tooltip => "Load a DICOM study for the current patient";
		public override string Category => "File";
		public override CommandType Type => CommandType.Action;
		public override bool RequiresSelection => true;
		public override string HelpText => "Opens a DICOM study in the viewer. Requires a patient to be selected.";

		public override void Execute(CommandContext context)
		{
			if (context.CurrentPatient == null)
			{
				context.ShowMessage("Error", "No patient selected. Please select a patient first.");
				return;
			}

			if (context.DataProvider == null)
			{
				context.Cancel("Data provider not available");
				return;
			}

			if (context.DialogManager == null)
			{
				context.Cancel("Dialog manager not available");
				return;
			}

			try
			{
				context.SetStatus("Loading study...");
				context.SetStatus("Study loading in progress");

				// Simulate loading - in real code, this would query data provider
				var study = context.DataProvider.GetStudy("study_123");

				if (study == null)
				{
					context.ShowMessage("Error", "Study not found");
					return;
				}

				context.CurrentStudy = study;
				context.SetStatus($"Study loaded successfully");
			}
			catch (System.Exception ex)
			{
				context.Cancel($"Failed to load study: {ex.Message}");
				context.ShowMessage("Error", ex.Message);
			}
		}
	}

	/// <summary>
	/// Complex command: Apply window/level to image
	/// Requires access to: ImageViewer, user input from dialog
	/// </summary>
	public partial class AdjustWindowLevelCommand : CommandBase
	{
		public override string CommandID => "AdjustWindowLevel";
		public override string Caption => "Window/Level";
		public override string Tooltip => "Adjust window and level settings for DICOM image";
		public override string Category => "View";
		public override CommandType Type => CommandType.Action;
		public override string HelpText => "Modify the window and level values to optimize image contrast and brightness.";

		public override void Execute(CommandContext context)
		{
			if (context.CurrentImageViewer == null)
			{
				context.Cancel("No image viewer available");
				return;
			}

			if (context.CurrentImage == null)
			{
				context.Cancel("No image loaded");
				return;
			}

			if (context.DialogManager == null)
			{
				context.Cancel("Dialog manager not available");
				return;
			}

			try
			{
				// Get user input
				string input = context.DialogManager.ShowInputDialog(
					"Window/Level Settings",
                    "Enter window value (0-100):"
				);

				if (string.IsNullOrEmpty(input))
				{
					return;  // User cancelled
				}

				if (float.TryParse(input, out float window))
				{
					context.SetStatus($"Window/Level adjusted: {window}");
					// In real code, apply window/level to image viewer
				}
				else
				{
					context.ShowMessage("Error", "Invalid window value. Please enter a number.");
				}
			}
			catch (System.Exception ex)
			{
				context.Cancel($"Failed to adjust window/level: {ex.Message}");
			}
		}
	}

	/// <summary>
	/// Complex command: Measure distance on image
	/// Requires access to: ImageViewer, scene nodes, data storage
	/// </summary>
	public partial class MeasureCommand : CommandBase
	{
		public override string CommandID => "Measure";
		public override string Caption => "Measure";
		public override string Tooltip => "Measure distance on the current image";
		public override string Category => "Tools";
		public override CommandType Type => CommandType.Toggle;
		public override string HelpText => "Activate measurement tool to measure distances on DICOM images.";

		public override void Execute(CommandContext context)
		{
			if (context.CurrentImageViewer == null)
			{
				context.Cancel("No image viewer available");
				return;
			}

			if (context.CurrentImage == null)
			{
				context.Cancel("No image loaded");
				return;
			}

			try
			{
				// Find or create measurement overlay in scene
				var measurementPanel = context.FindNode<CanvasItem>("MeasurementPanel");
				
				if (measurementPanel != null)
				{
					bool isVisible = measurementPanel.Visible;
					measurementPanel.Visible = !isVisible;
					context.SetStatus(measurementPanel.Visible ? "Measure tool: ON" : "Measure tool: OFF");
				}
			}
			catch (System.Exception ex)
			{
				context.Cancel($"Failed to activate measurement tool: {ex.Message}");
			}
		}
	}
}
