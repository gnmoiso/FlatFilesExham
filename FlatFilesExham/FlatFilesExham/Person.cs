using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFilesExham.Core;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string City { get; set; }
    public decimal Balance { get; set; }

    public Person()
    {

        Id = 0;
        Name = "xxx xx";
        Phone = "xxx";
        City = "xxx";
        Balance = 0m;
    }
}