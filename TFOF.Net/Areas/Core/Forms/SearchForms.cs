/*
    Author: Patrick Dizon
    Date: 7/12/2016
    
    This model is used in conjunction with BasicController (basic.js) and Views/Shared/DisplayTemplate/FormSearch.cshtml.

*/


namespace TFOF.Areas.Core.Forms
{
    using Services;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    public class SearchForm {
        /// <summary>
        /// Name of a SearchForm. Default is "SearchForm"
        /// </summary>
        public string Name { get; set; } = "SearchForm";

        /// <summary>
        /// Enpoint of json api
        /// </summary>
        public string ApiUrl { get; set; }
        /// <summary>
        /// The API name to use. In conjunction with Api, used by controllers to map a route. Builds ApiURL property.
        /// </summary>
        public string ApiName { get; set; }
        /// <summary>
        /// The API controller to use. In conjunction with Api, used by controllers to map a route. Builds ApiURL property.
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// Define Related Objects To Expand to.
        /// </summary>
        public string Expand { get; set; }
        /// <summary>
        /// Top n rows
        /// </summary>
        public int Top { get; set; } = 30;
        /// <summary>
        /// Filters will be stored in browsers localStorage for revisits.
        /// </summary>
        public bool UseLocalStorage { get; set; } = true;
        /// <summary>
        /// Immediately calls and loads results.
        /// </summary>
        public bool Load  { get; set; } = true;
        /// <summary>
        /// For limiting the properties to a subset
        /// </summary>
        public List<string> Selects  { get; set; } = new List<string>();
        /// <summary>
        /// The search fields to render on the top of the list
        /// </summary>
        public List<SearchField> SearchFields { get; set; } = new List<SearchField>();
        /// <summary>
        /// The sort fields available to sort the result with
        /// </summary>
        public List<SortField> SortFields  { get; set; } = new List<SortField>();
        /// <summary>
        /// Set to false to hide the sortfields to prevent editing by user.
        /// </summary>
        public bool IsVisibleSortFields { get; set; } = true;
        /// <summary>
        /// Controls the visibility of the navigation controls.
        /// </summary>
        public bool IsVisibleNavigation { get; set; } = true;
        /// <summary>
        ///Always include the fields mentioned below plus the value to limit the results with.
        /// </summary>
        public List<RestrictSearchField> RestrictSearchFields { get; set; } = new List<RestrictSearchField>();
        /// <summary>
        ///Translations stores dictionaries to translate coded fields.
        ///For example CustomerType of C translates to Corporate and R equals Residential
        ///Translations['CustomerTypes'] = CustomerModel.CustomerTypes'
        /// </summary>
        public Dictionary<string, IEnumerable<SelectListItem>> Translations  { get; set; } = new Dictionary<string, IEnumerable<SelectListItem>>(); 
            
        public SearchForm(UrlHelper url)
        {

        }

        public SearchForm(string apiUrl)
        {
            ApiUrl = apiUrl;
        }

        public SearchForm(string apiName, string controller, UrlHelper url)
        {
            SetApiUrl(apiName, controller, url);
        }

        public SearchForm(string apiUrl, int top = 30, string expand = null)
        {  
            ApiUrl = apiUrl;
            Top = top;
            Expand = expand;
        }

        public void Select(string field)
        {
            Selects.Add(field);
        }

        public void Select(List<string> fields)
        {
            foreach(string field in fields)
            {
                Select(field);
            }
        }
        
        public void AddSearchField(SearchField searchField)
        {
            if (searchField.Options != null)
            {
                Translations[searchField.Name] = searchField.Options;
           
                foreach(SelectListItem option in searchField.Options)
                {
                    if(option.Text == null || option.Text == "")
                    {
                        option.Text = option.Value != null ? option.Value : "[null]";
                    
                    }
                }
            }
            if(string.IsNullOrWhiteSpace(searchField.Label))
            {
                searchField.Label = TextService.Title(searchField.Name);
            }

            if(string.IsNullOrWhiteSpace(searchField.Placeholder))
            {
                searchField.Placeholder = TextService.Title(searchField.Comparator).ToLower();
            }
            SearchFields.Add(searchField);
        }

        public void AddSortField(SortField sortField)
        {
            if (string.IsNullOrWhiteSpace(sortField.Label))
            {
                sortField.Label = TextService.Title(sortField.Name);
            }
            SortFields.Add(sortField);
        }

        public void SetSearchValue(string fieldName, string[] values)
        {
            foreach(SearchField searchField in SearchFields)
            {
                if(searchField.Name.Equals(fieldName))
                {
                    searchField.Value = values;
                }
            }
        }
        public void SetApiUrl(UrlHelper url)
        {
            if (ApiUrl == null && ApiName != null && Controller != null)
            {
                ApiUrl = url.RouteUrl(ApiName, new { httproute = "", controller = Controller });
            }
        }

        public void SetApiUrl(string apiName, string controller, UrlHelper url)
        {
            ApiName = apiName;
            Controller = controller;
            
            if (url != null)
            {
                SetApiUrl(url);
            }
        }
    }

    public abstract class AbstractSearchField
    {
        //Data Types, Values from oData Specification
        //https://github.com/devnixs/ODataAngularResources
        public const string boolType = "Boolean";
        public const string byteType = "Byte";
        public const string datetimeType = "DateTime";
        public const string decimalType = "Decimal";
        public const string doubleType = "Double";
        public const string guidType = "Guid";
        public const string intType = "Int32";
        public const string stringType = "String";
        public const string singleType = "Single";

        //Search Types
        //These Should really be a sub class
        public static class Comparators
        {
            public const string ContainsAll = "containsall";
            public const string ContainsAny = "containsany";
            public const string StartsWith = "startsWith";
            public const string EndsWith = "endsWith";
            public const string Equal = "equals";
            public const string Range = "range";
            public const string TextSearch = "textSearch";
            //public const string IsNull = "isnull";
            //public const string IsNotNull = "isnotnull";
        }

        public static string[] InBetweenDataTypes = new string[]
        {
            datetimeType,
            decimalType,
            doubleType,
            intType,
            singleType
        };

        public static class Operators
        {
            public const string And = "And";
            public const string Or = "Or";
        }

        public string Name { get; set; }
        public string[] Value { get; set; }
        public string DataType { get; set; }
        public string Comparator { get; set; }
        public string Operator { get; set; } = Operators.And;

        public static IEnumerable<SelectListItem> nullYesNoOptions = new List<SelectListItem>()
        {
            new SelectListItem() { Value = "[null]", Text = "[null]" },
            new SelectListItem() { Value = "true", Text = "Yes" },
            new SelectListItem() { Value = "false", Text = "No" }

        };

        public static IEnumerable<SelectListItem> yesNoOptions = new List<SelectListItem>()
        {
            new SelectListItem() { Value = "true", Text = "Yes" },
            new SelectListItem() { Value = "false", Text = "No" }

        };
    }

    public class RestrictSearchField : AbstractSearchField
    {
        public RestrictSearchField(string name, string[] value, string comparator = Comparators.Equal, string dataType = stringType , string operation = Operators.And)
        {
            Name = name;
            Value = value;
            DataType = dataType;
            Comparator = comparator;
            Operator = operation;
        }
    }

    public class SearchField : AbstractSearchField
    {
 
        public SearchField(
            string name
            , string label
            , string comparator
            , string dataType = stringType
            , string[] value = null
            , IEnumerable<SelectListItem> options = null
            , string operation = Operators.And
        )
        {
            //To search related object, use double underscore for notation in the Name paramter.
            //Example: RelatedModel__FieldName 
            Name = name;
            Label = label;
            Comparator = comparator;
            DataType = dataType;
            Value = value;
            Options = options;
            Operator = operation;

            if (Options != null)
            {  
                foreach (var option in Options)
                {

                    if (option.Value == null || option.Value == "")
                    {
                        option.Value = "null";
                    }
                    if (option.Text == null || option.Text == "")
                    {
                        option.Text = "Empty";
                    }
                    
                }
            }
        }

        public string Label { get; set; }
        public string Placeholder { get; set; }
        public IEnumerable<SelectListItem> Options;

        public Dictionary<string, string> inputTypeMap = new Dictionary<string, string>() {
             {  boolType , "checkbox" }
            , { byteType , "text" }
            , { datetimeType , "date" }
            , { decimalType , "text" }
            , { doubleType , "text" }
            , { guidType , "text" }
            , { intType , "number" }
            , { stringType , "text" }
            , { singleType , "text" }
        };

        public string InputType {
            get {
                return this.inputTypeMap[this.DataType];
            }
        }

        /// <summary>
        /// Provides the available comparators based on data type and options avialability
        /// </summary>
        /// <returns></returns>
        public SelectList ComparatorOptions()
        {
            List<SelectListItem> options = new List<SelectListItem>();

            if (!Comparator.Equals(SearchField.Comparators.TextSearch))
            {
                options.Add(new SelectListItem() { Value = Comparators.Equal, Text = "Equals", Group = new SelectListGroup() { Name = "Any" }});
                options.Add(new SelectListItem() { Value = "!" + Comparators.Equal, Text = "Not equals", Group = new SelectListGroup() { Name = "Any" } });
            }
            else
            {
                options.Add(new SelectListItem() { Value = Comparators.TextSearch, Text = "Full-Text Search", Group = new SelectListGroup() { Name = "Any" } });
            }

            if (SearchField.InBetweenDataTypes.Contains(DataType))
            {
                options.Add(new SelectListItem() { Value = Comparators.Range, Text = "Between", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = "!" + Comparators.Range, Text = "Not between", Group = new SelectListGroup() { Name = "Any" } });
            }

            if ((Options == null || Options.Count() == 0) &&
                DataType.Equals(stringType) &&
                !Comparator.Equals(SearchField.Comparators.TextSearch))
            {
                options.Add(new SelectListItem() { Value = Comparators.ContainsAny, Text = "Contains any", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = "!" + Comparators.ContainsAny, Text = "Not contains any", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = Comparators.StartsWith, Text = "Starts with", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = "!" + Comparators.StartsWith, Text = "Not starts with", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = Comparators.EndsWith, Text = "Ends with", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = "!" + Comparators.EndsWith, Text = "Not ends with", Group = new SelectListGroup() { Name = "Any" } });
                options.Add(new SelectListItem() { Value = Comparators.ContainsAll, Text = "Contains all", Group = new SelectListGroup() { Name = "All" } });
                options.Add(new SelectListItem() { Value = "!" + Comparators.ContainsAll, Text = "Not contains all", Group = new SelectListGroup() { Name = "All" } });
            }


            //options.Add(new SelectListItem() { Value = Comparators.IsNull, Text = "Is NULL", Group = new SelectListGroup() { Name = "Nullables" } });
            //options.Add(new SelectListItem() { Value = Comparators.IsNotNull, Text = "Is NOT NULL", Group = new SelectListGroup() { Name = "Nullables" } });
            return new SelectList(options, "Value", "Text", "Group.Name", null, null);
        }


    }
        public class SortField
    {   

        public SortField(string name, string label = null, bool isDefault = false, bool isDescending = false) {
            this.Name = name;
            this.Label = label;
            this.IsDefault = isDefault;
            this.IsDescending = isDescending;
        }

        public string Name { get; set; }
        public string Label { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDescending { get; set; }

    }

    public static class FieldHelper
    {
        public static string Not(this string text)
        {
            return "!" + text;
        }
    }
}