// C# example.
using UnityEditor;
using System.Diagnostics;

public class BuildTools
{
    [MenuItem("Build Tools/Release - All Platforms", priority = 1)]
    public static void BuildAllPlatforms()
    {
        AllPlatforms.Build();
    }

    [MenuItem("Build Tools/Windows Build With Postprocess", priority = 51)]
    public static void BuildWindows()
    {
        Windows.BuildGame();
    }

    [MenuItem("Build Tools/Linux Build With Postprocess", priority = 51)]
    public static void BuildLinux()
    {
        Linux.BuildGame();
    }

    [MenuItem("Build Tools/Mac Build With Postprocess", priority = 51)]
    public static void BuildMac()
    {
        Mac.BuildGame();
    }
}