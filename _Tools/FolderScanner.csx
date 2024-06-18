using System;
using System.IO;
using System.Linq;

void ScanDirectory(DirectoryInfo directoryInfo, StreamWriter writer, string indent = "")
{
    var excludedFolders = new[] { ".vs", "bin", "obj", "node_modules" };

    // Process directories
    try
    {
        foreach (var directory in directoryInfo.GetDirectories().Where(d => !excludedFolders.Any(folder => d.Name.Contains(folder))))
        {
            writer.WriteLine($"{indent}{directory.Name}/");
            ScanDirectory(directory, writer, indent + "    ");
        }
    }
    catch (UnauthorizedAccessException)
    {
        writer.WriteLine($"{indent}Access denied to directory: {directoryInfo.FullName}/");
    }

    // Process files
    try
    {
        foreach (var file in directoryInfo.GetFiles().Where(file => !excludedFolders.Any(folder => file.DirectoryName.Contains(folder))))
        {
            writer.WriteLine($"{indent}-{file.Name}");
        }
    }
    catch (UnauthorizedAccessException)
    {
        writer.WriteLine($"{indent}Access denied to file: {directoryInfo.FullName}/");
    }
}

var args = Environment.GetCommandLineArgs();

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet script thisscript.csx <directoryToScan>");
    return;
}

var directoryToScan = args[1];
var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var outputFileName = $"DirectoryStructure_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
var outputFilePath = Path.Combine(desktopPath, outputFileName);

DirectoryInfo directoryInfo = new DirectoryInfo(directoryToScan);

using (StreamWriter writer = new StreamWriter(outputFilePath))
{
    ScanDirectory(directoryInfo, writer);
}

Console.WriteLine("Directory structure has been written to " + outputFilePath);

