using GenericApp.Prism.Droid.Services;
using GenericApp.Prism.Services;
using Xamarin.Forms;
using System;
using System.IO;

[assembly: Dependency(typeof(FileHelper))]
namespace GenericApp.Prism.Droid.Services
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, fileName);
        }
    }
}