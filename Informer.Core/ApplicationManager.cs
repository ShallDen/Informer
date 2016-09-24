using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Informer.Core
{
    public class ApplicationManager
    {
        private static ApplicationManager mInstance;
        private List<IInfomerApplication> mApplications;

        public static ApplicationManager Instance
        {
            get { return mInstance ?? (mInstance = new ApplicationManager()); }
        }

        public List<IInfomerApplication> Applications
        {
            get { return mApplications; }
            set { mApplications = value; }
        }

        public ApplicationManager()
        {
            mApplications = new List<IInfomerApplication>();
        }

        public void LoadApplications()
        {
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
               // application.RunApplication();
            }
        }

        private List<string> GetAppPaths()
        {
            var appPaths = new List<string>();

            var mainPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Locati‌​on);
            var modulePath = mainPath + @"\Modules";

            if (Directory.Exists(modulePath))
            {
                var appDirectories = Directory.GetDirectories(modulePath);

                foreach (var appDirectory in appDirectories)
                {
                    string lastFolderName = Path.GetFileName(appDirectory);
                    string appPath = appDirectory + @"\" + lastFolderName + ".dll";

                    if (File.Exists(appPath))
                    {
                        appPaths.Add(appPath);
                    }
                }
            }

            return appPaths;
        }
    }
}
