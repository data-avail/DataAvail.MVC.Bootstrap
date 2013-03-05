using DataAvail.MVC.Bootstrap;
using DataAvail.MVC.Bootstrap.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DataAvail.MVC.Bootstrap.Helpers
{
    public enum FormType { 
        Create,
        Edit
    }

    public static class FormHelpers
    {
        public static MvcHtmlString BtpEditFormEmbed<TModel>(this HtmlHelper<TModel> html)
        {
            var modelType = html.GetModelType<TModel>();
            var formTemplate = BootstrapHelperResources.EditFrormEmbed;
            var fields = html.BtpFormFields(FormType.Edit);
            var form = string.Format(formTemplate, fields);

            return new MvcHtmlString(form);
        }

        public static MvcHtmlString BtpForm<TModel>(this HtmlHelper<TModel> html, FormType FormType, string Controller = null)
        {
            var modelType = html.GetModelType();
            var formTemplate = FormType == FormType.Edit ? BootstrapHelperResources.EditFrorm : BootstrapHelperResources.CreateFrorm;
            var fields = html.BtpFormFields(FormType).ToString();

            var metadata = ModelMetadataProviders.Current.GetMetadataForType(null, modelType);

            object useRepeatCreate;


            if (FormType == FormType.Create && metadata.AdditionalValues.TryGetValue("UseRepeatCreate", out useRepeatCreate))
            { 
                fields +=  "<div class=\"control-group\"><div class=\"controls\"><label class=\"checkbox\"><input type=\"checkbox\" data-bind=\"checked : useRepeatCreate\"> Создать другую </label></div></div>";
            }

            var form = string.Format(formTemplate, Controller ?? (string)html.ViewContext.RouteData.GetRequiredString("controller"), fields, metadata.DisplayName);

            return new MvcHtmlString(form);
        }


        public static MvcHtmlString BtpFormFields<TModel>(this HtmlHelper<TModel> html, FormType FormType)
        {
            var modelType = html.GetModelType<TModel>();

            var columns = modelType.GetProperties().Select(p =>
            {                
                ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, modelType, p.Name);
                var show = metadata.ShowForEdit;
                object renderMode;

                if (metadata.AdditionalValues.TryGetValue("RenderMode", out renderMode))
                {
                    show = (FormType == FormType.Create && (((RenderMode)renderMode) & RenderMode.Create) != RenderMode.None) ||
                        (FormType == FormType.Edit && (((RenderMode)renderMode) & RenderMode.Edit) != RenderMode.None);
                }

                //((System.Web.Mvc.DataAnnotationsModelValidator)col.meta.GetValidators(html.ViewContext).ElementAt(2)).Attribute
                var rules = metadata.GetValidators(html.ViewContext).OfType<System.Web.Mvc.DataAnnotationsModelValidator>().SelectMany(s => s.GetClientValidationRules());

                return show ? new { property = p.Name, name = metadata.GetDispayFieldName(p.Name), order = metadata.Order, hidden = metadata.AdditionalValues.ContainsKey("Hidden"), meta = metadata, rules = rules} : null;
            })
            .Where(p => p != null).OrderBy(p => p.order);


            TagBuilder tag = new TagBuilder("div");
            foreach (var col in columns.Where(p => !p.hidden))
            {
                TagBuilder rowTag = new TagBuilder("div");
                rowTag.AddCssClass("control-group");
                rowTag.InnerHtml = GetLabel(col.meta, col.property, FormType);

                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("controls");                
                if (GetValidation(col.meta, col.rules) != null)
                {
                    rowTag.MergeAttribute("data-bind", string.Format("{{validationCss : {0}}}", col.property));
                    var msgTag = new TagBuilder("span");
                    msgTag.MergeAttribute("data-bind", string.Format("validationMessage: {0}", col.property));
                    msgTag.AddCssClass("help-inline");
                    div.InnerHtml += msgTag.ToString(TagRenderMode.Normal);
                }
                div.InnerHtml += GetEditor(col.meta, col.property, FormType, col.rules);
                rowTag.InnerHtml += div;

                tag.InnerHtml += rowTag.ToString(TagRenderMode.Normal);                
            }
            foreach (var col in columns.Where(p => p.hidden))
            {
                tag.InnerHtml += GetEditor(col.meta, col.property, FormType);
            }

            return new MvcHtmlString(tag.ToString(TagRenderMode.Normal));
        }

        public static string GetLabel(ModelMetadata metadata, string htmlFieldName, FormType FormType)
        {
            string labelText = metadata.GetDispayFieldName(htmlFieldName);
            if (String.IsNullOrEmpty(labelText))
            {
                return null;
            }
            TagBuilder tag = new TagBuilder("label");
            tag.AddCssClass("control-label");
            //tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", metadata.GetDispayFieldId(htmlFieldName, FormType));

            TagBuilder span = new TagBuilder("span");
            span.SetInnerText(labelText);

            // assign <span> to <label> inner html
            tag.InnerHtml = span.ToString(TagRenderMode.Normal);

            return tag.ToString(TagRenderMode.Normal);
        }

        public static string GetEditor(ModelMetadata metadata, string htmlFieldName, FormType FormType, IEnumerable<ModelClientValidationRule> Rules = null)
        {

            TagBuilder tag = new TagBuilder("input");
            //tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("id", metadata.GetDispayFieldId(htmlFieldName, FormType));
            tag.Attributes.Add("name", htmlFieldName);

            string dataBindAttr = null;
            object autoCompleteUrl;
            if (metadata.AdditionalValues.TryGetValue("AutoCompleteUrl", out autoCompleteUrl))
            {
                var opts = string.Format("url : '{0}', fields : {{ {1} }}", autoCompleteUrl, string.Join(",", metadata.AdditionalValues.Where(p => p.Key.StartsWith("AutoCompleteRelatedField"))
                    .Select(p => string.Format("{0} : '{1}'", p.Value, ((string)p.Key).Split('$')[1]))));

                object autoCompleteReset;
                object autoCompleteNotInList;
                if (metadata.AdditionalValues.TryGetValue("AutoCompleteResetRelatedFieldsOnNull", out autoCompleteReset))
                {
                    opts += string.Format(", resetRelatedFieldsOnNull : {0}", ((bool)autoCompleteReset).ToString().ToLower()); 
                }
                if (metadata.AdditionalValues.TryGetValue("AutoCompleteAllowNotInList", out autoCompleteNotInList))
                {
                    opts += string.Format(", allowNotInList : {0}", ((bool)autoCompleteNotInList).ToString().ToLower());
                }

                dataBindAttr = string.Format("autocomplete : {0}, autocompleteOpts : {{ {1} }}", htmlFieldName, opts);
            }
            else
            {
                if (metadata.ModelType == typeof(DateTime) || metadata.ModelType == typeof(DateTime?))
                    dataBindAttr = string.Format("datetime : {0}", htmlFieldName);
                else
                    dataBindAttr = string.Format("value : {0}", htmlFieldName);
            }

            if (dataBindAttr != null)
            {
                var validation = metadata.AdditionalValues.ContainsKey("Hidden") ? null : GetValidation(metadata, Rules);

                if (validation != null)
                    dataBindAttr += string.Format(", validation : {0}", validation);

                tag.MergeAttribute("data-bind", dataBindAttr);
            }

           
            if (!metadata.AdditionalValues.ContainsKey("Hidden"))
            {               
                tag.MergeAttribute("type", "Text");

                var readOnly = metadata.IsReadOnly;

                object renderReadOnly;

                if (metadata.AdditionalValues.TryGetValue("RenderReadOnly", out renderReadOnly))
                {
                    readOnly = (FormType == FormType.Create && (((RenderMode)renderReadOnly) & RenderMode.Create) != RenderMode.None) ||
                        (FormType == FormType.Edit && (((RenderMode)renderReadOnly) & RenderMode.Edit) != RenderMode.None);
                }

                if (readOnly)
                {
                    tag.MergeAttribute("readonly", "readonly");
                }

                if (!string.IsNullOrEmpty(metadata.Watermark))
                {
                    tag.MergeAttribute("placeholder", metadata.Watermark);
                }
                /*
                TagBuilder div = new TagBuilder("div");
                div.AddCssClass("controls");
                div.InnerHtml = tag.ToString(TagRenderMode.SelfClosing);
                tag = div;
                 */
            }
            else
            {
                tag.Attributes.Add("type", "Hidden");
            }


            return tag.ToString(TagRenderMode.Normal);
        }

        public static string GetValidation(ModelMetadata metadata, IEnumerable<ModelClientValidationRule> Rules)
        {
            var attrs = new List<string>();
            if (metadata.IsRequired)
            {
                attrs.Add("{required : true}");
            }
            if (metadata.ModelType == typeof(int) || metadata.ModelType == typeof(int?))
            {
                attrs.Add("{digit : true}");
            }
            if (metadata.ModelType == typeof(double) || metadata.ModelType == typeof(double?))
            {
                attrs.Add("{number : true}");
            }

            if (Rules != null)
            {
                foreach (var rule in Rules)
                {
                    if (rule is ModelClientValidationStringLengthRule)
                    {
                        if (rule.ValidationParameters.ContainsKey("min"))
                        {
                            attrs.Add(string.Format("{{minLength : {0}}}", rule.ValidationParameters["min"]));
                        }

                        if (rule.ValidationParameters.ContainsKey("max"))
                        {
                            attrs.Add(string.Format("{{maxLength : {0}}}", rule.ValidationParameters["max"]));
                        }
                    }
                }
            }

            return attrs.Count() != 0 ? string.Format("[{0}]", string.Join(",", attrs)) : null;
        }
    }
}