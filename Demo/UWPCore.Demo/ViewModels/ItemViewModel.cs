﻿using UWPCore.Demo.Models;
using UWPCore.Framework.Mvvm;

namespace UWPCore.Demo.ViewModels
{
    public class ItemViewModel : ViewModelBase
    {
        public ItemModel Model { get; internal set; }

        /// <summary>
        /// Constructor for design-time data.
        /// </summary>
        public ItemViewModel() { }

        public ItemViewModel(ItemModel itemModel)
        {
            Model = new ItemModel(itemModel);
        }
    }
}