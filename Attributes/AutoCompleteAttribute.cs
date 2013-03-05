using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAvail.MVC.Bootstrap.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutoCompleteAttribute : Attribute, IMetadataAware
    {
        public AutoCompleteAttribute()
        {
            ResetRelatedFieldsOnNull = true;
        }

        public string Url { get; set; }

        public bool AllowNotInList { get; set; }

        public bool ResetRelatedFieldsOnNull { get; set; }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.AdditionalValues["AutoCompleteUrl"] = Url;
            metadata.AdditionalValues["AutoCompleteAllowNotInList"] = AllowNotInList;
            metadata.AdditionalValues["AutoCompleteResetRelatedFieldsOnNull"] = ResetRelatedFieldsOnNull;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AutoCompleteRelatedFieldAttribute : Attribute, IMetadataAware
    {
        public AutoCompleteRelatedFieldAttribute(string FieldName, string RemoteFieldName)
        {
            _fieldNameRemotePairs = new Dictionary<string, string>();
            _fieldNameRemotePairs.Add(FieldName, RemoteFieldName);
        }

        public AutoCompleteRelatedFieldAttribute(string[] FieldNames, string[] RemoteFieldNames)
        {
            _fieldNameRemotePairs = new Dictionary<string, string>();
            for (var i = 0; i < FieldNames.Length; i++)
                _fieldNameRemotePairs.Add(FieldNames[i], RemoteFieldNames[i]);            
        }

        private readonly Dictionary<string, string> _fieldNameRemotePairs;

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            foreach(var kvp in _fieldNameRemotePairs)
                metadata.AdditionalValues[string.Format("AutoCompleteRelatedField${0}", kvp.Key)] = kvp.Value;
        }
    }
}