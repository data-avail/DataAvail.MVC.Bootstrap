using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Attributes
{
    [Flags]
    public enum RenderMode
    { 
        None = 0,
        Edit = 0x1,
        Create = 0x10,
        Table = 0x100,
        Form = Edit | Create,
        All = Edit | Create | Table
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RenderModeAttribute : Attribute, IMetadataAware
    {
        public RenderModeAttribute(RenderMode Mode, RenderMode ReadOnly = RenderMode.None)
        {
            this.Mode = Mode;
            this.ReadOnly = ReadOnly;
        }

        public RenderMode Mode { get; set; }

        public RenderMode ReadOnly { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("RenderMode", Mode);
            metadata.AdditionalValues.Add("RenderReadOnly", ReadOnly);
        }
    }
}