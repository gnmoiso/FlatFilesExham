using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFilesExham.Core;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _Writer;

    public LogWriter(string path)
    {
        _Writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }
    public void WriteLog(string level, string message, string user)
    {
        var timestamp = DateTime.Now.ToString("s");
        _Writer.WriteLine($"{timestamp} [{level}] [Usuario: {user}] {message}");
    }

    public void Dispose() => _Writer.Dispose();
}
    
