using GodotDicomViewer.Code.Interfaces;

namespace GodotDicomViewer.Configuration
{
    internal class TestConfiguration : IConfiguration
    {
        public int NumberOfMonitors { get; set; } = 2;
    }
}
