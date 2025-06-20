using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using cw1_sd;

namespace FinanceTests;

public class CoreTests
{
    [Fact]
    public void BankAccountFacade_CRUD()
    {
        var fac = new BankAccountFacade();
        fac.CreateBankAccount("Cash", 500);
        var acc = fac.ToList().Single();
        Assert.Equal(500, acc.Balance);

        fac.AdjustBalance(acc.Id, 250);
        Assert.Equal(750, fac.ToList().Single().Balance);

        fac.DeleteBankAccount(acc.Id);
        Assert.Empty(fac.ToList());
    }

    [Fact]
    public void CategoryFacade_CRUD()
    {
        var fac = new CategoryFacade();
        fac.CreateCategory("Food", CategoryType.Expense);
        var cat = CategoryFacade.ToList().Single();

        CategoryFacade.EditCategory(cat.Id, "Groceries", CategoryType.Expense);
        Assert.Equal("Groceries", CategoryFacade.GetCategory(cat.Id).Name);

        CategoryFacade.DeleteCategory(cat.Id);
        Assert.Empty(CategoryFacade.ToList());
    }

    [Fact]
    public void OperationFacade_CRUD()
    {
        var catFac = new CategoryFacade();
        catFac.CreateCategory("Salary", CategoryType.Income);
        int catId = CategoryFacade.ToList().Single().Id;

        var opFac = new OperationFacade();
        opFac.CreateOperation(1, 1000, Operation.OperationType.Income, catId);
        var op = OperationFacade.Repository.Values.Single();

        opFac.EditOperation(op.Id, 1200, null, null, op.Date.AddDays(1), "Bonus", catId);
        Assert.Equal(1200, OperationFacade.Repository[op.Id].Amount);

        opFac.DeleteOperation(op.Id);
        Assert.Empty(OperationFacade.Repository);
    }
}
