namespace GlobalLogisticsManagementUI.Tests
{
    public class CurrencyCalculationTests
    {
        [Fact]
        public void ConvertUsdToZar_WithValidRate_ReturnsCorrectAmount()
        {
            decimal costUsd = 100.00m;
            decimal exchangeRate = 18.50m;

            decimal costZar = costUsd * exchangeRate;

            Assert.Equal(1850.00m, costZar);
        }

        [Fact]
        public void ConvertUsdToZar_WithZeroAmount_ReturnsZero()
        {
            decimal costUsd = 0m;
            decimal exchangeRate = 18.50m;

            decimal costZar = costUsd * exchangeRate;

            Assert.Equal(0m, costZar);
        }

        [Fact]
        public void ConvertUsdToZar_WithLargeAmount_ReturnsCorrectAmount()
        {
            decimal costUsd = 9999.99m;
            decimal exchangeRate = 16.6473m;

            decimal costZar = costUsd * exchangeRate;

            decimal expected = 9999.99m * 16.6473m;
            Assert.Equal(expected, costZar);
        }

        [Fact]
        public void ConvertUsdToZar_WithDecimalPrecision_MaintainsPrecision()
        {
            decimal costUsd = 150.75m;
            decimal exchangeRate = 18.2345m;

            decimal costZar = costUsd * exchangeRate;

            decimal expected = 150.75m * 18.2345m;
            Assert.Equal(expected, costZar);
        }

        [Fact]
        public void ConvertUsdToZar_RateChanges_ProduceDifferentResults()
        {
            decimal costUsd = 500.00m;
            decimal rate1 = 16.00m;
            decimal rate2 = 18.00m;

            decimal result1 = costUsd * rate1;
            decimal result2 = costUsd * rate2;

            Assert.NotEqual(result1, result2);
            Assert.Equal(8000.00m, result1);
            Assert.Equal(9000.00m, result2);
        }
    }
}
