namespace FlatFilesExham.Core;

public class SimpleTextFile
{
    private readonly string _Path;

    public SimpleTextFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Path cannot be null or whitespace", nameof(path));
        }
        _Path = path;

        var directory = Path.GetDirectoryName(_Path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_Path))
        {
            using var fs = File.Create(_Path);
        }
    }

    public void WriteAllLines(IEnumerable<string> lines)
    {
        File.WriteAllLines(_Path, lines);
    }

    public string[] ReadAllLines()
    {
        return File.ReadAllLines(_Path);
    }
}