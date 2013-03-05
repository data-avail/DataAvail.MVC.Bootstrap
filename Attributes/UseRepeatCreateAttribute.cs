using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class UseRepeatCreateAttribute : Attribute, IMetadataAware
    {
        public UseRepeatCreateAttribute()
        { }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("UseRepeatCreate", true);
        }
    }
}