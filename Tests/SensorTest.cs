using System;
using System.Threading;

using Moq;

using Xunit;

namespace IndustrialControlService.Components.Test
{
    public class SensorTests : IDisposable
    {
        private readonly PressureSensor pressureSensor;
        private readonly TemperatureSensor temperatureSensor;
        private readonly CancellationTokenSource source;

        public SensorTests()
        {
            pressureSensor = new PressureSensor(0, 10, 1000);
            temperatureSensor = new TemperatureSensor(-50, 50, 1000);
            source = new CancellationTokenSource();
        }

        [Fact]
        public void SetpointWithinSafetyRange()
        {
            double setpointValue = 7.5;
            pressureSensor.Setpoint = setpointValue;

            Assert.Equal(setpointValue, pressureSensor.Setpoint);
        }

        [Fact]
        public void SetpointOutOfRangeThrowsException()
        {
            double setpointValue = 15;

            Assert.Throws<ArgumentOutOfRangeException>(() => pressureSensor.Setpoint = setpointValue);
        }

        [Fact]
        public void UpdateValue()
        {
            double initialValue = 0;
            double setpointValue = 5;
            double expectedValue = initialValue + (setpointValue - initialValue) * 0.1;

            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), source.Token)
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"temperature\":22 }")
                });

            temperatureSensor.Setpoint = setpointValue;

            Thread.Sleep(10000);

            Assert.Equal(expectedValue, temperatureSensor.Value, 1);
        }

        public void Dispose()
        {
            source?.Cancel();
        }
    }
}

public class SensorTests
{
    [Theory]
    [InlineData(-50, 50, 500)]
    [InlineData(0, 100, 1000)]
    public void Setpoint_SetWithinSafetyRange(double safetyMin, double safetyMax, int scanRate)
    {
        //Arrange
        var mockTimer = new Mock<Timer>(null);
        var sensor = new Mock<Sensor>(safetyMin, safetyMax, scanRate) { CallBase = true };
        sensor.Setup(s => s.UpdateValue());
        sensor.Setup(s => s.Dispose());

        //Act
        double expected = 50;
        sensor.Object.Setpoint = expected;
        double actual = sensor.Object.Setpoint;

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(-50, 50, 500, -100)]
    [InlineData(0, 100, 1000, 150)]
    public void Setpoint_SetOutsideSafetyRange(double safetyMin, double safetyMax, int scanRate, double value)
    {
        //Arrange
        var mockTimer = new Mock<Timer>(null);
        var sensor = new Mock<Sensor>(safetyMin, safetyMax, scanRate) { CallBase = true };
        sensor.Setup(s => s.UpdateValue());
        sensor.Setup(s => s.Dispose());

        //Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => sensor.Object.Setpoint = value);
    }

    [Fact]
    public void UpdateValue()
    {
        //Arrange
        var mockTimer = new Mock<Timer>(null);
        var sensor = new Mock<Sensor>(50, 100, 1000) { CallBase = true };
        sensor.Setup(s => s.Dispose());

        //Act
        double expected = 70;
        sensor.Object.Setpoint = 70;
        sensor.Object.UpdateValue();
        double actual = sensor.Object.GetPrivateField<double>("value");

        //Assert
        Assert.Equal(expected, actual, 3);
    }
}

