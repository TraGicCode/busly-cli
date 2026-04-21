using System.ComponentModel;

namespace BuslyCLI.TypeConverters;

[TypeConverter(typeof(MessageBodyTypeConverter))]
public class MessageBodyValue
{
    public string Value { get; set; }


    public MessageBodyValue(string value)
    {
        Value = value;
    }
}