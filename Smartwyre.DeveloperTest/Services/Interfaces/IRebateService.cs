using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services.Interfaces;

public interface IRebateService
{
    CalculateRebateResult Calculate(CalculateRebateRequest request);
}
