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

            var path = parameter.ToString().Split('.');
            var obj = value;

            // Handle dot-notation for properties
            // TODO: Support bracket notation for those property syntaxes
            foreach (var propertyName in path)
            {
                var property = FindProperty(obj.GetType(), propertyName);
                if (property != null)
                {
                    obj = property.GetValue(obj);
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
