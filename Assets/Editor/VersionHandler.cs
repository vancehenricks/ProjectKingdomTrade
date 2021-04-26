using DebugHandler;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class VersionHandler : IPostprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPostprocessBuild(BuildReport report)
    {
        string revision = File.ReadAllText("revision");

        string date = DateTime.UtcNow.ToString("ddMMyyyy");
        string version = date + "." + revision;

        CDebug.Log(this, "Generating version: " + version, LogType.Warning);

        PlayerSettings.bundleVersion = version;
    }
}
