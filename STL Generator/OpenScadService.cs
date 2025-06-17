using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace STL_Generator
{
    public class OpenScadService
    {
        private readonly string _openScadPath;

        public OpenScadService(string openScadPath)
        {
            _openScadPath = openScadPath;
        }

        public async Task<(bool Success, string ErrorMessage)> GenerateStlAsync(string scadFilePath, string stlFilePath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _openScadPath,
                Arguments = $"-o \"{stlFilePath}\" \"{scadFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
                return (false, "Failed to start OpenSCAD process.");
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            bool success = process.ExitCode == 0 && File.Exists(stlFilePath);
            return (success, success ? string.Empty : error);
        }
    }
}
