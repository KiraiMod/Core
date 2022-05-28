using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace KiraiMod.Core.Components
{
    [ModuleAPI.Module]
    public static class ScreenLogger
    {
        public static ConfigEntry<bool> enabled = Plugin.Configuration.Bind("Logging.Screen", "Enabled", true, "Should the on screen logger be created and used");
        public static ConfigEntry<bool> hightlightNames = Plugin.Configuration.Bind("Logging.Screen", "HightlightNames", true, "Should usernames get colored");
        public static ConfigEntry<LogLevel> level = Plugin.Configuration.Bind("Logging.Screen", "LogLevel", LogLevel.Message, "The levels that the logger should listen to");

        public static Listener listener;

        private static Font font;
        public static Font Font
        {
            get => font;
            set
            {
                if (font == value) return;
                font = value;

                if (listener != null)
                    listener.Log.font = font;
            }
        }

        static ScreenLogger() => Events.UIManagerLoaded += UILoaded;

        private static void UILoaded() => enabled.SettingChanged += ((EventHandler)((sender, args) => listener = enabled.Value ? new() : null)).Invoke();

        public static void DisplayOnScreen(string message, float duration = 3) => listener?.Display(message, duration);

        public sealed class Listener : ILogListener, IDisposable
        {
            public LogLevel LogLevelFilter => level.Value;

            public readonly Text Log;
            public readonly Queue<string> Lines = new();

            public Listener()
            {
                GameObject go = new("KiraiMod.Core.Log");
                Log = go.AddComponent<Text>();

                go.transform.SetParent(GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud").transform, false);
                go.transform.localPosition = new(15, 300);

                go.GetComponent<RectTransform>().sizeDelta = new Vector2(1_000, 30);

                Log.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                Log.horizontalOverflow = HorizontalWrapMode.Wrap;
                Log.verticalOverflow = VerticalWrapMode.Overflow;
                Log.alignment = TextAnchor.UpperLeft;
                Log.fontStyle = FontStyle.Bold;
                Log.supportRichText = true;
                Log.fontSize = 30;

                BepInEx.Logging.Logger.Listeners.Add(this);
            }

            public void Dispose()
            {
                BepInEx.Logging.Logger.Listeners.Remove(this);

                Log.transform.Destroy();
            }

            public void LogEvent(object sender, LogEventArgs eventArgs)
            {
                if (!Utils.Misc.IsMainThread) return;

                Display(eventArgs.Data.ToString(), 3);
            }

            public void Display(string message, float duration)
            {
                if (!Utils.Misc.IsMainThread)
                {
                    Console.WriteLine("ignore message due to wrong thread");
                    return;
                }

                if (hightlightNames.Value)
                    foreach (VRCPlayerApi player in VRCPlayerApi.AllPlayers)
                        message = message.Replace(player.displayName, $"<color=#ccf>{player.displayName}</color>");

                Lines.Enqueue(message);
                Log.text = string.Join("\n", Lines);
                DelayedRemove(duration).Start();
            }

            private IEnumerator DelayedRemove(float duration)
            {
                // i would cache this but it appears that causes 
                // it to only remove 1 message per period
                yield return new WaitForSeconds(duration);
                Lines.Dequeue();
                Log.text = string.Join("\n", Lines);
            }
        }
    }
}
