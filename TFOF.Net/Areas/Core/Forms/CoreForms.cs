//<summary>
//Author: Patrick Dizon
//Date: 7/12/2016
//This model is used in conjunction with BasicController (basic.js) and Views/Shared/DisplayTemplate/FormSearch.cshtml.
//</summary>

namespace TFOF.Areas.Core.Forms
{
    using Attributes;
    using Models;
    using Newtonsoft.Json;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using User.Models;

    public class AngularProperties
    {
        private string ApiName { get; set; }
        private string Controller { get; set; }
        public string ModelName { get; set; }
        public string EndPoint { get; set; }


        public AngularProperties(string apiName, string controller, string modelName = "basic")
        {
            ApiName = apiName;
            Controller = controller;
            ModelName = modelName;
        }

      
        public void SetApiUrl(UrlHelper url)
        {
            if (EndPoint == null && ApiName != null && Controller != null)
            {
                EndPoint = url.RouteUrl(ApiName, new { httproute = "", controller = Controller });
            }
        }

    }

    public class Form
    {
        public FormMethod Method { get; set; } = FormMethod.Post;

        public string FormTitle { get; set; } //For some reason the name Title will override the values in any model with a Title property
        public List<Field> Fields { get; set; } = new List<Field>();

        /// <summary>
        /// Additional Attributes in the <form> tag
        /// </summary>
        public Dictionary<string, object> FormAttributes { get; set; } = new Dictionary<string, object>() { { "name", "EditForm" }, { "id", "EditForm" } };
  
        public string SaveButtonText { get; set; } = "Save";

        public string SaveButtonIcon { get; set; }

        /// <summary>
        /// NewLink provides a link to create new records.
        /// </summary>
        public string NewLink { get; set; }
        /// <summary>
        /// DeleteLink and DeleteRole should be used in combination 
        /// to prevent accidental deletion by unauthorized users.
        /// </summary>
        //public string DeleteLink { get; set; }

        public BaseDeleteModel Delete { get; set; } 

        /// <summary>
        /// Post API Url is the API endpoint for creat execution
        /// </summary>
        public string PostAPIUrl { get; set; }

        public List<string> DeleteRole { get; set; } 

        public UserModel ModifiedBy { get; set; }
        public DateTime Modified { get; set; }
        public UserModel CreatedBy { get; set; }
        public DateTime Created { get; set; }

        public string ForeignKeys { get; set; } = "";

        public AngularProperties AngularProperties { get; set; }
        

        /// <summary>
        /// Initializes the form fields and, date and user stamps.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public virtual Form Init(object model, IPrincipal user = null)
        {
            if (string.IsNullOrWhiteSpace(FormTitle))
            {
                FormTitle = TextService.Title(model.GetType().Name.Replace("Model", ""));
            }

            SetStamps(model);
            if (Fields != null)
            {
                foreach (var field in Fields)
                {

                    if (!string.IsNullOrWhiteSpace(field.Name) && string.IsNullOrWhiteSpace(field.Label))
                    {
                        field.Label = TextService.Title(field.Name);
                    }
                    if (model != null && field is InputField)
                    {
                        if(string.IsNullOrWhiteSpace(field.Name))
                        {
                            throw new Exception("Name is null, empty or whitespace.");
                        }
                        //Model property
                        var mp = model.GetType().GetProperty(field.Name);

                        //Check whether the instance has value then proceed with the following:
                        //Assign Value to field
                        //Setup Options
                        //Setup Selectize Parameters
                        if (mp != null)
                        {
                            if (mp.GetValue(model, null) != null)
                            {
                                //Set to correct DateTime format.
                                if (mp.GetValue(model) is DateTime && (field is DateTimeField || (field is ReadOnlyField && ((ReadOnlyField)field).InputType.Equals("Date"))))
                                {
                                    field.Value = ((DateTime)mp.GetValue(model)).ToString("yyyy-MM-dd");
                                }
                                else if(field is CharField && ((CharField)field).IsMultipleSelect)
                                {
                                    field.Value = string.Join(",", (string[])mp.GetValue(model));
                                }
                                else //Set to just a string
                                {
                                    field.Value = mp.GetValue(model).ToString();
                                }
                            }
                            RenderOptions(model, field);


                            //Check for max-length of field 
                            if (field is CharField)
                            {
                                Attribute stringLengthAttribute = mp.GetCustomAttribute(typeof(StringLengthAttribute));
                                if (stringLengthAttribute != null)
                                {
                                    ((CharField)field).MaxLength = ((StringLengthAttribute)stringLengthAttribute).MaximumLength;
                                }

                                //Set field Mask property if specified by the model and not overridden by form constructor
                                Attribute inputmaskAttribute = mp.GetCustomAttribute(typeof(InputmaskAttribute));
                                if (inputmaskAttribute != null && string.IsNullOrWhiteSpace(((CharField)field).Mask))
                                {
                                    ((CharField)field).Mask = ((InputmaskAttribute)inputmaskAttribute).Mask;
                                }

                                //Add input mask to attributes
                                if(!string.IsNullOrWhiteSpace(((CharField)field).Mask))
                                {
                                    field.Attributes.Add("data-inputmask", ((CharField)field).Mask);
                                }
                            }
                            //Check for required input
                            if (field is InputField)
                            {
                                Attribute attr = mp.GetCustomAttribute(typeof(RequiredAttribute));
                                if (attr != null)
                                {
                                    ((InputField)field).Required = true;
                                }
                            }

                            //Check for Readonly properties
                            if (field is InputField)
                            {
                                Attribute attr = mp.GetCustomAttribute(typeof(ReadOnlyAttribute));
                                if (attr != null)
                                {
                                    ((InputField)field).IsReadOnly = true;
                                }
                            }
                        } else
                        {
                            //Cannot be null. Otherwise razor will assigned any similarly name variable;
                            //The variable Title in the form for example will populate any 'Title' field with its value.
                            field.Value = "";
                        }

                        
                        //Check for selectize widgets
                        if (field is CharField && ((CharField)field).Widget != null && ((CharField)field).Widget is SelectizeWidget)
                        {
                            ((SelectizeWidget)((CharField)field).Widget).Set(model, field);
                        }
                    }

                    if (AngularProperties != null)
                    {
                        field.Attributes.Add("ng-model", AngularProperties.ModelName + "." + field.Name);
                    }
                }

                
            }
            return this;
        }
        
        public Field GetField(string name) {
            foreach(Field field in Fields.Where( f => f.Name != null))
            {
                if (field.Name.Equals(name))
                {
                    return field;
                }
            }
            return null;
        }

        public void RemoveField(string name)
        {
            Fields.Remove(Fields.Single(f => f.Name == name));
        }

        public void RenderOptions(object model, Field field)
        {
            
            //Set the options. Make sure we include current value in the options.
            if (field.Options != null)
            {
                List<string> ToAdd = new List<string>();
                // bool found = false;
                List<SelectListItem> selectListItems = new List<SelectListItem>();

                //Always add a null entry so that the user is forced to select the correct option
                //instead of the first option. When a field is required validation will prompt them.
                selectListItems.Add(new SelectListItem { Text = null, Value = null });
                
                //A SelectList can contain different kinds of lists
                foreach (var item in field.Options.Items)
                {
                    string value;
                    string text;

                    //Include the value that is currently in the model property
                    //In order to not lose any data. Place it in the obsolete group
                    try
                    {
                        value = item.GetType().GetProperty(field.Options.DataValueField).GetValue(item).ToString();
                        text = item.GetType().GetProperty(field.Options.DataTextField).GetValue(item).ToString();
                        selectListItems.Add(new SelectListItem() { Value = value, Text = text });
                    }
                    catch
                    {

                    }
                }
                //Check whether the values are in the options.
                if (field.Value != null && selectListItems != null) {
                    //Use split if multiple
                    if (field is CharField && ((CharField)field).IsMultipleSelect) {
                        foreach (string fieldValue in field.Value.Split(','))
                        {
                            if(selectListItems.Where(v => v.Value != null && v.Value.Equals(fieldValue)).Count() == 0)
                            {
                                selectListItems.Add(new SelectListItem() { Value = fieldValue, Text = fieldValue, Group = new SelectListGroup() { Name = "Current" } });
                            }
                        }
                    } else {
                        if (selectListItems.Where(v => v.Value != null && v.Value.Equals(field.Value)).Count() == 0)
                        {
                            selectListItems.Add(new SelectListItem() { Value = field.Value, Text = field.Value, Group = new SelectListGroup() { Name = "Current" } });
                        }
                    }
                }
                field.Options = new SelectList(selectListItems, "Value", "Text", "Group.Name", field.Value, null);
            }
        }
        public void SetStamps<T>(T instance) {
            try
            {
                ModifiedBy = (UserModel)instance.GetType().GetProperty("ModifiedBy").GetValue(instance, null);
            } catch
            {

            }

            try
            {
                Modified = (DateTime)instance.GetType().GetProperty("Modified").GetValue(instance, null);

            } catch
            {

            }

            try
            {
                CreatedBy = (UserModel)instance.GetType().GetProperty("CreatedBy").GetValue(instance, null);

            }
            catch
            {

            }
            try
            {
                Created = (DateTime)instance.GetType().GetProperty("Created").GetValue(instance, null);

            }
            catch
            {

            }
        }

        /// <summary>
        /// Generates an HttpRouteUrl based on api Name and controller name
        /// </summary>
        /// <param name="apiName"></param>
        /// <param name="controller"></param>
        /// <returns>string</returns>
        public string GetUrl(string apiName, string controller)
        {
            return Url.HttpRouteUrl(apiName, new { httproute = "", controller = controller });
        }

        public UrlHelper Url
        {
            get
            {
                return new UrlHelper(HttpContext.Current.Request.RequestContext);
            }
        }
    }
    
    public class Field
    {
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string HelpText { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public SelectList Options { get; set; }
        public bool OptionsAsSuggestions { get; set; } = false;
        public string ClassNames { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Disables the form field. Inserts a hidden field where it stores the current value to be passed to the form. ReadOnly model properties utilize this or ReadOnlyFields, but not vice-versa.
        /// </summary>
        public bool IsReadOnly { get; set; }
    }
    /// <summary>
    /// Blank Cell provides a blank area in a Bootstrap 12 cell grid
    /// </summary>
    public class BlankCell : Field
    {
     
    }
    /// <summary>
    /// Distinguishes which are Input fields vs Plain Fields.
    /// </summary>
    public class InputField : Field {
        public bool Required { get; set; } = false;
        public int TabIndex { get; set; }
    }
    
    /// <summary>
    /// Field used for true or false values. Renders a checkbox.
    /// </summary>
    public class BooleanField : InputField
    {
        public new bool Value { get; set; }
        public bool IsChecked { get; set; }
    }
    /// <summary>
    /// A field used for strings
    /// </summary>
    public class CharField : InputField
    {
        public BaseWidget Widget { get; set; }
        public int MaxLength { get; set; }
        public bool IsMultipleSelect { get; set;}
        public string Mask { get; set; }

        public static SelectList yesNoOptions = new SelectList(new List<SelectListItem>()
        {
            new SelectListItem() { Value = "Y", Text = "Yes" },
            new SelectListItem() { Value = "N", Text = "No" }

        }, "Value", "Text");
    }
    /// <summary>
    /// Field used in textareas
    /// </summary>
    public class CharMaxField : InputField
    {
        public int Rows { get; set; } = 2;
    }
    /// <summary>
    /// Field used for date format
    /// </summary>
    public class DateTimeField : InputField { }

    /// <summary>
    /// Field used for money or any numerical value with a decimal
    /// </summary>
    public class DoubleField : InputField {
        
        public DoubleField(int? decimals = 2, int? min = 0, int? max = null)
        {
            Decimals = (decimals == null ? 2 : (int)decimals);
            if(Attributes == null)
            {
                Attributes = new Dictionary<string, String>();
            }
            Attributes.Add("step", "0." + 1.ToString($"D{Decimals}"));
            Attributes.Add("min", string.Format("{0}.",min) + 0.ToString($"D{Decimals}"));
            if(max != null)
            {
                Attributes.Add("max", string.Format("{0}.", max) + 0.ToString($"D{Decimals}"));
            }
        }
        int Decimals { get; set; }
    }

    /// <summary>
    /// A group label separator.
    /// </summary>
    public class GroupLabel : Field { }

    /// <summary>
    /// Field used for hidden data
    /// </summary>
    public class HiddenField : InputField { }

    /// <summary>
    /// Field used for Ids, similar to HiddenField
    /// </summary>
    public class IdField : InputField { }

    /// <summary>
    /// Field used for Integers or whole numbers. Contains a numeric scroller for some browsers.
    /// </summary>
    public class IntegerField : InputField { }


    /// <summary>
    /// Field used for passwords.
    /// </summary>
    public class PasswordField : InputField { }
        
    /// <summary>
    /// Field used for non-modifiable data. Appears on the form but is not editable.
    /// </summary>
    public class ReadOnlyField : InputField {
        public string InputType { get; set; } = "text";
        public BaseWidget Widget { get; set; }
    }
    
    public class Link : Field
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public string LinkClass { get; set; }
        /// <summary>
        /// Fontawesome icon before text
        /// </summary>
        public string IconBefore { get; set; }
        /// <summary>
        /// Fontawesome icon after text
        /// </summary>
        public string IconAfter { get; set; }
    }

    public class BaseWidget { }

    /// <summary>
    /// Converts a Field to a selectized type of input.
    /// </summary>
    public class SelectizeWidget : BaseWidget
    {
        public string ApiUrl { get; set; }
        public string Filters { get; set; }
        public string Label { get; set; }
        public string ValueField { get; set; }
        public string LabelFields { get; set; }
        public string OrderBy { get; set; }
        public Int16 Top { get; set; }
        public string Data { get; set; }
        public string SearchType { get; set; }
        /// <summary>
        /// A colon separated parameter:value pair
        /// </summary>
        public string RestrictBy { get; set; }

        public bool Create { get; set; } = false;

        public void Set(object instance, Field field)
        {
            //Get the model property of the named field
            //A
            var mp = instance.GetType().GetProperty(field.Name);
            if(mp == null)
            {
                throw new Exception($"{field.Name} property is not in model {instance.GetType().Name}.");
            }
            List<string> label = new List<string>();
            Attribute attr = mp.GetCustomAttribute(typeof(ForeignKeyAttribute));
            //SelectizeWidget widget = (SelectizeWidget)((CharField)field).Widget;

            if (attr != null)
            {
                PropertyInfo fkp = instance.GetType().GetProperty(((ForeignKeyAttribute)attr).Name);

                //Loop through comma separated labels.
                foreach (string wlabel in Label.Split(','))
                {
                    if (fkp.GetValue(instance, null) != null && fkp.GetValue(instance).GetType().GetProperty(wlabel) != null)
                    {
                        var lp = fkp.GetValue(instance).GetType().GetProperty(wlabel).GetValue(fkp.GetValue(instance), null);
                        if (lp != null)
                        {
                            label.Add(lp.ToString());
                        }
                    }
                    else
                    {
                        //Get the Foreign Model type
                        if (!string.IsNullOrWhiteSpace(field.Value))
                        {
                            Type t = typeof(BaseModelContext<>).MakeGenericType(fkp.PropertyType);
                            dynamic baseModelContext = Activator.CreateInstance(t);
                            try
                            {
                                object model;
                                //Try using int for id
                                try
                                {
                                    model = baseModelContext.Models.Find(int.Parse(field.Value));
                                }
                                //Else try string.
                                catch
                                {
                                    model = baseModelContext.Models.Find(field.Value);
                                }
                                if (model != null)
                                {
                                    var nfkp = model.GetType().GetProperty(wlabel);
                                    if (nfkp != null)
                                    {
                                        label.Add(nfkp.GetValue(model).ToString());
                                    }
                                }
                            }
                            catch
                            {

                                //do nothing
                            }
                        }

                    }
                }
                //Set the data
                Dictionary<string, object> data = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(Data))
                {
                    foreach (string d in Data.Split(','))
                    {

                        if (fkp.GetValue(instance, null) != null && fkp.GetValue(instance).GetType().GetProperty(d) != null)
                        {
                            var dp = fkp.GetValue(instance).GetType().GetProperty(d).GetValue(fkp.GetValue(instance), null);
                            data.Add(d, dp);
                        }
                    }
                }
                Data = JsonConvert.SerializeObject(data);
                
            }

            Label = string.Join(" ", label);

        }
    }


}