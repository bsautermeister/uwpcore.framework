﻿using System;
using UWPCore.Framework.Common;
using UWPCore.Framework.IoC;
using UWPCore.Framework.Mvvm;
using UWPCore.Framework.Navigation;
using UWPCore.Framework.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWPCore.Framework.Controls
{
    /// <summary>
    /// The base class for every page. Provides some short access functions to reduce boilerplate code.
    /// </summary>
    /// <remarks>
    /// The base class MUST be used when using MVVM, to ensure the navigation events of <see cref="Mvvm.ViewModelBase"/> gets called.
    /// </remarks>
    public class UniversalPage : Page
    {
        /// <summary>
        /// The pages theme color.
        /// </summary>
        public static StoredObjectBase<string> PageTheme = new LocalObject<string>("__pageTheme__", ElementTheme.Default.ToString());

        /// <summary>
        /// Gets the parent page of the navigation tree. This attribute is optional and only
        /// used in case the app uses sub-pages in the AppShell.
        /// </summary>
        public Type ParentPage { get; private set; }

        public UniversalPage() { }

        public UniversalPage(Type parentPage)
        {
            ParentPage = parentPage;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // call navigation service events from here to ensure ViewModel navigation methods are called in right order
            NavigationService.NavigateTo(e.NavigationMode, e.Parameter);
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            // call navigation service events from here to ensure ViewModel navigation methods are called in right order
            e.Cancel = !NavigationService.NavigatingFrom(e, false);

            if (!e.Cancel)
            {
                await NavigationService.NavigateFromAsync(false);
            }
        }

        /// <summary>
        /// Updates the theme.
        /// </summary>
        public void UpdateTheme()
        {
            RequestedTheme = (ElementTheme)Enum.Parse(typeof(ElementTheme), PageTheme.Value);
        }

        /// <summary>
        /// Gets the navigation serivce.
        /// </summary>
        public NavigationService NavigationService
        {
            get
            {
                return UniversalApp.Current.NavigationService;
            }
        }

        /// <summary>
        /// Gets the injector.
        /// </summary>
        public IInjector Injector
        {
            get
            {
                return IoC.Injector.Instance;
            }
        }

        /// <summary>
        /// Gets called when the page gets resumed.
        /// </summary>
        public virtual void OnResume() {
            if (DataContext != null)
            {
                var viewModel = DataContext as ViewModelBase;

                if (viewModel != null)
                    viewModel.OnResume();
            }
        }
    }
}
