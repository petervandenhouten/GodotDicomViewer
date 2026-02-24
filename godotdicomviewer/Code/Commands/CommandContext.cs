using Godot;
using System;

namespace GodotDicomViewer.Code.Commands
{
    /// <summary>
    /// Context passed to commands, providing access to various systems, models, and UI elements.
    /// This allows complex commands to interact with images, data, and scene graph.
    /// </summary>
    public class CommandContext
    {
        // UI and Scene Access
        public Node SceneRoot { get; set; }
        public IMediator Mediator { get; set; }
        private Node _callerNode;  // Node that triggered the command
        
        // Image/Viewer Access
        public IImageViewer CurrentImageViewer { get; set; }
        public object CurrentImage { get; set; }  // DICOM image or texture
        
        // Data Access
        public IDataProvider DataProvider { get; set; }
        public object CurrentPatient { get; set; }
        public object CurrentStudy { get; set; }
        public object CurrentSeries { get; set; }
        
        // UI State
        public IDialogManager DialogManager { get; set; }
        public IStatusBar StatusBar { get; set; }
        
        // Command execution state
        public bool IsCancelled { get; set; }
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Constructor for building context with fluent API
        /// </summary>
        public CommandContext()
        {
        }

        /// <summary>
        /// Fluent builder methods for convenience
        /// </summary>
        public CommandContext WithSceneRoot(Node root)
        {
            SceneRoot = root;
            return this;
        }

        public CommandContext WithMediator(IMediator mediator)
        {
            Mediator = mediator;
            return this;
        }

        public CommandContext WithImageViewer(IImageViewer viewer)
        {
            CurrentImageViewer = viewer;
            return this;
        }

        public CommandContext WithDataProvider(IDataProvider provider)
        {
            DataProvider = provider;
            return this;
        }

        public CommandContext WithDialogManager(IDialogManager dialogManager)
        {
            DialogManager = dialogManager;
            return this;
        }

        public CommandContext WithStatusBar(IStatusBar statusBar)
        {
            StatusBar = statusBar;
            return this;
        }

        public CommandContext WithCallerNode(Node caller)
        {
            _callerNode = caller;
            return this;
        }

        /// <summary>
        /// Helper to find a node in the scene tree
        /// </summary>
        public T FindNode<T>(string nodePath) where T : Node
        {
            if (SceneRoot == null) return null;
            var node = SceneRoot.GetNodeOrNull(nodePath);
            return node as T;
        }

        /// <summary>
        /// Helper to show a message to the user
        /// </summary>
        public void ShowMessage(string title, string message)
        {
            DialogManager?.ShowDialog(title, message);
        }

        /// <summary>
        /// Helper to set status bar text
        /// </summary>
        public void SetStatus(string message)
        {
            StatusBar?.SetText(message);
        }

        /// <summary>
        /// Find a node scoped to the caller's subtree first,
        /// then fall back to global groups if not found locally.
        /// </summary>
        public T FindNodeInCallerScope<T>(string group) where T : Node
        {
            if (_callerNode == null)
                return SceneRoot?.GetTree().GetFirstNodeInGroup(group) as T;

            // Get the owner (scene root) of the caller
            var owner = _callerNode.GetOwner();
            if (owner != null)
            {
                // Search for nodes in the group within this scene's subtree
                var allInGroup = _callerNode.GetTree().GetNodesInGroup(group);
                foreach (var node in allInGroup)
                {
                    // Check if node belongs to the same scene (has same owner)
                    if (node.GetOwner() == owner || node == owner)
                    {
                        return node as T;
                    }
                }
            }

            // Fall back to global search if not found in scoped search
            return SceneRoot?.GetTree().GetFirstNodeInGroup(group) as T;
        }

        /// <summary>
        /// Cancel command execution with error message
        /// </summary>
        public void Cancel(string reason)
        {
            IsCancelled = true;
            ErrorMessage = reason;
        }
    }

    /// <summary>
    /// Interface for image viewer access
    /// </summary>
    public interface IImageViewer
    {
        void SetImage(object image);
        object GetCurrentImage();
        float GetZoomLevel();
        void SetZoomLevel(float zoom);
        void ResetView();
        Vector2 GetPanOffset();
        void SetPanOffset(Vector2 offset);
    }

    /// <summary>
    /// Interface for data access
    /// </summary>
    public interface IDataProvider
    {
        object GetPatient(string patientId);
        object GetStudy(string studyId);
        object GetSeries(string seriesId);
        object GetImage(string imageId);
        void SaveData(object data);
    }

    /// <summary>
    /// Interface for dialog management
    /// </summary>
    public interface IDialogManager
    {
        void ShowDialog(string title, string message);
        bool ShowConfirmation(string title, string message);
        string ShowInputDialog(string title, string label);
    }

    /// <summary>
    /// Interface for status bar
    /// </summary>
    public interface IStatusBar
    {
        void SetText(string text);
        void SetProgress(float percent);
        void ClearProgress();
    }
}
