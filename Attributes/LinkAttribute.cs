using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LinkAttribute : Attribute, IMetadataAware
    {
        public LinkAttribute(string Resource, string ReferenceFieldName)
        {
            this.Resource = Resource;

            this.ReferenceFieldName = ReferenceFieldName;
        }

        public string Resource { get; set; }

        public string ReferenceFieldName { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues.Add("LinkResource", Resource);
            metadata.AdditionalValues.Add("LinkReferenceFieldName", ReferenceFieldName);
        }
    }
}