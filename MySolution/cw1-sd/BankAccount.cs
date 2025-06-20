namespace cw1_sd;

public class BankAccountFacade
{
    private static int _nextId;
    private static readonly Dictionary<int, BankAccount> BankAccountRepository = new();

    public static void LoadFromList(List<BankAccount> bankAccounts)
    {
        BankAccountRepository.Clear();
        _nextId = 0;

        foreach (var acc in bankAccounts)
        {
            if (acc.Id >= _nextId)
                _nextId = acc.Id + 1;

            BankAccountRepository[acc.Id] = acc;
        }
    }

    public List<BankAccount> ToList()
    {
        return BankAccountRepository.Values.ToList();
    }

    public void CreateBankAccount(string name, int balance = 0)
    {
        var account = new BankAccount { Id = _nextId++, Name = name, Balance = balance };
        BankAccountRepository[account.Id] = account;
    }

    public void DeleteBankAccount(int id)
    {
        BankAccountRepository.Remove(id);
    }

    private BankAccount GetBankAccount(int id)
    {
        return BankAccountRepository[id];
    }

    public void EditBankAccount(int id, string? name = null, int? balance = null)
    {
        var bankAccount = GetBankAccount(id);
        if (name != null)
        {
            bankAccount.Name = name;
        }

        if (balance != null)
        {
            bankAccount.Balance = balance.Value;
        }
    }

    public int AdjustBalance(int id, int delta)
    {
        if (!BankAccountRepository.ContainsKey(id))
        {
            return 1;
        }
        var bankAccount = GetBankAccount(id);
        bankAccount.Balance += delta;
        return 0;
    }
}

public class BankAccount
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public int Balance { get; set; }
}