﻿using System;
using System.Collections.Generic;
using System.Linq;
using UWPCore.Framework.Navigation;
using Windows.UI.Xaml;

namespace UWPCore.Framework.Common
{
    /// <summary>
    /// The window wrapper class that wrapps the active windows and links to others.
    /// </summary>
    public class WindowWrapper
    {
        /// <summary>
        /// All active wrapped windows.
        /// </summary>
        public readonly static List<WindowWrapper> ActiveWrappers = new List<WindowWrapper>();

        /// <summary>
        /// Gets the wrapped window.
        /// </summary>
        public Window Window { get; private set; }

        /// <summary>
        /// Gets the dispatcher wrapper for thread-safity.
        /// </summary>
        public DispatcherWrapper Dispatcher { get; private set; }

        /// <summary>
        /// Gets the list of navigation services.
        /// </summary>
        public List<NavigationService> NavigationServices { get; } = new List<NavigationService>();

        /// <summary>
        /// Creates a WindowsWrapper instance.
        /// </summary>
        /// <param name="window">The window to wrapp.</param>
        public WindowWrapper(Window window)
        {
            if (ActiveWrappers.Any(x => x.Window == window))
                throw new Exception("Windows already has a wrapper; use Current(window) to fetch.");
            Window = window;
            ActiveWrappers.Add(this);
            Dispatcher = new DispatcherWrapper(window.Dispatcher);
            window.Closed += (s, e) => { ActiveWrappers.Remove(this); };
        }

        /// <summary>
        /// Gets the current active wrapped window.
        /// </summary>
        /// <returns>The current active window.</returns>
        public static WindowWrapper Current()
        {
            return ActiveWrappers.First(x => x.Window.Dispatcher.HasThreadAccess);
        }

        /// <summary>
        /// Gets the current active wrapped window.
        /// </summary>
        /// <param name="window">The containing window instance.</param>
        /// <returns></returns>
        public static WindowWrapper Current(Window window)
        {
            return ActiveWrappers.First(x => x.Window == window);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            Window.Close();
        }
    }
}
