using Godot;

namespace GodotDicomViewer.Code.Commands
{
    /// <summary>
    /// Example command: Toggle zoom functionality
    /// </summary>
    public partial class ZoomCommand : CommandBase
    {
        private bool _isZoomed = false;

        public override string CommandID => "Zoom";
        public override string Caption => "Zoom";
        public override string Tooltip => "Zoom in/out on DICOM image";
        public override string Category => "View";
        public override CommandType Type => CommandType.Toggle;
        public override string HelpText => "Toggle zoom mode for viewing details in medical images.";
        public override bool? IsToggled => _isZoomed;

        public void SetZoomState(bool zoomed)
        {
            _isZoomed = zoomed;
        }
    }
}
