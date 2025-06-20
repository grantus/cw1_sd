using Newtonsoft.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace cw1_sd
{
    public static class ImportExport
    {
        public static class CsvDataImporter
        {
            public static void ImportData(string fileName, ref FinanceData financeData)
            {
                try
                {
                    var newAccounts = new List<BankAccount>();
                    if (File.Exists("accounts.csv"))
                    {
                        var lines = File.ReadAllLines("accounts.csv");
                        newAccounts.AddRange(from line in lines.Skip(1)
                            where !string.IsNullOrWhiteSpace(line)
                            select line.Split(',')
                            into parts
                            where parts.Length >= 3
                            select new BankAccount
                                { Id = int.Parse(parts[0]), Name = parts[1], Balance = int.Parse(parts[2]) });

                        Console.WriteLine($"accounts.csv импортирован ({newAccounts.Count} счетов).");
                    }
                    else
                    {
                        Console.WriteLine("accounts.csv не найден.");
                    }

                    var newCategories = new List<Category>();
                    if (File.Exists("categories.csv"))
                    {
                        var lines = File.ReadAllLines("categories.csv");

                        newCategories.AddRange(from line in lines.Skip(1)
                            where !string.IsNullOrWhiteSpace(line)
                            select line.Split(',')
                            into parts
                            where parts.Length >= 3
                            let parsedType = Enum.Parse<CategoryType>(parts[2])
                            select new Category
                                { Id = int.Parse(parts[0]), Name = parts[1], Type = parsedType });

                        Console.WriteLine($"categories.csv импортирован ({newCategories.Count} категорий).");
                    }
                    else
                    {
                        Console.WriteLine("categories.csv не найден.");
                    }

                    var newOperations = new List<Operation>();
                    if (File.Exists("operations.csv"))
                    {
                        var lines = File.ReadAllLines("operations.csv");
                        newOperations.AddRange(from line in lines.Skip(1)
                            where !string.IsNullOrWhiteSpace(line)
                            select line.Split(',')
                            into parts
                            where parts.Length >= 7
                            let parsedType = Enum.Parse<Operation.OperationType>(parts[1])
                            select new Operation
                            {
                                Id = int.Parse(parts[0]),
                                Type = parsedType,
                                BankAccountId = int.Parse(parts[2]),
                                Amount = int.Parse(parts[3]),
                                Date = DateTime.Parse(parts[4]),
                                Description = parts[5],
                                CategoryId = int.Parse(parts[6])
                            });

                        Console.WriteLine($"operations.csv импортирован ({newOperations.Count} операций).");
                    }
                    else
                    {
                        Console.WriteLine("operations.csv не найден.");
                    }

                    financeData.BankAccounts = newAccounts;
                    financeData.Categories = newCategories;
                    financeData.Operations = newOperations;

                    BankAccountFacade.LoadFromList(newAccounts);
                    CategoryFacade.LoadFromList(newCategories);
                    OperationFacade.LoadFromList(newOperations);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка иморта в CSV: {ex.Message}");
                    throw;
                }
            }
        }

        public static class JsonDataImporter
        {
            public static void ImportData(string fileName, ref FinanceData financeData)
            {

                try
                {
                    var jsonText = File.ReadAllText(fileName);
                    var loaded = JsonConvert.DeserializeObject<FinanceData>(jsonText);
                    if (loaded == null)
                    {
                        Console.WriteLine("Не удалось десериализовать данные из JSON.");
                        return;
                    }
                    financeData = loaded;
                    
                    BankAccountFacade.LoadFromList(financeData.BankAccounts);
                    CategoryFacade.LoadFromList(financeData.Categories);
                    OperationFacade.LoadFromList(financeData.Operations);

                    Console.WriteLine($"Импорт из JSON {fileName} завершён.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка импорта из JSON: {ex.Message}");
                    throw;
                }
            }
        }

        public static class YamlDataImporter
        {
            public static void ImportData(string fileName, ref FinanceData financeData)
            {
                try
                {
                    var yamlText = File.ReadAllText(fileName);

                    var deserializer = new DeserializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var loaded = deserializer.Deserialize<FinanceData>(yamlText);

                    financeData = loaded;
                    BankAccountFacade.LoadFromList(financeData.BankAccounts);
                    CategoryFacade.LoadFromList(financeData.Categories);
                    OperationFacade.LoadFromList(financeData.Operations);

                    Console.WriteLine("Импорт из YAML завершён.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка импорта из YAML: {ex.Message}");
                    throw;
                }
            }
        }

        public static class CsvDataExporter
        {
            public static void ExportData(string fileName, ref FinanceData financeData)
            {
                
                var accounts = financeData.BankAccounts.ToList();
                var accLines = new List<string> { "Id,Name,Balance" };

                accLines.AddRange(accounts.Select(a => $"{a.Id},{a.Name},{a.Balance}"));
                try
                {
                    var csvData = string.Join(Environment.NewLine, accLines);
                    File.WriteAllText(fileName, csvData);
                    Console.WriteLine($"Data exported to CSV: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка экспорта в CSV: {ex.Message}");
                    throw;
                }
                
                Console.WriteLine("accounts.csv экспортирован.");

                var categories = financeData.Categories.ToList();
                var catLines = new List<string> { "Id,Name,Type" };
                catLines.AddRange(categories.Select(category => $"{category.Id},{category.Name},{category.Type}"));
                File.WriteAllLines("categories.csv", catLines);
                Console.WriteLine("categories.csv экспортирован.");

                var operations = financeData.Operations.ToList();
                var opLines = new List<string> { "Id,Type,BankAccountId,Amount,Date,Description,CategoryId" };
                opLines.AddRange(operations.Select(o =>
                    $"{o.Id},{o.Type},{o.BankAccountId},{o.Amount},{o.Date:yyyy-MM-dd},{o.Description},{o.CategoryId}"));
                File.WriteAllLines("operations.csv", opLines);
                Console.WriteLine("operations.csv экспортирован.");
                
            }
        }

        public static class JsonDataExporter
        {
            public static void ExportData(string fileName, ref FinanceData financeData)
            {
                try
                {
                    var jsonData = JsonConvert.SerializeObject(financeData, Formatting.Indented);
                    File.WriteAllText(fileName, jsonData);
                    Console.WriteLine($"Данные exported to JSON: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка экспорта в JSON: {ex.Message}");
                    throw;
                }
            }
        }

        public static class YamlDataExporter
        {
            public static void ExportData(string fileName, ref FinanceData financeData)
            {
                try
                {
                    var serializer = new SerializerBuilder()
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var yamlText = serializer.Serialize(financeData);
                    File.WriteAllText("finance.yaml", yamlText);

                    Console.WriteLine("Данные экспортированы в finance.yaml (YAML).");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка экспорта в YAML: {ex.Message}");
                    throw;
                }
            }
        }
    }
}