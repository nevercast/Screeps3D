// C# example.
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Semver;

public class AllPlatforms
{
    public static void Build()
    {
        var releaseOutputFolder = EditorUtility.SaveFolderPanel("Make a folder following semver for a new release", "", "");

        string versionString = releaseOutputFolder;
        versionString = versionString.Substring(versionString.LastIndexOf("/")+1);
        // Get filename.
        //string path = EditorUtility.SaveFolderPanel("Choose build output folder", "", "");

        var version = SemVersion.Parse(versionString);

        //if (int.TryParse(version.Build, out var build))
        //{
        //    // bump build version
        //}

        PlayerSettings.bundleVersion = version.ToString();


        var currentDirectory = Directory.GetCurrentDirectory();
        string path = $"{currentDirectory}/Build/{version}";

        //Windows.BuildGame(path + "/Windows");
        //Windows.Release(path + "/Windows", version.ToString(), releaseOutputFolder);
        //Linux.BuildGame(path + "/Linux");
        //Linux.Release(path + "/Linux", version.ToString(), releaseOutputFolder);
        //Mac.BuildGame(path + "/Mac");
        Mac.Release(path + "/Mac", version.ToString(), releaseOutputFolder);

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/Screeps3D.exe";
        //proc.Start();
    }



}