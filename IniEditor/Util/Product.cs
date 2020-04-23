using System.Reflection;

namespace IniEditor.Util
{
    public static class Product
    {
        public static string Name { get; } = Assembly.GetExecutingAssembly().GetName().Name;
    }
}
