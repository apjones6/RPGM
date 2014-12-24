using System;
using System.Reflection;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class ObjectPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
            {
                return value;
            }

            var obj = value;
            var path = parameter.ToString().Split('.');
            Type type = null;

            // Handle dot-notation for properties
            // TODO: Support bracket notation for those property syntaxes
            foreach (var propertyName in path)
            {
                // NOTE: Use the property type instead of instance type if available (fix for COM objects)
                var property = FindProperty(type ?? obj.GetType(), propertyName);
                if (property != null)
                {
                    obj = property.GetValue(obj);
                    type = property.PropertyType;
                }
                else
                {
                    obj = null;
                }
                
                // Handles both property missing and null value
                if (obj == null)
                {
                    break;
                }
            }

            return obj;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static PropertyInfo FindProperty(Type type, string propertyName)
        {
            var info = type.GetTypeInfo();
            var prop = info.GetDeclaredProperty(propertyName);

            // Try base type (recursive)
            if (prop == null && info.BaseType != null)
            {
                prop = FindProperty(info.BaseType, propertyName);
            }

            return prop;
        }
    }
}
