﻿using DebugHandler;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class VersionHandler : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        string path = Application.dataPath.Remove(Application.dataPath.IndexOf("Assets"),@"Assets".Length);
        string fileName = "get-revision.bat";
        CDebug.Log(this,"Executing " + fileName + " path=" +  path, LogType.Warning);

        string revision;

        using (Process process = new Process())
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = @"/c " + Path.Combine(path, fileName);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            revision = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
        }

        string date = DateTime.UtcNow.ToString("ddMMyyyy");
        string version = date + "." + revision;

        CDebug.Log(this, "Generating version: " + version, LogType.Warning);

        PlayerSettings.bundleVersion = version;
    }
}
