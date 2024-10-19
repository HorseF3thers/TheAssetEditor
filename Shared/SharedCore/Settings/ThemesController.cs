﻿using System.Windows;
using System.Windows.Media;

namespace Shared.Core.Settings
{
    public enum ThemeType
    {
        DarkTheme,
        LightTheme
    }

    public static class ThemesController
    {
        public static ThemeType CurrentTheme { get; set; }

        private static ResourceDictionary ThemeDictionary
        {
            get => Application.Current.Resources.MergedDictionaries[0];
            set => Application.Current.Resources.MergedDictionaries[0] = value;
        }

        private static ResourceDictionary ControlColours
        {
            get => Application.Current.Resources.MergedDictionaries[1];
            set => Application.Current.Resources.MergedDictionaries[1] = value;
        }

        private static void RefreshControls()
        {
            var merged = Application.Current.Resources.MergedDictionaries;
            var dictionary = merged[2];
            merged.RemoveAt(2);
            merged.Insert(2, dictionary);
        }

        public static void SetTheme(ThemeType theme)
        {
            var themeName = theme.ToString();
            if (string.IsNullOrEmpty(themeName))
                return;
            CurrentTheme = theme;
            ThemeDictionary = new ResourceDictionary() { Source = new Uri($"Themes/ColourDictionaries/{themeName}.xaml", UriKind.Relative) };
            ControlColours = new ResourceDictionary() { Source = new Uri("Themes/ControlColours.xaml", UriKind.Relative) };
            RefreshControls();
        }

        public static object GetResource(object key)
        {
            return ThemeDictionary[key];
        }

        public static SolidColorBrush GetBrush(string name)
        {
            return GetResource(name) is SolidColorBrush brush ? brush : new SolidColorBrush(Colors.White);
        }

        public static string GetEnumAsString(ThemeType theme)
        {
            return theme switch
            {
                ThemeType.DarkTheme => "Dark",
                ThemeType.LightTheme => "Light"
            };
        }
    }
}