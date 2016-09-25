using System.Windows.Controls;

namespace Informer.Core
{
    public interface IInfomerApplication
    {
        UserControl Control { get; set; }
        string Path { get; set; }
        string FriendlyName { get; set; }
        string AssemblyName { get; set; }
        byte[] Image { get; set; }

        void RunApplication();
        void StopApplication();
    }
}

