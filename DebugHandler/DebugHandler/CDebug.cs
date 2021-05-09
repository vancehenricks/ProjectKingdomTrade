/* Copyright 2021 by Vance Henricks Patual - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Vance Henricks Patual <vpatual@gmail.com>, April 2021
 */

using System;
using UnityEngine;

namespace DebugHandler
{
    public static class CDebug
    {
        public delegate string ExtraParams();
        public static ExtraParams extraParams;

        public static void Log(System.Object obj, object debug, LogType logtype = LogType.Log, 
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0, 
            [System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
        {
            if (!Debug.unityLogger.IsLogTypeAllowed(logtype)) return;

            string output = logtype + "|" + DateTime.UtcNow + "|";
            string objName = "Undefined";

            if (obj != null)
            {
                objName = obj.ToString();
            }

            string exParam = "";

            if (extraParams != null)
            {
                exParam = extraParams() + "|";
            }

            output += exParam + objName + " " + methodName + ":" + lineNumber + "|" + debug;


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
