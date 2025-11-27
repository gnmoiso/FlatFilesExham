using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FlatFilesExham.Core;

public class NugetCsvHelper
{
    private const string DefaultName = "xx";
    private const string DefaultPhone = "xxx";
    private const string DefaultCity = "xxx";
    private const string DefaultExtension = ".csv";

    public void Write(string path, IEnumerable<Person> people)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("path must not be null or whitespace.", nameof(path));
        }

        if (!Path.HasExtension(path))
        {
            path = path + DefaultExtension;
        }

        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        }

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var normalized = people.Select(p =>
        {
            if (string.IsNullOrWhiteSpace(p.Name)) p.Name = DefaultName;
            if (string.IsNullOrWhiteSpace(p.Phone)) p.Phone = DefaultPhone;
            if (string.IsNullOrWhiteSpace(p.City)) p.City = DefaultCity;
            return p;
        }).ToList();

        // Asignar Ids enteros secuenciales para registros sin Id (Id <= 0)
        var nextId = normalized.Any() ? normalized.Max(p => p.Id) : 0;
        foreach (var p in normalized)
        {
            if (p.Id <= 0) p.Id = ++nextId;
        }

        using var sw = new StreamWriter(path, false, Encoding.UTF8);
        using var cw = new CsvWriter(sw, CultureInfo.InvariantCulture);
        cw.WriteRecords(normalized);
    }

    public IEnumerable<Person> Read(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("path must not be null or whitespace.", nameof(path));
        }

        if (!Path.HasExtension(path))
        {
            path = path + DefaultExtension;
        }

        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
        }

        if (!File.Exists(path))
        {
            return Enumerable.Empty<Person>();
        }

        using var sr = new StreamReader(path, Encoding.UTF8);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        };

        using var cr = new CsvReader(sr, config);
        var records = cr.GetRecords<Person>().ToList();

        // Asegurar Ids únicos para registros que vienen sin Id
        var nextId = records.Any() ? records.Max(p => p.Id) : 0;
        foreach (var r in records)
        {
            if (r.Id <= 0) r.Id = ++nextId;
            if (string.IsNullOrWhiteSpace(r.Name)) r.Name = DefaultName;
            if (string.IsNullOrWhiteSpace(r.Phone)) r.Phone = DefaultPhone;
            if (string.IsNullOrWhiteSpace(r.City)) r.City = DefaultCity;
        }

        return records;
    }
}