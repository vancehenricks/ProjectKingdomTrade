using DebugHandler;
using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Runtime.InteropServices;

class VersionHandler : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            GenerateVersion("get-revision.bat"/*, "cmd.exe", "/c", path*/);
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            GenerateVersion("get-revision.sh"/*, "xterm", "-hold -e", path*/);
        }
    }

    private void GenerateVersion(string fileName/*, string args, string path, string argsPreFix = " \"", string argsPosFix = "\""*/)
    {
        string path = Application.dataPath.Remove(Application.dataPath.IndexOf("Assets"),@"Assets".Length);
        CDebug.Log(this,"Executing " + fileName + " path=" +  path, LogType.Warning);
        string revision;

        using (Process process = new Process())
        {
            process.StartInfo.FileName = fileName;
            //process.StartInfo.Arguments = args + argsPreFix + Path.Combine(path, fileName) + argsPosFix;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = path;
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
