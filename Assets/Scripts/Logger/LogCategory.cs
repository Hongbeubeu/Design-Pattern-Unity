namespace hcore.Logger
{
    public class LogCategory : ILogCategory
    {
        public string Id { get; }
        public string Tag { get; }
        public string HexColorDarkTheme { get; }
        public string HexColorLightTheme { get; }

        public LogCategory(string id, string tag, string hexColorDarkTheme, string hexColorLightTheme)
        {
            Id = id;
            Tag = tag;
            HexColorDarkTheme = hexColorDarkTheme;
            HexColorLightTheme = hexColorLightTheme;
        }
    }
}