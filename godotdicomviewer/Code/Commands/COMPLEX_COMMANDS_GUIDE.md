# Complex Commands Guide

This guide shows how to implement and use complex commands that require access to multiple systems (images, data, UI).

## Overview

Complex commands need access to:
- **Image Viewer**: Current image, zoom level, pan offset
- **Data Provider**: Patient, study, series information
- **Dialog Manager**: Confirmations, user input
- **Status Bar**: Progress updates, messages
- **Scene Graph**: Finding/creating UI nodes

All of this is provided through the `CommandContext` object.

## Example 1: Zoom Command

A simple command that modifies the image viewer:

```csharp
public partial class ZoomInCommand : CommandBase
{
    private const float ZOOM_FACTOR = 1.2f;

    public override string CommandID => "ZoomIn";
    public override string Caption => "Zoom In";
    public override string Category => "View";

    public override void Execute(CommandContext context)
    {
        // Validate preconditions
        if (context.CurrentImageViewer == null)
        {
            context.Cancel("No image viewer available");
            return;
        }

        try
        {
            // Access and modify viewer state
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
```

**Key Points:**
- Check preconditions (viewer available, image loaded)
- Use `context.CurrentImageViewer` to access viewer methods
- Use `context.SetStatus()` for user feedback
- Use `context.Cancel()` to report errors

---

## Example 2: Load Study Command

A complex command requiring data provider, user confirmation, and status updates:

```csharp
public partial class LoadStudyCommand : CommandBase
{
    public override string CommandID => "LoadStudy";
    public override string Caption => "Load Study";
    public override bool RequiresSelection => true;  // Can't run without selection

    public override void Execute(CommandContext context)
    {
        // Check prerequisites
        if (context.CurrentPatient == null)
        {
            context.ShowMessage("Error", "No patient selected");
            return;
        }

        if (context.DataProvider == null)
        {
            context.Cancel("Data provider not available");
            return;
        }

        try
        {
            // Show progress to user
            context.SetStatus("Loading study...");

            // Access data through provider
            var study = context.DataProvider.GetStudy("study_123");

            if (study == null)
            {
                context.ShowMessage("Error", "Study not found");
                return;
            }

            // Update context with loaded data
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
```

**Key Points:**
- Check `RequiresSelection` property prevents execution without data
- Use `context.DataProvider` to fetch data
- Use `context.ShowMessage()` for user dialogs
- Update context properties (`CurrentStudy`, `CurrentPatient`) to share state

---

## Example 3: Window/Level Command with User Input

A command that asks user for input:

```csharp
public partial class AdjustWindowLevelCommand : CommandBase
{
    public override string CommandID => "AdjustWindowLevel";
    
    public override void Execute(CommandContext context)
    {
        if (context.CurrentImageViewer == null)
        {
            context.Cancel("No image viewer available");
            return;
        }

        if (context.DialogManager == null)
        {
            context.Cancel("Dialog manager not available");
            return;
        }

        try
        {
            // Get user input through dialog manager
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
                // Apply to image viewer
            }
            else
            {
                context.ShowMessage("Error", "Invalid value");
            }
        }
        catch (System.Exception ex)
        {
            context.Cancel($"Failed: {ex.Message}");
        }
    }
}
```

**Key Points:**
- Use `context.DialogManager.ShowInputDialog()` for user input
- Handle user cancellation (null/empty response)
- Validate input before using

---

## Example 4: Measure Tool Command with Scene Navigation

A toggle command that finds or creates UI nodes:

```csharp
public partial class MeasureCommand : CommandBase
{
    public override string CommandID => "Measure";
    public override string Category => "Tools";
    public override CommandType Type => CommandType.Toggle;

    public override void Execute(CommandContext context)
    {
        if (context.CurrentImageViewer == null)
        {
            context.Cancel("No image viewer available");
            return;
        }

        try
        {
            // Find node in scene tree
            var measurementPanel = context.FindNode<CanvasItem>("MeasurementPanel");
            
            if (measurementPanel != null)
            {
                // Toggle visibility
                bool isVisible = measurementPanel.Visible;
                measurementPanel.Visible = !isVisible;
                
                context.SetStatus(
                    measurementPanel.Visible 
                        ? "Measure tool: ON" 
                        : "Measure tool: OFF"
                );
            }
        }
        catch (System.Exception ex)
        {
            context.Cancel($"Failed: {ex.Message}");
        }
    }
}
```

**Key Points:**
- Use `context.FindNode<T>()` to search scene tree for specific nodes
- Works with `CanvasItem` (base for UI nodes) not `Node`
- Toggle commands should update UI to reflect new state

---

## CommandContext API Reference

### Properties

```csharp
// Scene
Node SceneRoot { get; }
MediatorV2 Mediator { get; }

// Viewer & Image
IImageViewer CurrentImageViewer { get; set; }
// Represents current image (type depends on IImageViewer impl)
object? CurrentImage { get; set; }

// Data
IDataProvider DataProvider { get; set; }
PatientData CurrentPatient { get; set; }
StudyData CurrentStudy { get; set; }
SeriesData CurrentSeries { get; set; }

// UI
IDialogManager DialogManager { get; set; }
IStatusBar StatusBar { get; set; }

// State
bool IsCancelled { get; }
string ErrorMessage { get; }
```

### Methods

```csharp
// Fluent builders for initialization
CommandContext WithSceneRoot(Node root)
CommandContext WithMediator(MediatorV2 mediator)
CommandContext WithImageViewer(IImageViewer viewer)
CommandContext WithDataProvider(IDataProvider provider)
CommandContext WithDialogManager(IDialogManager dialogs)
CommandContext WithStatusBar(IStatusBar statusBar)

// Scene operations
T FindNode<T>(string name) where T : Node

// User feedback
void ShowMessage(string title, string message)
void SetStatus(string message)
void SetStatus(string message, int progress)  // 0-100
void ClearStatus()

// Command control
void Cancel(string errorMessage)  // Marks command as failed
```

### IImageViewer Interface

```csharp
public interface IImageViewer
{
    void SetImage(object? image);
    object? GetCurrentImage();
    
    float GetZoomLevel();
    void SetZoomLevel(float level);
    
    void ResetView();
    
    (float x, float y) GetPanOffset();
    void SetPanOffset(float x, float y);
}
```

### IDataProvider Interface

```csharp
public interface IDataProvider
{
    PatientData? GetPatient(string patientID);
    StudyData? GetStudy(string studyID);
    SeriesData? GetSeries(string seriesID);
    object? GetImage(string imageID);  // Type depends on image format
    
    void SaveData(object data);
}
```

### IDialogManager Interface

```csharp
public interface IDialogManager
{
    void ShowDialog(string title, string message);
    bool ShowConfirmation(string title, string message);
    string ShowInputDialog(string title, string prompt);
}
```

### IStatusBar Interface

```csharp
public interface IStatusBar
{
    void SetText(string message);
    void SetProgress(int percentComplete);  // 0-100
    void ClearProgress();
}
```

---

## Best Practices

### 1. Always Check Preconditions

```csharp
// Bad
public override void Execute(CommandContext context)
{
    float zoom = context.CurrentImageViewer.GetZoomLevel();  // Will crash if null!
}

// Good
public override void Execute(CommandContext context)
{
    if (context.CurrentImageViewer == null)
    {
        context.Cancel("No image viewer available");
        return;
    }
    float zoom = context.CurrentImageViewer.GetZoomLevel();
}
```

### 2. Use Fluent Builder Pattern for Initialization

```csharp
// In GuiManager or startup
var context = new CommandContext()
    .WithSceneRoot(GetNode("/root"))
    .WithMediator(mediator)
    .WithImageViewer(imageViewer)
    .WithDataProvider(dataProvider)
    .WithDialogManager(dialogManager)
    .WithStatusBar(statusBar);

// Pass to all commands
command.Execute(context);
```

### 3. Always Handle Exceptions

```csharp
try
{
    // Command logic
}
catch (System.Exception ex)
{
    context.Cancel($"Operation failed: {ex.Message}");
    context.ShowMessage("Error", ex.Message);
}
```

### 4. Use Command Metadata for Validation

```csharp
// Register command metadata
public override bool RequiresSelection => true;
public override bool RequiresUser => true;

// In mediator, check before executing
if (command.RequiresSelection && !HasSelection())
{
    commandButton.mark_as_invalid("Selection required");
    return;
}
```

### 5. Update Status Bar for Long Operations

```csharp
try
{
    context.SetStatus("Processing...");
    
    for (int i = 0; i < items.Count; i++)
    {
        ProcessItem(items[i]);
        context.SetStatus($"Processing item {i+1}/{items.Count}", 
                         (i + 1) * 100 / items.Count);
    }
    
    context.SetStatus("Done!");
}
finally
{
    context.ClearStatus();
}
```

---

## Registering New Commands

Add to `CommandRegistry.cs` in the constructor:

```csharp
public CommandRegistry()
{
    // File operations
    RegisterCommand<ExitCommand>();
    RegisterCommand<LoadStudyCommand>();
    
    // View operations
    RegisterCommand<ZoomCommand>();
    RegisterCommand<ZoomInCommand>();
    RegisterCommand<AdjustWindowLevelCommand>();
    RegisterCommand<MeasureCommand>();
}
```

The command will then be:
- Available to `CommandButton` components
- Listed in dynamic menus
- Type-safe in event handlers

---

## Consuming Commands

### Option 1: CommandButton (Automatic)

```gdscript
# In Godot scene
var button = $CommandButton
button.command_id = "ZoomIn"  # Auto-loads metadata
```

### Option 2: Manual in Code

```csharp
// Get command from registry
var command = mediator.GetCommand("ZoomIn");

if (command != null)
{
    var context = CreateCommandContext();
    command.Execute(context);
}
```

### Option 3: Event-Based

```csharp
// MediatorV2 fires event with command object
mediator.command_triggered_v2 += OnCommandTriggered;

private void OnCommandTriggered(object sender, ICommand command)
{
    if (command is ZoomInCommand zoom)
    {
        // Handle zoom-specific logic
    }
}
```

