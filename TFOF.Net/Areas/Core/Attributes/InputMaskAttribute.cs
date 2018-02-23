namespace TFOF.Areas.Core.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class InputmaskAttribute: Attribute
    {   
        public InputmaskAttribute(string mask)
        {
            Mask = mask;
        }
        public string Mask { get; set; }

        public static class Masks
        {
            public const string PhoneNumber = "'mask': '999/999-9999'";
            public const string PhoneNumberUS = "'mask': '(999) 999-9999'";
            public const string SocialSecurityNumber = "'mask': '999-99-9999'";
            public const string ZipCode = "'mask': '99999-9999'";
        } 
    }
}