using Newtonsoft.Json;

namespace cw1_sd
{
    public static class Program
    {
        private static string? _jsonPath;

        private static readonly BankAccountFacade BankAccountFacade = new();
        private static readonly CategoryFacade CategoryFacade = new();
        private static readonly OperationFacade OperationFacade = new();

        private static FinanceData _financeData = new();

        public static void Main()
        {
            LoadData();

            var exit = false;
            while (!exit)
            {
                var choice = PrintMenu();
                switch (choice)
                {
                    case "1":
                        ShowIncomeExpenseDifference();
                        break;
                    case "2":
                        GroupIncomeExpenseByCategory();
                        break;
                    case "3":
                        ManageBankAccounts();
                        break;
                    case "4":
                        ManageCategories();
                        break;
                    case "5":
                        ManageOperations();
                        break;
                    case "6":
                        ExportDataToCsv();
                        break;
                    case "7":
                        ExportDataToYaml();
                        break;
                    case "8":
                        ExportDataToJson();
                        break;
                    case "9":
                        ImportDataFromCsv();
                        break;
                    case "10":
                        ImportDataFromYaml();
                        break;
                    case "11":
                        ImportDataFromJson();
                        break;
                    case "12":
                        RecalculateBalance();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод. Повторите попытку.");
                        break;
                }
            }

            SaveData();
            Console.WriteLine("Завершение программы...");
        }

        private static string PrintMenu()
        {
            Console.WriteLine("\nМеню:");
            Console.WriteLine("1) Показать разницу доходов и расходов за выбранный период");
            Console.WriteLine("2) Группировать доходы и расходы по категориям");
            Console.WriteLine("3) Действие над счетами");
            Console.WriteLine("4) Действие над категориями");
            Console.WriteLine("5) Действие над операциями");
            Console.WriteLine("6) Экспортировать данные в CSV");
            Console.WriteLine("7) Экспортировать данные в YAML");
            Console.WriteLine("8) Экспортировать данные в JSON");
            Console.WriteLine("9) Импортировать данные из CSV");
            Console.WriteLine("10) Импортировать данные из YAML");
            Console.WriteLine("11) Импортировать данные из JSON");
            Console.WriteLine("12) Пересчет баланса");
            Console.WriteLine("0) Выход");
            Console.Write("Ваш выбор: ");
            return Console.ReadLine() ?? "";
        }

        private static string PrintActionMenu(string objectName)
        {
            Console.WriteLine($"\nДействия над {objectName}:");
            Console.WriteLine("1) Создать");
            Console.WriteLine("2) Редактировать");
            Console.WriteLine("3) Удалить");
            Console.WriteLine("0) Назад");
            Console.Write("Ваш выбор: ");
            return Console.ReadLine() ?? "";
        }

        private static void LoadData()
        {
            Console.Write("Введите путь к JSON-файлу (либо Enter для \"finance_export.json\"): ");
            var userInput = Console.ReadLine();
            _jsonPath = string.IsNullOrWhiteSpace(userInput) ? "finance_export.json" : userInput.Trim();

            if (!File.Exists(_jsonPath))
            {
                Console.WriteLine($"Файл {_jsonPath} не найден. При сохранении будет создан новый JSON.");
                return;
            }

            try
            {
                var jsonText = File.ReadAllText(_jsonPath);
                var loaded = JsonConvert.DeserializeObject<FinanceData>(jsonText);
                if (loaded == null)
                {
                    Console.WriteLine("Ошибка при чтении JSON. Загружаем пустые данные.");
                    return;
                }

                _financeData = loaded;

                BankAccountFacade.LoadFromList(_financeData.BankAccounts);
                CategoryFacade.LoadFromList(_financeData.Categories);
                OperationFacade.LoadFromList(_financeData.Operations);

                Console.WriteLine("Данные загружены из JSON.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки JSON: {ex.Message}");
            }
        }

        private static void SaveData()
        {
            _financeData.BankAccounts = BankAccountFacade.ToList();
            _financeData.Categories = CategoryFacade.ToList();
            _financeData.Operations = OperationFacade.ToList();

            try
            {
                var jsonText = JsonConvert.SerializeObject(_financeData, Formatting.Indented);
                File.WriteAllText(_jsonPath!, jsonText);
                Console.WriteLine($"Данные сохранены в {_jsonPath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения JSON: {ex.Message}");
            }
        }

        private static DateTime InputDateTime(bool lower)
        {
            var input = Console.ReadLine();

            while (true)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return lower ? DateTime.MinValue : DateTime.MaxValue;
                }

                if (DateTime.TryParse(input, out var date))
                {
                    return date;
                }

                Console.WriteLine("Неверный ввод, введите еще раз, пожалуйста: ");
            }
        }

        private static void ShowIncomeExpenseDifference()
        {
            Console.WriteLine("\nВведите дату начала (dd.MM.yyyy) (Enter = за все время): ");
            var startDate = InputDateTime(true);

            Console.WriteLine("Введите дату окончания (dd.MM.yyyy) (Enter = за все время): ");
            var endDate = InputDateTime(false);

            var result = IAnalysis.CalculateIncomeExpenseDifference(
                OperationFacade.OperationRepository, startDate, endDate);
            Console.WriteLine($"Разница доходов и расходов: {result}");
        }

        private static void GroupIncomeExpenseByCategory()
        {
            Console.WriteLine("\nВведите дату начала (dd.MM.yyyy) (Enter = за все время): ");
            var startDate = InputDateTime(true);

            Console.WriteLine("Введите дату окончания (dd.MM.yyyy) (Enter = за все время): ");
            var endDate = InputDateTime(false);
            
            var grouped = IAnalysis.GroupByCategory(OperationFacade.OperationRepository, startDate, endDate);
            Console.WriteLine("Группировка:");
            foreach (var item in grouped)
            {
                Console.WriteLine($"{item.Key} => {item.Value}");
            }
        }

        private static void ManageBankAccounts()
        {
            
            var choice = PrintActionMenu("счетом");
            switch (choice)
            {
                case "1":
                    CreateBankAccount();
                    break;
                case "2":
                    EditBankAccount();
                    break;
                case "3":
                    RemoveBankAccount();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }
        }

        private static void CreateBankAccount()
        {
            Console.WriteLine("\nВведите название счета:");
            var name = Console.ReadLine() ?? "";
            Console.WriteLine("Введите начальный баланс:");
            var balance = int.Parse(Console.ReadLine() ?? "0");

            BankAccountFacade.CreateBankAccount(name, balance);
            Console.WriteLine("Счет создан.");
        }

        private static void EditBankAccount()
        {
            Console.WriteLine("\nВведите ID счета для редактирования:");
            var id = int.Parse(Console.ReadLine() ?? "0");
            if (BankAccountFacade.AdjustBalance(id, 0) == 1)
            {
               Console.WriteLine($"Не сущетсвует счета с id = {id}.");
            }
            else
            {
                Console.WriteLine("Введите новое название счета (Enter, чтобы пропустить):");
                var newName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newName)) newName = null;

                Console.WriteLine("Введите новый баланс (Enter, чтобы пропустить):");
                var balStr = Console.ReadLine();
                int? newBalance = null;
                if (int.TryParse(balStr, out var b)) newBalance = b;

                BankAccountFacade.EditBankAccount(id, newName, newBalance);
                Console.WriteLine("Счет обновлен.");
            }
        }

        private static void RemoveBankAccount()
        {
            Console.WriteLine("\nВведите ID счета для удаления:");
            var id = int.Parse(Console.ReadLine() ?? "0");
            
            if (BankAccountFacade.AdjustBalance(id, 0) == 1)
            {
                Console.WriteLine($"Не сущетсвует счет с id = {id}.");
            }

            BankAccountFacade.DeleteBankAccount(id);
            Console.WriteLine("Счет удален.");
        }

        private static void ManageCategories()
        {
            var choice = PrintActionMenu("категорией");
            switch (choice)
            {
                case "1":
                    CreateCategory();
                    break;
                case "2":
                    EditCategory();
                    break;
                case "3":
                    RemoveCategory();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }
        }

        private static void CreateCategory()
        {
            Console.WriteLine("\nВведите название категории:");
            var name = Console.ReadLine() ?? "";
            Console.WriteLine("Введите тип категории (1 - доход, 2 - расход):");
            var typeStr = Console.ReadLine() ?? "2";

            var type = typeStr.Equals("1", StringComparison.OrdinalIgnoreCase)
                ? CategoryType.Income
                : CategoryType.Expense;

            CategoryFacade.CreateCategory(name, type);
            Console.WriteLine("Категория создана.");
        }

        private static void EditCategory()
        {
            Console.WriteLine("\nВведите ID категории для редактирования:");
            
            var id = int.Parse(Console.ReadLine() ?? "0");

            if (CategoryFacade.EditCategory(id) == 1)
            {
                Console.WriteLine($"Не существует категории с id={id}");
                return;
            }
            
            Console.WriteLine("Введите новое название категории (Enter, чтобы пропустить):");
            var newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName)) newName = null;

            Console.WriteLine("Введите новый тип категории (1 - доход, 2 - расход) или Enter, чтобы пропустить:");
            var newTypeStr = Console.ReadLine();
            CategoryType? newType = null;
            if (!string.IsNullOrWhiteSpace(newTypeStr))
            {
                newType = newTypeStr.Equals("1", StringComparison.OrdinalIgnoreCase)
                    ? CategoryType.Income
                    : CategoryType.Expense;
            }

            CategoryFacade.EditCategory(id, newName, newType);
            Console.WriteLine("Категория обновлена.");
        }

        private static void RemoveCategory()
        {
            Console.WriteLine("\nВведите ID категории для удаления:");
            var id = int.Parse(Console.ReadLine() ?? "0");
            
            if (CategoryFacade.EditCategory(id) == 1)
            {
                Console.WriteLine($"Не существует категории с id={id}");
                return;
            }

            CategoryFacade.DeleteCategory(id);
            Console.WriteLine("Категория удалена.");
        }

        private static void ManageOperations()
        {
            var choice = PrintActionMenu("операцией");
            switch (choice)
            {
                case "1":
                    CreateOperation();
                    break;
                case "2":
                    EditOperation();
                    break;
                case "3":
                    RemoveOperation();
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Неверный ввод");
                    break;
            }
        }

        private static void CreateOperation()
        {
            Console.WriteLine("\nВведите ID счета для операции:");
            var bankAccountId = int.Parse(Console.ReadLine() ?? "0");
            
            if (BankAccountFacade.AdjustBalance(bankAccountId, 0) == 1)
            {
                Console.WriteLine($"Не сущетсвует счет с id = {bankAccountId}.");
            }
            
            Console.WriteLine("Введите сумму операции:");
            var amount = int.Parse(Console.ReadLine() ?? "0");
            Console.WriteLine("Введите ID категории:");
            var categoryId = int.Parse(Console.ReadLine() ?? "0");
            
            if (CategoryFacade.EditCategory(categoryId) == 1)
            {
                Console.WriteLine($"Не существует категории с id={categoryId}");
                return;
            }

            var opType = CategoryFacade.GetCategory(categoryId) is { Type: CategoryType.Income }
                ? Operation.OperationType.Income
                : Operation.OperationType.Expense;

            OperationFacade.CreateOperation(bankAccountId, amount, opType, categoryId);
            Console.WriteLine("Операция добавлена.");
        }

        private static void EditOperation()
        {
            Console.WriteLine("\nВведите ID операции для редактирования:");
            var id = int.Parse(Console.ReadLine() ?? "0");

            if (OperationFacade.EditOperation(id) == 1)
            {
                Console.WriteLine($"Не существует операции с ID = {id}");
                return;
            }
            
            Console.WriteLine("Введите новую сумму операции (Enter, чтобы пропустить):");
            var amountStr = Console.ReadLine();
            int? newAmount = null;
            if (int.TryParse(amountStr, out var am))
                newAmount = am;

            Console.WriteLine("Введите новый ID категории (Enter, чтобы пропустить):");
            var catStr = Console.ReadLine();
            var newCategoryId = -1;
            if (int.TryParse(catStr, out var cid))
                newCategoryId = cid;

            if (CategoryFacade.EditCategory(newCategoryId) == 1)
            {
                Console.WriteLine($"Не существует категории с id={newCategoryId}");
                return;
            }
            
            Operation.OperationType? newType = newCategoryId == -1
                ? null
                : CategoryFacade.GetCategory(newCategoryId) is { Type: CategoryType.Income }
                    ? Operation.OperationType.Income
                    : Operation.OperationType.Expense;
            
            OperationFacade.EditOperation(
                id,
                newAmount,
                newType,
                null,
                null,
                null,
                newCategoryId);

            Console.WriteLine("Операция обновлена.");
        }

        private static void RemoveOperation()
        {
            Console.WriteLine("\nВведите ID операции для удаления:");
            var id = int.Parse(Console.ReadLine() ?? "0");

            OperationFacade.DeleteOperation(id);
            Console.WriteLine("Операция удалена.");
        }

        private static void RecalculateBalance()
        {
            Console.WriteLine("\nВведите ID счета для пересчета:");
            var id = int.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine(BankAccountFacade.AdjustBalance(id, 0) == 0
                ? "Баланс пересчитан."
                : $"Не сущетсвует счет с id = {id}.");
        }
        
        private static string InsertFileExport(string format)
        {
            Console.Write($"Введите путь к {format.ToUpper()} (по умолчанию finance_import.{format.ToLower()}): ");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"finance_import.{format.ToLower()}";
            }
            return fileName;
        }

        private static void ExportDataToCsv()
        {
            var fileName = InsertFileExport("csv");
            
            Console.WriteLine("\nЭкспорт в CSV...");
            
            ImportExport.CsvDataExporter.ExportData(fileName, ref _financeData);

            Console.WriteLine("Экспорт в CSV завершён.");
        }

        private static void ExportDataToJson()
        {
            var fileName = InsertFileExport("json");

            Console.WriteLine("\nЭкспорт в JSON...");
            
            ImportExport.JsonDataExporter.ExportData(fileName, ref _financeData);
            
            Console.WriteLine("Экспорт в JSON завершён.");
        }

        private static void ExportDataToYaml()
        {
            var fileName = InsertFileExport("yaml");
            
            Console.WriteLine("\nЭкспорт в YAML...");

            ImportExport.YamlDataExporter.ExportData(fileName, ref _financeData);
            
            Console.WriteLine("Экспорт в YAML завершён.");
        }

        private static string InsertAndFindFile(string format)
        {
            Console.Write($"Введите путь к {format.ToUpper()} (по умолчанию finance_import.{format.ToLower()}): ");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"finance_import.{format.ToLower()}";
            }
            if (File.Exists(fileName))
            {
                return fileName;
            }
            Console.WriteLine($"Файл {fileName} не найден.");
            return "";
        }

        private static void ImportDataFromCsv()
        {
            if (InsertAndFindFile("csv") == "")
            {
                return;
            }
            Console.WriteLine("\nИмпорт из CSV...");

            ImportExport.CsvDataImporter.ImportData("finance.csv", ref _financeData);

            Console.WriteLine("Импорт из CSV завершён.");
        }

        private static void ImportDataFromYaml()
        {
            var fileName = InsertAndFindFile("yaml");
            if (fileName == "")
            {
                return;
            }
            
            Console.WriteLine("\nИмпорт из YAML...");
            
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Файл {fileName} не найден.");
                return;
            }
            ImportExport.YamlDataImporter.ImportData(fileName, ref _financeData);
            
        }

        private static void ImportDataFromJson()
        {
            var fileName = InsertAndFindFile("json");
            if (fileName == "")
            {
                return;
            }

            ImportExport.JsonDataImporter.ImportData(fileName, ref _financeData);
        }
    }
}