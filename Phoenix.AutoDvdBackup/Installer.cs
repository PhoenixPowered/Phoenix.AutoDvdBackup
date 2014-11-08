using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Phoenix.AutoDvdBackup
{
    public static class Installer
    {
        public static void Install()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var fileName = Path.GetFileNameWithoutExtension(executingAssembly.CodeBase);
            var appName = string.Join(string.Empty, fileName.Where(char.IsLetterOrDigit));

            RegistryKey root = Registry.ClassesRoot;
            root = root.OpenSubKey("DVD\\shell", true);

            var ripperKey = root.OpenSubKey(appName);

            var executingAssemblyLocation = executingAssembly.Location;

            if(ripperKey == null)
            {
                var newKey = root.CreateSubKey(appName);
                newKey.SetValue("", appName);

                newKey = newKey.CreateSubKey("command");
                newKey.SetValue("", executingAssemblyLocation + " %L");
            }
            else
            {
                return;
            }

            root = Registry.LocalMachine;
            root = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers");

            var handlers = root.OpenSubKey("Handlers", true);
            var newSubKey = handlers.CreateSubKey(appName);
            newSubKey.SetValue("Action", "Rip");
            newSubKey.SetValue("DefaultIcon", executingAssemblyLocation + ",0");
            newSubKey.SetValue("InvokeProgId", "DVD");
            newSubKey.SetValue("InvokeVerb", appName);
            newSubKey.SetValue("Provider", appName);

            var eventHandler = root.OpenSubKey("EventHandlers\\PlayDVDMovieOnArrival", true);
            eventHandler.SetValue(appName, "");
        }

        public static void Uninstall()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var fileName = Path.GetFileNameWithoutExtension(executingAssembly.CodeBase);
            var appName = string.Join(string.Empty, fileName.Where(char.IsLetterOrDigit));

            var root = Registry.ClassesRoot;
            root = root.OpenSubKey("DVD\\shell", true);

            var ripperKey = root.OpenSubKey(appName);

            if(ripperKey == null)
            {
                return;
            }

            root.DeleteSubKeyTree(appName);

            root = Registry.LocalMachine;

            var handlers = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers\\Handlers", true);

            handlers.DeleteSubKeyTree(appName);

            var eventHandler = root.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers\\EventHandlers\\PlayDVDMovieOnArrival", true);

            eventHandler.DeleteValue(appName);
        }
    }
}
