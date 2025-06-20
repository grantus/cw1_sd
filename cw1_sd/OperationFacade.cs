namespace cw1_sd;

public class OperationFacade
{
    private static int _nextId;
    public static readonly Dictionary<int, Operation> OperationRepository = new();

    public static Dictionary<int, Operation> Repository => OperationRepository;

    public static void LoadFromList(List<Operation> operations)
    {
        OperationRepository.Clear();
        _nextId = 0;

        foreach (var op in operations)
        {
            if (op.Id >= _nextId)
                _nextId = op.Id + 1;
            OperationRepository[op.Id] = op;
        }
    }

    public List<Operation> ToList()
    {
        return OperationRepository.Values.ToList();
    }

    public void CreateOperation(int bankAccountId, int amount, Operation.OperationType type, int categoryId,
        DateTime? date = null, string? description = null)
    {
        date ??= DateTime.Now;
        if (amount < 0)
        {
            throw new ArgumentException("Operation amount cannot be negative.");
        }

        var operation = new Operation
        {
            Id = _nextId++,
            BankAccountId = bankAccountId,
            Amount = amount,
            Type = type,
            Date = date.Value,
            CategoryId = categoryId,
            Description = description
        };

        OperationRepository[operation.Id] = operation;
    }

    public void DeleteOperation(int id)
    {
        OperationRepository.Remove(id);
    }

    private Operation? GetOperation(int id)
    {
        if (!OperationRepository.ContainsKey(id))
        {
            return null;
        }
        return OperationRepository[id];
    }

    public int EditOperation(int id, int? amount = null, Operation.OperationType? type = null,
        int? bankAccountId = null, DateTime? date = null, string? description = null, int? categoryId = null)
    {
        var operation = GetOperation(id);
        if (operation == null)
        {
            return 1;
        }

        if (type != null) operation.Type = type.Value;
        if (amount != null)
        {
            if (amount < 0) throw new ArgumentException("Operation amount cannot be negative.");
            operation.Amount = amount.Value;
        }

        if (bankAccountId != null)
        {
            operation.BankAccountId = bankAccountId.Value;
        }

        if (date != null)
        {
            operation.Date = date.Value;
        }

        if (description != null)
        {
            operation.Description = description;
        }

        if (categoryId != null)
        {
            operation.CategoryId = categoryId.Value;
        }

        return 0;
    }
}

public class Operation
{
    public int Id { get; init; }
    public OperationType Type { get; set; }
    public int BankAccountId { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }

    public enum OperationType
    {
        Income,
        Expense
    }
}