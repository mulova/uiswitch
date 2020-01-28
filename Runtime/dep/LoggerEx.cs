#if STANDALONE
using System;
using UnityEngine;

namespace mulova.ui
{
    public static class LoggerEx
    {
        public const string TAG = "Unity";

        public static bool IsLoggable(this ILogger l, LogType type)
        {
            if (!l.logEnabled)
            {
                return false;
            }
            return l.IsLogTypeAllowed(type);
        }

        public static void Debug(this ILogger l, string format, object arg1)
        {
            if (!l.IsLogTypeAllowed(LogType.Log))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1));
        }

        public static void Debug(this ILogger l, string format, object arg1, object arg2)
        {
            if (!l.IsLogTypeAllowed(LogType.Log))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2));
        }

        public static void Debug(this ILogger l, string format, object arg1, object arg2, object arg3)
        {
            if (!l.IsLogTypeAllowed(LogType.Log))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2, arg3));
        }

        public static void Debug(this ILogger l, string format, params object[] args)
        {
            if (!l.IsLoggable(LogType.Log))
            {
                return;
            }
            l.LogFormat(LogType.Log, format, args);
        }

        public static void Warn(this ILogger l, string format, object arg1)
        {
            if (!l.IsLogTypeAllowed(LogType.Warning))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1));
        }

        public static void Warn(this ILogger l, string format, object arg1, object arg2)
        {
            if (!l.IsLogTypeAllowed(LogType.Warning))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2));
        }

        public static void Warn(this ILogger l, string format, object arg1, object arg2, object arg3)
        {
            if (!l.IsLogTypeAllowed(LogType.Warning))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2, arg3));
        }

        public static void Warn(this ILogger l, string format, params object[] args)
        {
            if (!l.IsLoggable(LogType.Warning))
            {
                return;
            }
            l.LogFormat(LogType.Warning, format, args);
        }

        public static void Error(this ILogger l, string format, object arg1)
        {
            if (!l.IsLogTypeAllowed(LogType.Error))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1));
        }

        public static void Error(this ILogger l, string format, object arg1, object arg2)
        {
            if (!l.IsLogTypeAllowed(LogType.Error))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2));
        }

        public static void Error(this ILogger l, string format, object arg1, object arg2, object arg3)
        {
            if (!l.IsLogTypeAllowed(LogType.Error))
            {
                return;
            }
            l.Log(TAG, string.Format(format, arg1, arg2, arg3));
        }

        public static void Error(this ILogger l, string format, params object[] args)
        {
            if (!l.IsLoggable(LogType.Error))
            {
                return;
            }
            l.LogFormat(LogType.Error, format, args);
        }

        public static void Error(this ILogger l, Exception ex)
        {
            if (!l.IsLoggable(LogType.Exception))
            {
                return;
            }
            l.LogException(ex);
        }
    }

}

#endif