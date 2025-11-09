namespace CompCube.Extensions;

public static class StringExtensions
{
    public static string FormatWithHtmlColor(this string str, string color) => $"<color={color}>{str}</color>";
}