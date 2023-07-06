using Smartwyre.DeveloperTest.Services;
using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        RebateService rebateService = new RebateService(new RebateDataStore(), new ProductDataStore());

        Console.Write("Enter rebate identifier: ");
        var rebateIdentifier = Console.ReadLine();
        Console.Write("Enter product identifier: ");
        var productIdentifier = Console.ReadLine();
        Console.Write("Enter volume: ");
        var volume = Decimal.Parse(Console.ReadLine());

        CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            { RebateIdentifier = rebateIdentifier, ProductIdentifier = productIdentifier, Volume = volume }
        );

        Console.WriteLine($"Result: {result.Success}");
    }
}
