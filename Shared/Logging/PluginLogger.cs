using System;
using System.Runtime.CompilerServices;
using VRage.Utils;

namespace Shared.Logging;

public class PluginLogger : LogFormatter, IPluginLogger
{
    public PluginLogger(string pluginName)
        : base($"{pluginName}: ") { }

    public bool IsTraceEnabled => MyLog.Default.LogEnabled;
    public bool IsDebugEnabled => MyLog.Default.LogEnabled;
    public bool IsInfoEnabled => MyLog.Default.LogEnabled;
    public bool IsWarningEnabled => MyLog.Default.LogEnabled;
    public bool IsErrorEnabled => MyLog.Default.LogEnabled;
    public bool IsCriticalEnabled => MyLog.Default.LogEnabled;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(Exception ex, string message, params object[] data)
    {
        if (!IsTraceEnabled)
            return;

        // Keen does not have a Trace log level, using Debug instead
        MyLog.Default.Log(MyLogSeverity.Debug, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(Exception ex, string message, params object[] data)
    {
        if (!IsDebugEnabled)
            return;

        MyLog.Default.Log(MyLogSeverity.Debug, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(Exception ex, string message, params object[] data)
    {
        if (!IsInfoEnabled)
            return;

        MyLog.Default.Log(MyLogSeverity.Info, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(Exception ex, string message, params object[] data)
    {
        if (!IsWarningEnabled)
            return;

        MyLog.Default.Log(MyLogSeverity.Warning, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(Exception ex, string message, params object[] data)
    {
        if (!IsErrorEnabled)
            return;

        MyLog.Default.Log(MyLogSeverity.Error, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Critical(Exception ex, string message, params object[] data)
    {
        if (!IsCriticalEnabled)
            return;

        MyLog.Default.Log(MyLogSeverity.Critical, "{0}", Format(ex, message, data));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Trace(string message, params object[] data)
    {
        Trace(null, message, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Debug(string message, params object[] data)
    {
        Debug(null, message, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Info(string message, params object[] data)
    {
        Info(null, message, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Warning(string message, params object[] data)
    {
        Warning(null, message, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Error(string message, params object[] data)
    {
        Error(null, message, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Critical(string message, params object[] data)
    {
        Critical(null, message, data);
    }
}
