using System.IO;
using System.Reflection;
using System.Xml.Serialization;

public static class IEnumerableExtensions
{
    public static string? WriteToXml<T>(this List<T> items)
    {
        XmlSerializer xmlSerializer = new(typeof(List<T>));
        using StringWriter stringWriter = new();
        xmlSerializer.Serialize(stringWriter, items.ToList());

        return stringWriter.ToString();
    }

    public static void WriteToCsv<T>(this IEnumerable<T> items, string path)
    {
        Type itemType = typeof(T);
        var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(pi => pi.PropertyType == typeof(string) || pi.PropertyType == typeof(bool) || pi.PropertyType.IsPrimitive).ToArray();

        using StreamWriter sw = new(path);
        // Writing header (property names)
        sw.WriteLine(string.Join(",", props.Select(p => p.Name)));
        // Writing data
        foreach (var item in items)
            if (item is not null)
                sw.WriteLine(string.Join(",", props.Select(p => GetValue(p, item))));
    }

    private static string? GetValue(PropertyInfo propertyInfo, object obj)
        => propertyInfo.GetValue(obj, null).ToString().Replace(",", ";");
}
