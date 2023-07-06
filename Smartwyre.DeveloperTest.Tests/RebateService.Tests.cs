using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;
using Moq;

namespace Smartwyre.DeveloperTest.Tests
{
    public class RebateServiceTests
    {
        [Fact]
        public void Calculate_ShouldFail_WithNoRebate()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1")).Returns((Rebate)null);
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1")).Returns(new Product());

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
                {
                    ProductIdentifier = "1",
                    RebateIdentifier = "1",
                    Volume = 10
                }
            );

            // ASSERT
            Assert.False(result.Success);
        }


        [Fact]
        public void Calculate_ShouldFail_WithNoProduct()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1")).Returns(new Rebate());
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1")).Returns((Product)null);

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
                {
                    ProductIdentifier = "1",
                    RebateIdentifier = "1",
                    Volume = 10
                }
            );

            // ASSERT
            Assert.False(result.Success);
        }

        [Fact]
        public void Calculate_ShouldFail_NotSupportedIncentiveType()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1"))
                                 .Returns(getMockedRebate(IncentiveType.FixedRateRebate));
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1"))
                                  .Returns(getMockedProduct(SupportedIncentiveType.FixedCashAmount));

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = "1",
                RebateIdentifier = "1",
                Volume = 10
            }
            );

            // ASSERT
            Assert.False(result.Success);
        }

        [Fact]
        public void Calculate_ShouldFail_ZeroRequestVolume()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1"))
                                 .Returns(getMockedRebate(IncentiveType.FixedRateRebate));
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1"))
                                  .Returns(getMockedProduct(SupportedIncentiveType.FixedRateRebate));

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = "1",
                RebateIdentifier = "1",
                Volume = 0
            }
            );

            // ASSERT
            Assert.False(result.Success);
        }

        [Fact]
        public void Calculate_ShouldSucceed_ForFixedRateRebate()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1"))
                                 .Returns(getMockedRebate(IncentiveType.FixedRateRebate));
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1"))
                                  .Returns(getMockedProduct(SupportedIncentiveType.FixedRateRebate));

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = "1",
                RebateIdentifier = "1",
                Volume = 10
            }
            );

            // ASSERT
            Assert.True(result.Success);
        }

        [Fact]
        public void Calculate_ShouldSucceed_ForAmountPerUom()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1"))
                                 .Returns(getMockedRebate(IncentiveType.AmountPerUom));
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1"))
                                  .Returns(getMockedProduct(SupportedIncentiveType.AmountPerUom));

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = "1",
                RebateIdentifier = "1",
                Volume = 10
            }
            );

            // ASSERT
            Assert.True(result.Success);
        }

        [Fact]
        public void Calculate_ShouldSucceed_ForFixedCashAmount()
        {
            // MOCKED
            var mockedRebateDataStore = new Mock<IRebateDataStore>();
            mockedRebateDataStore.Setup(r => r.GetRebate("1"))
                                 .Returns(getMockedRebate(IncentiveType.FixedCashAmount));
            var mockedProductDataStore = new Mock<IProductDataStore>();
            mockedProductDataStore.Setup(p => p.GetProduct("1"))
                                  .Returns(getMockedProduct(SupportedIncentiveType.FixedCashAmount));

            // ARRANGE
            var rebateService = new RebateService(mockedRebateDataStore.Object, mockedProductDataStore.Object);

            // ACTION
            CalculateRebateResult result = rebateService.Calculate(new CalculateRebateRequest()
            {
                ProductIdentifier = "1",
                RebateIdentifier = "1",
                Volume = 10
            }
            );

            // ASSERT
            Assert.True(result.Success);
        }


        #region Private Methods

        private Product getMockedProduct(SupportedIncentiveType supportedIncentiveType)
        {
            return new Product()
            {
                Id = 1,
                Identifier = "1",
                Price = 100,
                SupportedIncentives = supportedIncentiveType
            };
        }

        private Rebate getMockedRebate(IncentiveType incentiveType)
        {
            return new Rebate()
            {
                Identifier = "1",
                Amount = 10,
                Incentive = incentiveType,
                Percentage = 50
            };
        }

        #endregion
    }
}
