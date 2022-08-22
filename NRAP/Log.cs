﻿
using System;
using System.Diagnostics;
//using UnityEngine;
using KSP_Log;

namespace NRAP
{
    public static class Log
    {
#if false
        static string PREFIX = "";

        public static void setTitle(string t)
        {
            PREFIX = t + ": ";
        }
        public enum LEVEL
        {
            OFF = 0,
            ERROR = 1,
            WARNING = 2,
            INFO = 3,
            DETAIL = 4,
            TRACE = 5
        };

        public static LEVEL level = LEVEL.INFO;



        public static LEVEL GetLevel()
        {
            return level;
        }

        public static void SetLevel(LEVEL level)
        {
            UnityEngine.Debug.Log("log level " + level);
            Log.level = level;
        }

        public static LEVEL GetLogLevel()
        {
            return level;
        }

        private static bool IsLevel(LEVEL level)
        {
            return level == Log.level;
        }

        public static bool IsLogable(LEVEL level)
        {
            return level <= Log.level;
        }

        public static void Trace(String msg)
        {
            if (IsLogable(LEVEL.TRACE))
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        public static void Detail(String msg)
        {
            if (IsLogable(LEVEL.DETAIL))
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        //        [ConditionalAttribute("DEBUG")]
        public static void Info(String msg)
        {
#if DEBUG
            if (IsLogable(LEVEL.INFO))
#endif
            {
                UnityEngine.Debug.Log(PREFIX + msg);
            }
        }

        //        [ConditionalAttribute("DEBUG")]
        public static void Test(String msg)
        {
            //if (IsLogable(LEVEL.INFO))


            UnityEngine.Debug.LogWarning(PREFIX + "TEST:" + msg);

        }


        public static void Warning(String msg)
        {
            if (IsLogable(LEVEL.WARNING))
            {
                UnityEngine.Debug.LogWarning(PREFIX + msg);
            }
        }

        public static void Error(String msg)
        {
            if (IsLogable(LEVEL.ERROR))
            {
                UnityEngine.Debug.LogError(PREFIX + msg);
            }
        }

        public static void Exception(Exception e)
        {
            Log.Error("exception caught: " + e.GetType() + ": " + e.Message);
        }
#else
        static KSP_Log.Log log;
        static public void InitLog(string str)
        {
            if (log == null)
#if DEBUG
                log = new KSP_Log.Log(str, KSP_Log.Log.LEVEL.INFO);
#else
                log = new KSP_Log.Log(str, KSP_Log.Log.LEVEL.ERROR);
#endif

        }
        public static void Info(String msg)
        {
            log.Info(msg);
        }
        public static void Error(String msg)
        {
            log.Info(msg);
        }


#endif
    }
}
