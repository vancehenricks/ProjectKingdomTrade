using DebugHandler;
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

        CDebug.Log(this, "get-revision.bat path=" +  path, LogType.Warning);

        Process.Start("cmd.exe", "/C " + path +  @"\get-revision.bat");

        string revision = File.ReadAllText("revision");
        string date = DateTime.UtcNow.ToString("ddMMyyyy");
        string version = date + "." + revision;

        CDebug.Log(this, "Generating version: " + version, LogType.Warning);

        PlayerSettings.bundleVersion = version;
    }
}
