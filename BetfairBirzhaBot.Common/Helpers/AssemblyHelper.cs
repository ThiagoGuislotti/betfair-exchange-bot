using System.Reflection;

namespace BetfairBirzhaBot.Common.Helpers
{
    public static class AssemblyHelper
    {
        public static string ReadResource(string name)
        {
            // Determine path
            var assemblies = new List<Assembly>
            {
                Assembly.GetExecutingAssembly(),
                Assembly.GetCallingAssembly(),
                Assembly.GetEntryAssembly()
            };

            foreach (var assembly in assemblies)
            {
                var result = GetTextFromAssembly(assembly, name);
                if (result.Item1)
                    return result.Item2;
            }

            throw new Exception("Assembly not found");
        }

        private static (bool, string) GetTextFromAssembly(Assembly assembly, string file)
        {
            var recourseNames = assembly.GetManifestResourceNames().ToList();
            var neededRecource = recourseNames.Find(x => x.EndsWith(file));

            if (neededRecource is null)
                return (false, "");

            using (Stream stream = assembly.GetManifestResourceStream(neededRecource))
            using (StreamReader reader = new StreamReader(stream))
            {
                return (true, reader.ReadToEnd());
            }
        }
    }
}
