using System.ComponentModel;

namespace BuslyCLI.TypeConverters;

[TypeConverter(typeof(ExpandedPathTypeConverter))]
public class ExpandedPath
{
    public string Path { get; }
    public ExpandedPath(string path)
    {
        Path = ExpandTilde(path);
    }

    private string ExpandTilde(string value)
    {
        if (value.StartsWith("~"))
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            value = System.IO.Path.Combine(home, value.TrimStart('~').TrimStart('/'));
        }
        return System.IO.Path.GetFullPath(value);
    }
}