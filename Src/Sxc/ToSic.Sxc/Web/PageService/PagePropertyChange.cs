﻿namespace ToSic.Sxc.Web.PageService
{
    public class PagePropertyChange
    {
        public PagePropertyChange()  { }

        /// <summary>
        /// Clone-Constructor
        /// </summary>
        /// <param name="original"></param>
        public PagePropertyChange(PagePropertyChange original)
        {
            ChangeMode = original.ChangeMode;
            Property = original.Property;
            Value = original.Value;
            ReplacementIdentifier = original.ReplacementIdentifier;
        }

        public PageChangeModes ChangeMode { get; set; }
        
        internal PageProperties Property { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// This is part of the original property, which would be replaced.
        /// </summary>
        public string ReplacementIdentifier { get; set; }


    }
}
