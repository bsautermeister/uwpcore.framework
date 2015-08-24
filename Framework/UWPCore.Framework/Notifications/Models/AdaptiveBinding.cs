﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UWPCore.Framework.Common;

namespace UWPCore.Framework.Notifications.Models
{
    public enum VisualHintTextStacking
    {
        Top,
        Center,
        Bottom
    }

    public enum VisualHintPresentation
    {
        Photos,
        People,
        Contact
    }

    /// <summary>
    /// Tile template name v3.
    /// </summary>
    public enum VisualTemplate
    {
        TileSmall,
        TileMedium,
        TileWide,
        TileLarge, // Desktop only!
        ToastGeneric // Toast only template!
    }

    public class AdaptiveBinding : AdaptiveVisualBindingBase, IAdaptive
    {
        /// <summary>
        /// Gets or sets the tile template name v3.
        /// </summary>
        public VisualTemplate Template { get; set; }
        
        /// <summary>
        /// Gets or sets the fallback tile template name v1.
        /// </summary>
        public string Fallback { get; set; }

        public VisualHintTextStacking? HintTextStacking { get; set; }

        public VisualHintPresentation? HintPresentation { get; set; }

        private int? _hintOverlay;
        /// <summary>
        /// none|logo|name|nameAndLogo
        /// </summary>
        public int? HintOverlay
        {
            get
            {
                return _hintOverlay;
            }

            set
            {
                if (value >= 0 && value <= 100)
                {
                    _hintOverlay = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("HintOverlay", "Must in range of [0-100]");
                }
            }
        }

        public string HintLockDetailedStatus1 { get; set; }
        public string HintLockDetailedStatus2 { get; set; }
        public string HintLockDetailedStatus3 { get; set; }

        public IList<IAdaptiveVisualChild> Children { get; set; } = new List<IAdaptiveVisualChild>();

        public XElement GetXElement()
        {
            var element = new XElement("binding", GetXAttributes());
            element.Add(new XAttribute("template", Template.ToString()));
            if (HintTextStacking.HasValue)
            {
                element.Add(new XAttribute("hint-textStacking", HintTextStacking.Value.ToString().FirstLetterToLower()));
            }

            if (HintOverlay.HasValue)
            {
                element.Add(new XAttribute("hint-overlay", HintOverlay.Value));
            }

            if (HintPresentation.HasValue)
            {
                element.Add(new XAttribute("hint-presentation", HintPresentation.Value.ToString().FirstLetterToLower()));
            }

            if (!string.IsNullOrWhiteSpace(HintLockDetailedStatus1))
            {
                element.Add(new XAttribute("hint-lockDetailedStatus1", HintLockDetailedStatus1));
            }

            if (!string.IsNullOrWhiteSpace(HintLockDetailedStatus2))
            {
                element.Add(new XAttribute("hint-lockDetailedStatus2", HintLockDetailedStatus2));
            }

            if (!string.IsNullOrWhiteSpace(HintLockDetailedStatus3))
            {
                element.Add(new XAttribute("hint-lockDetailedStatus3", HintLockDetailedStatus3));
            }

            if (!string.IsNullOrWhiteSpace(Fallback))
            {
                element.Add(new XAttribute("fallback", Fallback));
            }

            foreach (var child in Children)
            {
                element.Add(child.GetXElement());
            }

            return element;
        }
    }
}