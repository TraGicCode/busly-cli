using System.ComponentModel;

namespace BuslyCLI.TypeConverters;

public class MessageBodyTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {

        if (value is string str)
        {
            // Curl-style @file reference: parse only, no I/O
            if (str.StartsWith("@"))
            {
                var expandedFilePath = new ExpandedPath(str[1..]);
                return new MessageBodyValue(expandedFilePath.Path);
            }
        }

        return base.ConvertFrom(context, culture, value);
    }
}