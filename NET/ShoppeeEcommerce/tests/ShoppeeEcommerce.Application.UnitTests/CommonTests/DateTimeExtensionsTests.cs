using ShoppeeEcommerce.Application.Utilities;

namespace ShoppeeEcommerce.Application.Tests.CommonTests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void StartOfDay_ShouldReturnDateTimeWithZeroTime_WhenCalled()
        {
            // Arrange
            var date = new DateTime(2023, 12, 25, 14, 30, 45); // Mid-day
            var expected = new DateTime(2023, 12, 25, 0, 0, 0);

            // Act
            var result = date.StartOfDay();

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(0, result.Hour);
            Assert.Equal(0, result.Minute);
            Assert.Equal(0, result.Second);
        }

        [Fact]
        public void EndOfDay_ShouldReturnLastTickOfDay_WhenCalled()
        {
            // Arrange
            var date = new DateTime(2023, 12, 25, 10, 0, 0);

            // Expected is 23:59:59.999...
            var expected = new DateTime(2023, 12, 25, 23, 59, 59).AddTicks(TimeSpan.TicksPerSecond - 1);

            // Act
            var result = date.EndOfDay();

            // Assert
            Assert.Equal(expected, result);
            Assert.Equal(23, result.Hour);
            Assert.Equal(59, result.Minute);
            Assert.Equal(59, result.Second);
            // Adding one tick should move it to the exact start of the next day
            Assert.Equal(date.Date.AddDays(1), result.AddTicks(1));
        }

        [Theory]
        [InlineData(0, 0, 0)]      // Already start of day
        [InlineData(23, 59, 59)]   // Very end of day
        public void StartOfDay_ShouldAlwaysReturnSameDate_RegardlessOfTime(int h, int m, int s)
        {
            // Arrange
            var date = new DateTime(2023, 1, 1, h, m, s);
            var expected = new DateTime(2023, 1, 1, 0, 0, 0);

            // Act
            var result = date.StartOfDay();

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
