using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Telegram.Bot.Instagram.Tests.Utils
{
    public static class Resources
    {
        private static string GetResourceAsStringContent(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = name;

            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(assembly.FullName)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using var reader = new StreamReader(stream ?? throw new InvalidOperationException());
            return reader.ReadToEnd();
        }
    }
}
