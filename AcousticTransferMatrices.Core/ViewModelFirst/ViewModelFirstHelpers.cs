using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace AcousticTransferMatrices.Core.ViewModelFirst
{

    public static class ViewModelFirstHelpers
    {
        /// <summary>
        /// Register ViewModel first approach. Make sure the resource for the DataTemplate is added.
        /// </summary>
        /// <typeparam name="T">The ViewModel class</typeparam>
        /// <param name="container">Prism's container</param>
        public static void RegisterViewModelForNavigation<T>(this IContainerRegistry container)
        {
            container.Register(typeof(Object), typeof(T), typeof(T).FullName);
        }


        /// <summary>
        /// The function adds all xaml files in teh folder fond in the current project.
        /// </summary>
        /// <param name="main">Main resourcedirectory. Has to be initializxed as an instance in the Bootstrapper</param>
        /// <param name="current">The link to the current project with the "this" parameter passed in</param>
        /// <param name="folder">The folder as for instance "/Views" where all the resource files are located</param>
        public static void MergeResourceDirectories(this ResourceDictionary main, object current, string folder)
        {

            string[] files = GetXAMLResourceDirectories(current, GetXAMLResources(current, folder));

            if (files != null)
            {
                // Loop through all ResourDirectories that should be Mearged with its main resource directory
                foreach (string item in files)
                {
                    // Assembly path to resource
                    string name = current.GetType().Assembly.GetName().Name + ";component/" + item;

                    ResourceDictionary resource = new ResourceDictionary
                    {
                        Source = new Uri(name, UriKind.RelativeOrAbsolute)
                    };
                    // Add the resource if main ResourceDirectory does not alredy have it
                    if (!main.MergedDictionaries.Contains(resource))
                        main.MergedDictionaries.Add(resource);
                }
            }
        }

        /// <summary>
        /// Removes XAML files that have a corresponding XAML.cs 
        /// </summary>
        /// <param name="current">The current project "this"</param>
        /// <param name="xaml">Array of all XAML files found in project</param>
        /// <returns></returns>
        private static string[] GetXAMLResourceDirectories(object current, string[] xaml)
        {
            if (xaml.Count() == 0) return null;

            List<string> result = new List<string>();

            // Main namespace of project
            string WorkingProsject = current.GetType().Assembly.GetName().Name;

            // Names of registered classes
            List<string> RegisteredClasses = current.
                GetType().
                Assembly.
                GetTypes().
                ToList().
                Select(x => x.ToString().
                ToLower()).
                ToList();

            // create complete fullpath names
            string[] XAMLFullPath = xaml.
                ToList().
                // Add full path name
                Select(x => WorkingProsject + "." + x.Split('.')[0].Replace("/", ".").ToLower()).
                ToArray<string>();

            // Loop through and add if no corresponing XAML.cs is found
            for (int i = 0; i < XAMLFullPath.Length; i++)
            {
                if (!RegisteredClasses.Contains(XAMLFullPath[i].ToLower()))
                    result.Add(xaml[i]);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Finds all baml files and returns XAML file name
        /// </summary>
        /// <param name="Current">Current project "this"</param>
        /// <param name="folder">Whitch folder to search for XAML, for example "/Views"</param>
        /// <returns>List of XAML files in <paramref name="Current"/> project</returns>
        private static string[] GetXAMLResources(object Current, string folder)
        {
            List<string> resourceNames = new List<string>();
            var rm = new ResourceManager(Current.GetType().Assembly.GetName().Name + ".g", Current.GetType().Assembly);

            // In case something blows up!
            try
            {
                // Get all runtime resource sets
                ResourceSet list = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true);

                // In case we have no resources
                if (list != null)
                {
                    foreach (DictionaryEntry item in list)
                    {
                        // Find the current baml resource
                        string keyItem = ((string)item.Key);

                        // Is it in the spesific folder?
                        //TODO: Make this better?
                        if (keyItem.Contains(folder.ToLower() + "/"))
                            resourceNames.Add(keyItem.Replace("baml", "xaml"));
                    }
                }
            }
            finally
            {
                rm.ReleaseAllResources();
            }

            return resourceNames.ToArray();
        }
    }
}

