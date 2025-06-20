namespace cw1_sd;

//фасад
public static class AnalysisFacade
{
    public static decimal CalculateIncomeExpenseDifference(Dictionary<int, Operation> operations,
        DateTime startDate,
        DateTime endDate)
    {
        decimal totalIncome = 0;
        decimal totalExpense = 0;

        foreach (var operation in operations.Where(operation =>
                     operation.Value.Date >= startDate && operation.Value.Date <= endDate))
        {
            switch (operation.Value.Type)
            {
                case Operation.OperationType.Income:
                    totalIncome += operation.Value.Amount;
                    break;
                case Operation.OperationType.Expense:
                    totalExpense += operation.Value.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return totalIncome - totalExpense;
    }

    public static Dictionary<string, decimal> GroupByCategory(Dictionary<int, Operation> operations,
        DateTime startDate,
        DateTime endDate)
    {
        var groupedData = new Dictionary<string, decimal>();

        foreach (var operation in operations)
        {
            if (operation.Value.Date < startDate || operation.Value.Date > endDate)
            {
                continue;
            }

            var categoryName = CategoryFacade.GetCategory(operation.Value.CategoryId)?.Name;

            if (categoryName != null)
            {
                groupedData.TryAdd(categoryName, 0);
            }

            switch (operation.Value.Type)
            {
                case Operation.OperationType.Income:
                {
                    if (categoryName != null) groupedData[categoryName] += operation.Value.Amount;
                    break;
                }
                case Operation.OperationType.Expense:
                {
                    if (categoryName != null)
                        groupedData[categoryName] -= operation.Value.Amount;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return groupedData;
    }
}