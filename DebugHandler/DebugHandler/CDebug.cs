using System;
using UnityEngine;

namespace DebugHandler
{
    public static class CDebug
    {
        public static void Log(System.Object obj, object debug, LogType logtype = LogType.Log)
        {
            string output = DateTime.UtcNow.ToUniversalTime() + "|";
            string objName = "Undefined";

            if (obj != null)
            {
                objName = obj.ToString();
            }

            output += logtype + "|" + objName + "|" + debug;

            switch (logtype)
            {
                case LogType.Assert:
                    Debug.LogAssertion(debug);
                    break;
                case LogType.Exception:
                    Debug.LogException((Exception)debug);
                    break;
                case LogType.Error:
                    Debug.LogError(output);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(output);
                    break;
                default:
                    Debug.Log(output);
                    break;
            }

        }
    }
}
