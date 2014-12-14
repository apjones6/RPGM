using System;
using Windows.UI.Xaml.Data;

namespace RPGM.Notes.Converters
{
    public class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Avoid duplicating whole process
            if (value is DateTime)
            {
                value = new DateTimeOffset((DateTime)value);
            }
            
            // Only convert this class; anything else is returned unmodified
            if (value is DateTimeOffset)
            {
                var datetime = (DateTimeOffset)value;
                var now = DateTimeOffset.UtcNow;

                // Show time if it's today
                if (datetime.Date == now.Date)
                {
                    var timespan = (now - datetime);
                    if (timespan.Hours == 1)
                    {
                        return "1 hour ago";
                    }
                    else if (timespan.Hours > 0)
                    {
                        return string.Format("{0} hours ago", timespan.Hours);
                    }
                    else if (timespan.Minutes == 1)
                    {
                        return "1 minute ago";
                    }
                    else if (timespan.Minutes > 0)
                    {
                        return string.Format("{0} minutes ago", timespan.Minutes);
                    }
                    else
                    {
                        return "Just now";
                    }
                }

                // Show yesterday
                else if (datetime.Date.AddDays(1) == now.Date)
                {
                    return "Yesterday";
                }

                // Show week days for the last week
                else if (datetime.Date.AddDays(6) > now.Date)
                {
                    return datetime.DayOfWeek.ToString();
                }

                // Show the date
                else
                {
                    return datetime.ToString("dd/MM/yyyy");
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
