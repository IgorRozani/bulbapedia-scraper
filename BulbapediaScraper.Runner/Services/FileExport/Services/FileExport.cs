using BulbapediaScraper.Runner.Configurations;
using BulbapediaScraper.Runner.Services.FileExport.Interfaces;
using System.IO;

namespace BulbapediaScraper.Runner.Services.FileExport.Services
{
    public class FileExport : IFileExport
    {
        private readonly FileExportConfiguration _fileExportConfiguration;

        public FileExport(FileExportConfiguration fileExportConfiguration)
        {
            _fileExportConfiguration = fileExportConfiguration;
        }

        public void Export(string content)
        {
            if (File.Exists(_fileExportConfiguration.FileFullPath))
                File.Delete(_fileExportConfiguration.FileFullPath);
            File.WriteAllText(_fileExportConfiguration.FileFullPath, content);
        }
    }
}
