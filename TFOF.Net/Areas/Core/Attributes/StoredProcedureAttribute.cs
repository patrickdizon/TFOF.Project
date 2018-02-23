namespace TFOF.Areas.Core.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class StoredProcedureAttribute: Attribute
    {
        public string Name { get; set; }

        public static string GetName<T>()
        {
            StoredProcedureAttribute storedProcedureAttribute =
               (StoredProcedureAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(StoredProcedureAttribute));

            return storedProcedureAttribute.Name;
        }
    }
}