namespace Logger
{
    public interface ILogCategory
    {
        string Id { get; }
        string Tag { get; }
        string HexColorDarkTheme { get; }
        string HexColorLightTheme { get; }
    }
}