using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace cw1_sd;

public class FinanceData
{
    public List<BankAccount> BankAccounts { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
    public List<Operation> Operations { get; set; } = new();
}
