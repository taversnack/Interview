using System;
using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services.Interfaces;
using Smartwyre.DeveloperTest.Types;
using System.Linq;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private Rebate _rebate = null;
    private Product _product = null;
    private IRebateDataStore _rebateDataStore;
    private IProductDataStore _productDataStore;
    private CalculateRebateResult _result = new CalculateRebateResult();

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore)
    {
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        _rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        _product = _productDataStore.GetProduct(request.ProductIdentifier);

        try
        {
            var rebateAmount = calculateRebateAmountByIncentive(request);
            _result.Success = true;
            _rebateDataStore.StoreCalculationResult(_rebate, rebateAmount);
        }
        catch (Exception e)
        {
            _result.Success = false;
        }

        return _result;
    }

    // expressionValues is the array of values which are used to calcuate rebateAmount
    private void validateRebate(SupportedIncentiveType supportedIncentiveType, decimal[] expressionValues)
    {
        string exeptionMessage = "";
        if (_product == null)
        {
            exeptionMessage = "Product is not present";
        }
        else if (!_product.SupportedIncentives.HasFlag(supportedIncentiveType))
        {
            exeptionMessage = "Product does not support the incentive type";
        }
        else if (expressionValues.Any((expressionValue) => expressionValue == 0))
        {
            exeptionMessage = "One or more expression values for rebate amount is 0";
        }

        if (!string.IsNullOrEmpty(exeptionMessage)) throw new Exception(exeptionMessage);
    }

    private decimal calculateRebateAmountByIncentive(CalculateRebateRequest request)
    {
        switch (_rebate.Incentive)
        {

            case IncentiveType.FixedCashAmount:
                validateRebate(
                    SupportedIncentiveType.FixedCashAmount,
                    new[] { _rebate.Amount }
                );
                return (_rebate.Amount);

            case IncentiveType.FixedRateRebate:
                validateRebate(
                    SupportedIncentiveType.FixedRateRebate,
                    new[] { _product.Price, _rebate.Percentage, request.Volume }
                );
                return (_product.Price * _rebate.Percentage * request.Volume);

            case IncentiveType.AmountPerUom:
                validateRebate(
                    SupportedIncentiveType.AmountPerUom,
                    new[] { _rebate.Amount, request.Volume }
                );
                return (_rebate.Amount * request.Volume);

            default:
                return 0m;
        }
    }

}
