using FlatFilesExham.Core;

var helper = new NugetCsvHelper();
Console.WriteLine("Iniciando aplicación de ejemplo de Archivos Planos...");
var listUser = "Users.csv".ToList(); // añadir inicio de seccion maximo 3 intentos

Console.WriteLine("Escriba el nombre de la lista de personas a crear (por defecto 'people.csv'):");
var listName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(listName))
{
    listName = "people.csv";
}

var options = string.Empty;

do
{
    options = MyMenu();
    Console.WriteLine();
    Console.WriteLine();
    switch (options)
    {
        case "1":
            {
                ShowConten();
                break;
            }
        case "2":
            {
                var person = new Person();
                Console.Write("Escriba el nombre: ");
                person.Name = Console.ReadLine() ?? string.Empty;
                Console.Write("Escriba el teléfono: ");
                person.Phone = Console.ReadLine() ?? string.Empty;
                Console.Write("Escriba la ciudad: ");
                person.City = Console.ReadLine() ?? string.Empty;
                Console.Write("Escriba el balance: ");
                var balanceInput = Console.ReadLine() ?? "0";
                if (decimal.TryParse(balanceInput, out var balance))
                {
                    person.Balance = balance;
                }
                else
                {
                    person.Balance = 0;
                }
                var people = helper.Read(listName).ToList();
                people.Add(person);
                helper.Write(listName, people);
                Console.WriteLine("Persona agregada exitosamente.");
                break;
            }
    }
} while (options != "0");

void ShowConten()
{
    var people = helper.Read(listName);
    Console.WriteLine("Contenido de la lista:");
    foreach (var person in people)
    {
        Console.WriteLine($"Name: {person.Name}, Phone: {person.Phone}, City: {person.City}, Balance: {person.Balance}");
    }
    return;
}

string MyMenu()
{
    Console.WriteLine();
    Console.WriteLine("Seleccione una opción:");
    Console.WriteLine("1. Show Content - Mostrar contenido");
    Console.WriteLine("2. Add person - Agregar persona");
    Console.WriteLine("3. Save Changes - Guardar cambios");
    Console.WriteLine("0. Exit - Salir");
    Console.Write("Choose an option: - Elije una opción: ");
    Console.WriteLine();
    return Console.ReadLine() ?? string.Empty;
}