
using System.ComponentModel;

namespace BuslyCLI.TypeConverters;

public class ExpandedPathTypeConverter : System.ComponentModel.TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
    {
        if (value is string str)
        {
            return new ExpandedPath(str);
        }

        return base.ConvertFrom(context, culture, value);
    }
}