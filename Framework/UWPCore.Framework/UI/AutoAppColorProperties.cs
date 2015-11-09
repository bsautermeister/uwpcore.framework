﻿using UWPCore.Framework.Common;
using Windows.UI;
using Windows.UI.Xaml;

namespace UWPCore.Framework.UI
{
    /// <summary>
    /// The auto configured app colors.
    /// </summary>
    public class AutoAppColorProperties : IAppColorProperties
    {
        public Color? Theme { get; private set; }

        public Color? TitleBarForeground { get; private set; }

        public Color? TitleBarBackground { get; private set; }

        public bool IsAutoConfigured
        {
            get { return true; }
        }

        /// <summary>
        /// Creates auto-generated app colors depending on the dark/light theme.
        /// </summary>
        public AutoAppColorProperties()
        {
            Theme = (Color)UniversalApp.Current.Resources["SystemAccentColor"];
            if (UniversalApp.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                TitleBarForeground = Colors.White;
                TitleBarBackground = Colors.Black;
            }
            else
            {
                TitleBarForeground = Colors.Black;
                TitleBarBackground = Colors.White;
            }
        }
    }
}
