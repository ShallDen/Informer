using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Informer.Core;
using System.Linq;

namespace QuickApp
{
    public class ApplicationManager
    {
        private static ApplicationManager mInstance;
        private List<IInfomerApplication> mApplications;
        private List<ApplicationWindow> mAppWindows;

        public static ApplicationManager Instance
        {
            get { return mInstance ?? (mInstance = new ApplicationManager()); }
        }

        public List<IInfomerApplication> Applications
        {
            get { return mApplications; }
            set { mApplications = value; }
        }

        public List<ApplicationWindow> AppWindows
        {
            get { return mAppWindows ?? (mAppWindows = new List<ApplicationWindow>()); }
            set { mAppWindows = value; }
        }

        public ApplicationManager()
        {
            mApplications = new List<IInfomerApplication>();
        }

        public void LoadApplications()
        {
            try
            {
                OrganizeApplicationAssemblies();

                var appPaths = GetAppPaths();

                foreach (var appPath in appPaths)
                {
                    IInfomerApplication application = null;
                    Assembly asm = Assembly.LoadFrom(appPath);
                    Type[] tlist = asm.GetTypes();

                    foreach (Type t in tlist)
                    {
                        Type type = t.GetInterface("IInfomerApplication");

                        if (type != null)
                        {
                            application = asm.CreateInstance(t.FullName) as IInfomerApplication;
                            mApplications.Add(application);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void OrganizeApplicationAssemblies()
        {
            try
            {
                // Get path and files of main Quick application
                var mainPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on);
                var mainPathFiles = Directory.GetFiles(mainPath);
                var mainPathFileNames = new List<string>();

                // Get file paths in Quick app folders
                foreach (var mainPathFile in mainPathFiles)
                {
                    var name = Path.GetFileName(mainPathFile);
                    mainPathFileNames.Add(name);
                }

                var modulePath = mainPath + @"\Modules\";

                if (Directory.Exists(modulePath))
                {
                    // Get files from \Modules folder
                    var appDirectories = Directory.GetDirectories(modulePath);

                    // Loop for all application modules
                    foreach (var appDirectory in appDirectories)
                    {
                        if (Directory.Exists(appDirectory))
                        {
                            // Get all files from module
                            var modulePathFiles = Directory.GetFiles(appDirectory);
                            var modulePathFileNames = new List<string>();

                            // Get all file names from module
                            foreach (var modulePathFile in modulePathFiles)
                            {
                                var name = Path.GetFileName(modulePathFile);
                                modulePathFileNames.Add(name);
                            }

                            // Find assemblies which \Module folder is already contains
                            var fileNamesToDelete = mainPathFileNames.Intersect(modulePathFileNames).ToList();

                            // Delete files from app folder to keep only required assemblies
                            foreach (var fileNameToDelete in fileNamesToDelete)
                            {
                                var file = modulePathFiles.FirstOrDefault(c => c.Contains(fileNameToDelete));
                                if (File.Exists(file))
                                {
                                    File.Delete(file);
                                }
                            }

                            // Delete not .dll files
                            var otherFiles = Directory.GetFiles(appDirectory).Where(c => !c.EndsWith(".dll")).ToList();
                            otherFiles.ForEach(file => File.Delete(file));

                            // Get files from app folder
                            modulePathFiles = Directory.GetFiles(appDirectory);

                            // Copy dll's to \Modules folder
                            foreach (var file in modulePathFiles)
                            {
                                var name = Path.GetFileName(file);
                                File.Copy(Path.Combine(appDirectory, name), Path.Combine(modulePath, name), true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private List<string> GetAppPaths()
        {
            var appPaths = new List<string>();

            var mainPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on);
            var modulePath = mainPath + @"\Modules";

            // Get applications and their references from \Modules folder
            if (Directory.Exists(modulePath))
            {
                appPaths = Directory.GetFiles(modulePath).ToList();
                // appPaths = Directory.GetFiles(modulePath).Where(c=>c.EndsWith(".dll")).ToList();
            }

            return appPaths;
        }
    }
}
