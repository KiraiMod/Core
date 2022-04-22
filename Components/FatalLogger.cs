using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Runtime.InteropServices;

namespace KiraiMod.Core.Components
{
    public static class FatalLogger
    {
        public static ConfigEntry<bool> enabled = Plugin.Configuration.Bind("Logging.Fatal", "Enabled", true, "Should the fatal logger show a MessageBox when a fatal error is encountered");
        public static Listener listener;

        static FatalLogger() => enabled.SettingChanged += ((EventHandler)((sender, args) => listener = enabled.Value ? new() : null)).Invoke();

        public class Listener : ILogListener, IDisposable
        {
            public LogLevel LogLevelFilter => LogLevel.Fatal;

            public Listener() => Logger.Listeners.Add(this);

            public void Dispose() => Logger.Listeners.Remove(this);

            public void LogEvent(object sender, LogEventArgs eventArgs)
            {
                string msg = eventArgs.Source.SourceName + ": " + eventArgs.Data;

                MessageBox(IntPtr.Zero, msg, "A fatal error has occurred", 0);
                Environment.FailFast(msg);
            }

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
        }
    }
}
