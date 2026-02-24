# Integration Guide: Connecting Complex Commands to Your DICOM Viewer

## Quick Start

This guide shows how to integrate the command system with your existing DICOM viewer application.

## Step 1: Create Interface Implementations

### IImageViewer Implementation

```csharp
// In your ImageViewerControl class
public partial class DicomImageViewer : Control, IImageViewer
{
    private float _currentZoom = 1.0f;
    private Vector2 _panOffset = Vector2.Zero;
    private Image _currentImage;

    public void SetImage(object? image)
    {
        _currentImage = image as Image;
        ResetView();
        QueueRedraw();
    }

    public object? GetCurrentImage()
    {
        return _currentImage;
    }

    public float GetZoomLevel()
    {
        return _currentZoom;
    }

    public void SetZoomLevel(float level)
    {
        _currentZoom = Mathf.Max(0.1f, Mathf.Min(level, 10.0f));
        QueueRedraw();
    }

    public void ResetView()
    {
        _currentZoom = 1.0f;
        _panOffset = Vector2.Zero;
        QueueRedraw();
    }

    public (float x, float y) GetPanOffset()
    {
        return (_panOffset.X, _panOffset.Y);
    }

    public void SetPanOffset(float x, float y)
    {
        _panOffset = new Vector2(x, y);
        QueueRedraw();
    }
}
```

### IDataProvider Implementation

```csharp
// In your PatientDataManager class
public class PatientDataManager : IDataProvider
{
    private Dictionary<string, PatientData> _patients = new();
    private Dictionary<string, StudyData> _studies = new();
    private Dictionary<string, SeriesData> _series = new();

    public PatientData? GetPatient(string patientID)
    {
        _patients.TryGetValue(patientID, out var patient);
        return patient;
    }

    public StudyData? GetStudy(string studyID)
    {
        _studies.TryGetValue(studyID, out var study);
        return study;
    }

    public SeriesData? GetSeries(string seriesID)
    {
        _series.TryGetValue(seriesID, out var series);
        return series;
    }

    public object? GetImage(string imageID)
    {
        // Return DICOM image by ID
        return null;
    }

    public void SaveData(object data)
    {
        if (data is PatientData patient)
        {
            _patients[patient.PatientID] = patient;
        }
    }
}
```

### IDialogManager Implementation

```csharp
// In your DialogManager class
public class DialogManager : IDialogManager
{
    private FileDialog _fileDialog;
    private ConfirmationDialog _confirmDialog;
    private InputDialog _inputDialog;

    public void ShowDialog(string title, string message)
    {
        var dialog = new AcceptDialog();
        dialog.Title = title;
        dialog.DialogText = message;
        dialog.AddToTree();  // or GetTree().Root.AddChild(dialog)
        dialog.PopupCentered();
    }

    public bool ShowConfirmation(string title, string message)
    {
        _confirmDialog = new ConfirmationDialog();
        _confirmDialog.Title = title;
        _confirmDialog.DialogText = message;
        // Would need to implement async/await pattern or callback
        return false;
    }

    public string ShowInputDialog(string title, string prompt)
    {
        _inputDialog = new InputDialog();
        _inputDialog.Title = title;
        _inputDialog.PromptText = prompt;
        // Would need to implement async/await pattern or callback
        return "";
    }
}
```

### IStatusBar Implementation

```csharp
// In your StatusBarControl class
public class StatusBarControl : HBoxContainer, IStatusBar
{
    private Label _messageLabel;
    private ProgressBar _progressBar;

    public override void _Ready()
    {
        _messageLabel = GetNode<Label>("MessageLabel");
        _progressBar = GetNode<ProgressBar>("ProgressBar");
    }

    public void SetText(string message)
    {
        _messageLabel.Text = message;
    }

    public void SetProgress(int percentComplete)
    {
        _progressBar.Value = Mathf.Clamp(percentComplete, 0, 100);
        _progressBar.Show();
    }

    public void ClearProgress()
    {
        _progressBar.Hide();
    }
}
```

## Step 2: Set Up Command Context in Your Main GUI Manager

```csharp
public partial class GodotDicomViewerMain : Control
{
    private MediatorV2 _mediator;
    private CommandContext _commandContext;
    private DicomImageViewer _imageViewer;
    private PatientDataManager _dataProvider;
    private DialogManager _dialogManager;
    private StatusBarControl _statusBar;

    public override void _Ready()
    {
        // Get references to UI components
        _imageViewer = GetNode<DicomImageViewer>("VBoxContainer/ImageViewer");
        _statusBar = GetNode<StatusBarControl>("VBoxContainer/StatusBar");
        
        // Initialize managers
        _mediator = new MediatorV2();
        _dataProvider = new PatientDataManager();
        _dialogManager = new DialogManager();

        // Build command context with all systems
        _commandContext = new CommandContext()
            .WithSceneRoot(GetNode("/root"))
            .WithMediator(_mediator)
            .WithImageViewer(_imageViewer)
            .WithDataProvider(_dataProvider)
            .WithDialogManager(_dialogManager)
            .WithStatusBar(_statusBar);

        // Subscribe to command events
        _mediator.command_triggered_v2 += OnCommandTriggered;

        GD.Print("Command system initialized");
    }

    private void OnCommandTriggered(object sender, ICommand cmd)
    {
        try
        {
            cmd.Execute(_commandContext);
        }
        catch (Exception ex)
        {
            _statusBar.SetText($"Error: {ex.Message}");
        }
    }
}
```

## Step 3: Wire CommandButtons in Your Godot Scenes

In your `.tscn` file, add CommandButtons and set their command IDs:

```gdscript
[node name="ToolbarContainer" type="HBoxContainer"]
layout_mode = 2

[node name="ZoomInButton" type="Button" node_paths=["mediator"]]
text = "Zoom In"
script = SubResource("res://addons/command_button/CommandButton.cs")
mediator = NodePath("../../GuiManager")
command_id = "ZoomIn"

[node name="ZoomOutButton" type="Button" node_paths=["mediator"]]
text = "Zoom Out"
command_id = "ZoomOut"

[node name="MeasureButton" type="Button" node_paths=["mediator"]]
text = "Measure"
command_id = "Measure"
```

## Step 4: Create Application-Specific Commands

### Example: Pan Image Command

```csharp
public partial class PanCommand : CommandBase
{
    public enum Direction { Left, Right, Up, Down }
    
    private Direction _direction;
    private const float PAN_DISTANCE = 10f;

    public PanCommand(Direction direction)
    {
        _direction = direction;
    }

    public override string CommandID => $"Pan{_direction}";
    public override string Caption => $"Pan {_direction}";
    public override string Category => "View";

    public override void Execute(CommandContext context)
    {
        if (context.CurrentImageViewer == null)
        {
            context.Cancel("No image viewer");
            return;
        }

        var (x, y) = context.CurrentImageViewer.GetPanOffset();

        var (newX, newY) = _direction switch
        {
            Direction.Left => (x - PAN_DISTANCE, y),
            Direction.Right => (x + PAN_DISTANCE, y),
            Direction.Up => (x, y - PAN_DISTANCE),
            Direction.Down => (x, y + PAN_DISTANCE),
            _ => (x, y)
        };

        context.CurrentImageViewer.SetPanOffset(newX, newY);
        context.SetStatus($"Panned {_direction}");
    }
}
```

### Example: Export DICOM Command

```csharp
public partial class ExportDicomCommand : CommandBase
{
    public override string CommandID => "ExportDicom";
    public override string Caption => "Export as...";
    public override string Category => "File";
    public override bool RequiresSelection => true;

    public override void Execute(CommandContext context)
    {
        if (context.CurrentImage == null)
        {
            context.Cancel("No image selected");
            return;
        }

        if (context.DialogManager == null)
        {
            context.Cancel("No dialog manager");
            return;
        }

        try
        {
            context.SetStatus("Preparing export...");

            var format = context.DialogManager.ShowInputDialog(
                "Export Format",
                "PNG, JPEG, or DICOM?"
            );

            if (string.IsNullOrEmpty(format))
                return;

            context.SetStatus($"Exporting to {format}...");
            
            // Export logic here
            context.SetStatus($"Exported successfully as {format}");
        }
        catch (Exception ex)
        {
            context.Cancel($"Export failed: {ex.Message}");
        }
    }
}
```

### Example: Move to Next/Previous Series

```csharp
public partial class NavigateSeriesCommand : CommandBase
{
    public enum Direction { Next, Previous }
    
    private Direction _direction;

    public NavigateSeriesCommand(Direction direction)
    {
        _direction = direction;
    }

    public override string CommandID => $"Series{_direction}";
    public override string Caption => $"{_direction} Series";
    public override string Category => "Navigation";

    public override void Execute(CommandContext context)
    {
        if (context.DataProvider == null)
        {
            context.Cancel("No data provider");
            return;
        }

        if (context.CurrentStudy == null)
        {
            context.Cancel("No study loaded");
            return;
        }

        try
        {
            context.SetStatus($"Loading {_direction} series...");
            
            // Get next/previous series from study
            var series = GetNextSeries(context.CurrentStudy, context.CurrentSeries, _direction);
            
            if (series == null)
            {
                context.SetStatus("No more series");
                return;
            }

            context.CurrentSeries = series;
            // Load first image of series
            var firstImage = context.DataProvider.GetImage(series.FirstImageID);
            context.CurrentImageViewer?.SetImage(firstImage);
            
            context.SetStatus($"Series: {series.SeriesDescription}");
        }
        catch (Exception ex)
        {
            context.Cancel($"Navigation failed: {ex.Message}");
        }
    }

    private SeriesData? GetNextSeries(StudyData study, SeriesData? current, Direction dir)
    {
        // Implement series navigation logic
        return null;
    }
}
```

## Step 5: Register New Commands

Update `CommandRegistry.cs`:

```csharp
public CommandRegistry()
{
    // File operations
    RegisterCommand<ExitCommand>();
    RegisterCommand<LoadStudyCommand>();
    RegisterCommand<ExportDicomCommand>();
    
    // View operations
    RegisterCommand<ZoomCommand>();
    RegisterCommand<ZoomInCommand>();
    RegisterCommand<AdjustWindowLevelCommand>();
    RegisterCommand<MeasureCommand>();
    RegisterCommand<PanCommand>();
    
    // Navigation
    RegisterCommand<NavigateSeriesCommand>();
}
```

## Step 6: Test Integration

### Manual Test

```gdscript
func _ready():
    var mediator = GuiManager.get_mediator()
    var cmd = mediator.GetCommand("ZoomIn")
    if cmd:
        cmd.Execute(GuiManager.get_command_context())
```

### Automated Test

```csharp
[Test]
public void TestZoomInCommand()
{
    var context = new CommandContext()
        .WithImageViewer(new MockImageViewer())
        .WithStatusBar(new MockStatusBar());

    var cmd = new ZoomInCommand();
    cmd.Execute(context);

    Assert.That(context.IsCancelled, Is.False);
}
```

## Troubleshooting

### Command Not Found
- Check command ID is spelled correctly
- Verify command is registered in `CommandRegistry`
- Use `mediator.GetAllCommands()` to list available commands

### Null Reference in Context
- Build context with all required systems before executing commands
- Check systems are properly initialized (`_Ready()` called)
- Add null checks in command Execute() method

### Button Not Showing Metadata
- Ensure MediatorV2 is in scene before CommandButton
- Check command ID matches exactly (case-sensitive)
- Verify command is registered

### Performance Issues
- Command registry caches instances - commands are singletons
- For data-heavy operations, consider lazy loading in context
- Use `SetStatus()` to show progress on long operations

---

## Summary

You now have:
1. ✅ Type-safe command system with metadata
2. ✅ Complex commands with multi-system access
3. ✅ Automatic UI metadata application
4. ✅ Comprehensive error handling
5. ✅ Example implementations and patterns
6. ✅ Integration blueprint for your DICOM viewer

**Next**: Implement the interfaces and connect to your existing DICOM viewer application!
