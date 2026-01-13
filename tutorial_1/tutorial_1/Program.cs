using System;
using System.IO;
using System.Globalization;

public class Example
{
    public static void Main()
    {
        var os = Environment.OSVersion;
        Console.WriteLine("Current OS Information:\n");
        Console.WriteLine("Platform: {0:G}", os.Platform);
        Console.WriteLine("Version String: {0}", os.VersionString);
        Console.WriteLine("Version Information:");
        Console.WriteLine("   Major: {0}", os.Version.Major);
        Console.WriteLine("   Minor: {0}", os.Version.Minor);
        Console.WriteLine("Service Pack: '{0}'", os.ServicePack);
        try
        {
            // Get the current directory.
            string path = Directory.GetCurrentDirectory();
            string target = @"c:\temp";
            Console.WriteLine("The current directory is {0}", path);
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }

            // Change the current directory.
            Environment.CurrentDirectory = (target);
            if (target.Equals(Directory.GetCurrentDirectory()))
            {
                Console.WriteLine("You are in the temp directory.");
            }
            else
            {
                Console.WriteLine("You are not in the temp directory.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("The process failed: {0}", e.ToString());
        }
        DateTime localDate = DateTime.Now;
        DateTime utcDate = DateTime.UtcNow;
        String[] cultureNames = { "en-US", "en-GB", "fr-FR",
                                "de-DE", "ru-RU" };

        foreach (var cultureName in cultureNames)
        {
            var culture = new CultureInfo(cultureName);
            Console.WriteLine("{0}:", culture.NativeName);
            Console.WriteLine("   Local date and time: {0}, {1:G}",
                              localDate.ToString(culture), localDate.Kind);
            Console.WriteLine("   UTC date and time: {0}, {1:G}\n",
                              utcDate.ToString(culture), utcDate.Kind);
        }
        Console.ReadKey();
    }
}
