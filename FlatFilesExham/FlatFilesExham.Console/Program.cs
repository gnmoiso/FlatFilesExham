using FlatFilesExham.Core;

var helper = new NugetCsvHelper();
var userFileHelper = new SimpleTextFile("Users.txt");

var currentLines = userFileHelper.ReadAllLines();

if (currentLines.Length == 0)
{
    var defaultUsers = new List<string>
    {
        "admin,1234,true",
        "jzuluaga,P@ssw0rd123!,true",
        "cgarcia,Sistem@s.2025,true",
        "lrodriguez,C#EsGenial*,true",
        "ana.martinez,Dat0sSegur0s#,true",
        "mbedoya,S0yS3gur02025*,false",
        "fperez,Olvid3MiClav3.,false",
        "drestrepo,ErrorDeCapa8*,false",
        "test,test,true",
        "invitado,guest,true"
    };

    userFileHelper.WriteAllLines(defaultUsers);
}

Console.WriteLine("Iniciando aplicación de ejemplo de Archivos Planos...");

var logHelper = new SimpleTextFile("log.txt");

string loggedUser = null!;
int attempts = 0;
bool accessGranted = false;

while (attempts < 3 && !accessGranted)
{
    Console.WriteLine($"\nIntento {attempts + 1}/3");
    Console.Write("Usuario: ");
    string user = Console.ReadLine()!;
    Console.Write("Contraseña: ");
    string pass = Console.ReadLine()!;

    if (UserValidation(user, pass))
    {
        accessGranted = true;
        loggedUser = user;
        Console.WriteLine($"\n¡Bienvenido {user}!\n");
        LogAction($"Usuario {user} ha iniciado sesión exitosamente.");
    }
    else
    {
        attempts++;
        Console.WriteLine("Credenciales incorrectas o usuario bloqueado.");
        LogAction($"Intento fallido de inicio de sesión para el usuario: {user}.");

        if (attempts == 3)
        {
            BlockUser(user);
            Console.WriteLine($"El usuario {user} ha sido bloqueado (si existía).");
            LogAction($"El usuario {user} ha sido bloqueado tras 3 intentos fallidos.");
        }
    }
}

if (!accessGranted)
{
    Console.WriteLine("\nSe ha excedido el número de intentos. La aplicación se cerrará.");
    return;
}

Console.WriteLine("Escriba el nombre de la lista de personas a crear (por defecto 'people.csv'):");
var listName = Console.ReadLine();
if (string.IsNullOrWhiteSpace(listName))
{
    listName = "people.csv";
}
LogAction($"El usuario {loggedUser} está utilizando o creó la lista: {listName}");

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
                ShowContent();
                LogAction($"Se mostró el contenido de la lista: {listName}");
                break;
            }
        case "2":
            {
                AddPerson();
                break;
            }
        case "3":
            {
                helper.Write(listName, helper.Read(listName));
                Console.WriteLine("Cambios guardados exitosamente.");
                LogAction($"Se guardaron los cambios en la lista: {listName}");
                break;
            }
        case "4":
            {
                EditPerson();
                break;
            }
        case "5":
            {
                DeletePerson();
                break;
            }
        case "6":
            {
                ShowReportByCity();
                break;
            }
        case "7":
            {
                UnlockUser();
                break;
            }
        case "0":
            {
                Console.WriteLine("Saliendo de la aplicación. ¡Hasta luego!");
                LogAction($"El usuario {loggedUser} salió de la aplicación.");
                break;
            }
    }
} while (options != "0");

void AddPerson()
{
    Console.WriteLine("\n--- Agregar Nueva Persona ---");
    var people = helper.Read(listName).ToList();
    var person = new Person();

    bool validId = false;
    do
    {
        Console.Write("Escriba el ID (Cédula - Solo números): ");
        string inputId = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(inputId))
        {
            Console.WriteLine(">> El ID no puede estar vacío.");
            continue;
        }

        if (!int.TryParse(inputId, out var parsedId) || parsedId <= 0)
        {
            Console.WriteLine(">> Error: El ID debe ser un número entero positivo.");
            continue;
        }

        if (people.Any(p => p.Id == parsedId))
        {
            Console.WriteLine($">> Error: El ID '{parsedId}' ya existe en la lista.");
        }
        else
        {
            person.Id = parsedId;
            validId = true;
        }
    } while (!validId);

    bool validName = false;
    do
    {
        Console.Write("Escriba el Nombre completo (Nombres y Apellidos): ");
        string inputName = Console.ReadLine()!;

        if (string.IsNullOrWhiteSpace(inputName))
        {
            Console.WriteLine(">> El nombre no puede estar vacío.");
            continue;
        }

        if (!inputName.Trim().Contains(" "))
        {
            Console.WriteLine(">> Error: Debe ingresar Nombres y Apellidos (separados por espacio).");
            Console.WriteLine("   Ejemplo correcto: 'Moises Zuluaga'");
        }
        else
        {
            person.Name = inputName.Trim();
            validName = true;
        }
    } while (!validName);

    bool validPhone = false;
    do
    {
        Console.Write("Escriba el teléfono (Solo números, 7-15 dígitos): ");
        string inputPhone = Console.ReadLine()!;

        if (long.TryParse(inputPhone, out _) && inputPhone.Length >= 7 && inputPhone.Length <= 15)
        {
            person.Phone = inputPhone;
            validPhone = true;
        }
        else
        {
            Console.WriteLine(">> Error: El teléfono no es válido (use solo números, mín 7 dígitos).");
        }
    } while (!validPhone);

    person.City = ReadOrDefault("Escriba la ciudad: ", "Sin ciudad");

    bool validBalance = false;
    do
    {
        Console.Write("Escriba el balance (saldo positivo): ");
        string inputBalance = Console.ReadLine()!;

        if (decimal.TryParse(inputBalance, out decimal balance))
        {
            if (balance < 0)
            {
                Console.WriteLine(">> Error: El saldo no puede ser negativo.");
            }
            else
            {
                person.Balance = balance;
                validBalance = true;
            }
        }
        else
        {
            Console.WriteLine(">> Error: Por favor ingrese un valor numérico válido.");
        }
    } while (!validBalance);
    people.Add(person);
    helper.Write(listName, people);

    Console.WriteLine("\n Persona agregada exitosamente.");
    LogAction($"Se agregó persona ID: {person.Id}, Nombre: {person.Name}");
}
void ShowContent()
{
    var people = helper.Read(listName);
    var count = 1;
    Console.WriteLine("Contenido de la lista:\n");
    foreach (var person in people)
    {
        Console.WriteLine($"{count++}\tId: {person.Id}\n\tName: {person.Name}\n\tPhone: {person.Phone}\n\tCity: {person.City}\n\tBalance: \t{person.Balance:C2}\n");
    }
    return;
}
void LogAction(string action)
{
    string currentUser = loggedUser ?? "Desconocido";

    string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [Usuario: {currentUser}] {action}";

    logHelper.AppendLine(logLine);
}

string MyMenu()
{
    Console.WriteLine();
    Console.WriteLine("==========================================");
    Console.WriteLine("Seleccione una opción:");
    Console.WriteLine("1. Show Content - Mostrar contenido");
    Console.WriteLine("2. Add person - Agregar persona");
    Console.WriteLine("3. Save Changes - Guardar cambios");
    Console.WriteLine("4. Edit person - Editar persona");
    Console.WriteLine("5. Delete person - Borrar persona");
    Console.WriteLine("6. Report by City - Informe por ciudad");
    Console.WriteLine("7. Unlock User - Desbloquear usuario");
    Console.WriteLine("0. Exit - Salir");
    Console.Write("Choose an option: - Elije una opción: ");
    Console.WriteLine();
    Console.WriteLine("==========================================");
    return Console.ReadLine() ?? string.Empty;
}

static string ReadOrDefault(string prompt, string defaultValue)
{
    Console.Write(prompt);
    var input = Console.ReadLine();
    return string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
}
string ReadWithKeep(string fieldName, string currentValue)
{
    Console.Write($"{fieldName} ({currentValue}): ");
    string input = Console.ReadLine()!;

    return string.IsNullOrWhiteSpace(input) ? currentValue : input.Trim();
}

bool UserValidation(string user, string pass)
{
    var lines = userFileHelper.ReadAllLines();

    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        var parts = line.Split(',');

        if (parts.Length < 3) continue;

        string dbUser = parts[0].Trim();
        string dbPass = parts[1].Trim();
        string dbStatus = parts[2].Trim().ToLower();

        if (dbUser.Equals(user.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            if (dbStatus == "false")
            {
                return false;
            }

            if (dbPass == pass.Trim())
            {
                return true;
            }
        }
    }

    return false;
}

void BlockUser(string userToBlock)
{
    var lines = userFileHelper.ReadAllLines();
    var outputLines = new List<string>();

    foreach (var line in lines)
    {
        var parts = line.Split(',');
        if (parts[0] == userToBlock)
        {
            outputLines.Add($"{parts[0]},{parts[1]},false");
        }
        else
        {
            outputLines.Add(line);
        }
    }

    userFileHelper.WriteAllLines(outputLines);
}
void EditPerson()
{
    Console.WriteLine("\n--- Editar Persona ---");

    var people = helper.Read(listName).ToList();

    Console.Write("Ingrese el ID (Cédula) de la persona a editar: ");
    string idToSearch = Console.ReadLine()!;

    if (!int.TryParse(idToSearch, out var parsedId))
    {
        Console.WriteLine("ID inválido. Debe ser un número entero.");
        return;
    }

    var person = people.FirstOrDefault(p => p.Id == parsedId);

    if (person == null)
    {
        Console.WriteLine("¡Error! No se encontró ninguna persona con ese ID.");
        return;
    }

    Console.WriteLine($"\nEditando datos de: {person.Name}");
    Console.WriteLine("(Presione ENTER sin escribir nada para dejar el valor original)\n");

    person.Name = ReadWithKeep("Nombre", person.Name);
    person.Phone = ReadWithKeep("Teléfono", person.Phone);
    person.City = ReadWithKeep("Ciudad", person.City);

    Console.Write($"Balance ({person.Balance}): ");
    string balanceInput = Console.ReadLine()!;

    if (!string.IsNullOrWhiteSpace(balanceInput))
    {
        if (decimal.TryParse(balanceInput, out decimal newBalance))
        {
            if (newBalance >= 0)
            {
                person.Balance = newBalance;
            }
            else
            {
                Console.WriteLine("El saldo no puede ser negativo. Se mantuvo el anterior.");
            }
        }
        else
        {
            Console.WriteLine("Valor inválido. Se mantuvo el saldo anterior.");
        }
    }

    helper.Write(listName, people);

    Console.WriteLine("\n¡Cambios guardados exitosamente!");
    LogAction($"Se editó la información de la persona ID: {person.Id}");
}
void DeletePerson()
{
    Console.WriteLine("\n--- Borrar Persona ---");

    var people = helper.Read(listName).ToList();

    Console.Write("Ingrese el ID (Cédula) de la persona a eliminar: ");
    string idToSearch = Console.ReadLine()!;

    if (!int.TryParse(idToSearch, out var parsedId))
    {
        Console.WriteLine("ID inválido. Debe ser un número entero.");
        return;
    }

    var person = people.FirstOrDefault(p => p.Id == parsedId);

    if (person == null)
    {
        Console.WriteLine("¡Error! No se encontró ninguna persona con ese ID.");
        return;
    }

    Console.WriteLine($"\n¡ATENCIÓN! Va a eliminar a: {person.Name} (Ciudad: {person.City})");
    Console.Write("¿Está seguro de realizar esta operación? (S/N): ");
    var confirmation = Console.ReadLine();

    if (confirmation?.ToUpper() == "S")
    {
        people.Remove(person);
        helper.Write(listName, people);

        Console.WriteLine("Persona eliminada correctamente.");

        LogAction($"Se eliminó a la persona con ID: {person.Id}");
    }
    else
    {
        Console.WriteLine("Operación cancelada.");
    }
}
void UnlockUser()
{
    Console.WriteLine("\n--- Desbloquear Usuario ---");
    Console.Write("Ingrese el nombre del usuario a desbloquear: ");
    string userToUnlock = Console.ReadLine()!;

    if (string.IsNullOrWhiteSpace(userToUnlock))
    {
        Console.WriteLine("El nombre no puede estar vacío.");
        return;
    }

    var lines = userFileHelper.ReadAllLines();
    var outputLines = new List<string>();
    bool found = false;

    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line)) continue;

        var parts = line.Split(',');

        if (parts[0].Trim().Equals(userToUnlock.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            outputLines.Add($"{parts[0]},{parts[1]},true");
            found = true;
        }
        else
        {
            outputLines.Add(line);
        }
    }

    if (found)
    {
        userFileHelper.WriteAllLines(outputLines);
        Console.WriteLine($"\n¡Éxito! El usuario '{userToUnlock}' ha sido desbloqueado y ya puede ingresar.");

        LogAction($"Se desbloqueó al usuario de sistema: {userToUnlock}");
    }
    else
    {
        Console.WriteLine($"\nNo se encontró ningún usuario con el nombre '{userToUnlock}'.");
    }
}
void ShowReportByCity()
{
    var people = helper.Read(listName).OrderBy(p => p.City).ToList();

    if (!people.Any())
    {
        Console.WriteLine("No hay datos para generar el reporte.");
        return;
    }

    Console.Clear();
    Console.WriteLine("========================================================");
    Console.WriteLine("          INFORME DE SALDOS POR CIUDAD");
    Console.WriteLine("========================================================");

    Console.WriteLine($"{"CIUDAD",-20} {"NOMBRE",-25} {"SALDO",15}");
    Console.WriteLine(new string('-', 62));

    string currentCity = "";
    decimal cityTotal = 0;
    decimal grandTotal = 0;
    bool isFirst = true;

    foreach (var person in people)
    {
        if (isFirst)
        {
            currentCity = person.City;
            isFirst = false;
        }

        if (person.City != currentCity)
        {
            Console.WriteLine(new string('-', 62));
            Console.WriteLine($" TOTAL {currentCity.ToUpper()}: {cityTotal,45:C2}");
            Console.WriteLine(new string('=', 62));
            Console.WriteLine();

            currentCity = person.City;
            cityTotal = 0;
        }

        cityTotal += person.Balance;
        grandTotal += person.Balance;

        Console.WriteLine($"{person.City,-20} {person.Name,-25} {person.Balance,15:C2}");
    }

    Console.WriteLine(new string('-', 62));
    Console.WriteLine($" TOTAL {currentCity.ToUpper()}: {cityTotal,45:C2}");
    Console.WriteLine(new string('=', 62));

    Console.WriteLine($"\n *** GRAN TOTAL GENERAL: {grandTotal:C2} ***");

    LogAction("Se generó el informe de saldos agrupado por ciudad.");

    Console.WriteLine("\nPresione cualquier tecla para volver al menú...");
    Console.ReadKey();
}