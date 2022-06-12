﻿using BepInEx.Configuration;
using System;
using System.Globalization;
using UnityEngine;

namespace KiraiMod.Core.Serializers
{
    // this likely doesn't match specs
    public static class Color32
    {
        static Color32() =>
            TomlTypeConverter.AddConverter(typeof(UnityEngine.Color32), new TypeConverter()
            {
                ConvertToObject = (str, _) => FromString(str),
                ConvertToString = (obj, _) => FromColor32((UnityEngine.Color32)obj),
            });

        public static UnityEngine.Color32 FromString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentNullException(nameof(str));

            if (str.StartsWith("#"))
                str = str[1..];

            byte r, g, b, a;
            if (str.Length == 3 || str.Length == 4)
            {
                r = (byte)(byte.Parse(str[0].ToString(), NumberStyles.HexNumber) * 17);
                g = (byte)(byte.Parse(str[1].ToString(), NumberStyles.HexNumber) * 17);
                b = (byte)(byte.Parse(str[2].ToString(), NumberStyles.HexNumber) * 17);
                a = str.Length == 4 ? (byte)(byte.Parse(str[4].ToString(), NumberStyles.HexNumber) * 17) : (byte)255;
            }
            else if (str.Length == 6 || str.Length == 8)
            {
                r = byte.Parse(str[0..2], NumberStyles.HexNumber);
                g = byte.Parse(str[2..4], NumberStyles.HexNumber);
                b = byte.Parse(str[4..6], NumberStyles.HexNumber);
                a = str.Length == 4 ? (byte)(byte.Parse(str[6..8].ToString(), NumberStyles.HexNumber) * 17) : (byte)255;
            }
            else throw new ArgumentException(nameof(str));

            return new UnityEngine.Color32(r, g, b, a);
        }

        public static string FromColor32(UnityEngine.Color32 color) => $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";
    }
}
