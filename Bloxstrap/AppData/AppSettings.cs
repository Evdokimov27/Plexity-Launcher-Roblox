using Plexity.Enums;

namespace Plexity.AppData
{
    public class AppSettings
    {
        public string CustomFontLocation { get; set; } = string.Empty;
        public CursorType CursorType { get; set; } = CursorType.Default;
        public bool UseFastFlagManager { get; set; }
        public bool PlexityRPCReal { get; set; }
        public bool WPFSoftwareRender { get; set; }
        public string Locale { get; set; } = "nil";
        public string? SelectedCustomTheme { get; set; }
    }
}