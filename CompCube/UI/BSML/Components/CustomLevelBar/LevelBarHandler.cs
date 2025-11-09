using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.TypeHandlers;

namespace CompCube.UI.BSML.Components.CustomLevelBar;

[ComponentHandler(typeof(LevelBar))]
public class LevelBarHandler : TypeHandler<CustomLevelBar>
{
    public override void HandleType(BSMLParser.ComponentTypeWithData componentType, BSMLParserParams parserParams)
    {
        var levelBar = componentType.Component as CustomLevelBar;

        if (!componentType.Data.TryGetValue("name", out var name))
            throw new Exception("Level bar name never declared!");

        levelBar!.name = name;
    }

    public override Dictionary<string, Action<CustomLevelBar, string>> Setters => new();

    public override Dictionary<string, string[]> Props => new()
    {
        { "name", ["name"] }
    };
}