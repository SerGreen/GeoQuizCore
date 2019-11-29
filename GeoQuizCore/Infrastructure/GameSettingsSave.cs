namespace GeoQuizCore.Infrastructure
{
    /// <summary>
    /// This class is a copy of GameSettings, but custom model binder does not work for this one
    /// And that's exactly what was needed
    /// </summary>
    public class GameSettingsSave : Models.GameSettings
    { }
}