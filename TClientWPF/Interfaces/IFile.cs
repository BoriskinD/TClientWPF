using TClientWPF.Model;

namespace TClientWPF
{
    public interface IFile
    {
        Settings Open(string fileName);
        void Save(string fileName, Settings currentSettings);
    }
}
