using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CaptionAttribute : Attribute, IMetadataAware
    {
        public CaptionAttribute(string CreateCaption, string EditCaption)
        {
            this.CreateCaption = CreateCaption;
            this.EditCaption = EditCaption;
        }

        public string CreateCaption { get; set; }

        public string EditCaption { get; set; }


        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("CreateCaption", this.CreateCaption);
            metadata.AdditionalValues.Add("EditCaption", this.EditCaption);
        }
    }
}