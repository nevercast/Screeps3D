// C# example.
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Semver;
using System.Collections.Generic;
using System;
using System.Text;

public class AllPlatforms
{
    public static void Build()
    {
        var timings = new Dictionary<string, TimeSpan>();

        var sw = new Stopwatch();
        sw.Start();
       
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

        sw.Restart();
        Windows.BuildGame(path + "/Windows");
        Windows.Release(path + "/Windows", version.ToString(), releaseOutputFolder);

        timings.Add("Windows Release", sw.Elapsed);
        sw.Restart();

        Linux.BuildGame(path + "/Linux");
        Linux.Release(path + "/Linux", version.ToString(), releaseOutputFolder);

        timings.Add("Linux Release", sw.Elapsed);
        sw.Restart();

        Mac.BuildGame(path + "/Mac");
        Mac.Release(path + "/Mac", version.ToString(), releaseOutputFolder);

        timings.Add("Mac Release", sw.Elapsed);
        sw.Restart();

        TimeSpan totalTime;
        var sb = new StringBuilder();

        foreach (var timing in timings)
        {
            totalTime += timing.Value;
            sb.AppendLine($"{timing.Key} {timing.Value}");
        }

        sb.AppendLine($"Total: {totalTime}");

        UnityEngine.Debug.Log(sb.ToString());

    }



}