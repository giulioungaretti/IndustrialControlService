using IndustrialControlService.Components;

namespace Tests;

public class SensorTests : IDisposable
{
    private readonly PressureSensor pressureSensor = new(0, 10, 1000);
    private readonly TemperatureSensor temperatureSensor = new(-50, 50, 1000);
    private readonly CancellationTokenSource source = new();

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
        double setpointValue = 5;

        temperatureSensor.Setpoint = setpointValue;
        // test non linear response 
        Assert.NotEqual(setpointValue, temperatureSensor.Value, 1);
        // let it settle
        Thread.Sleep(1000);
        // must have reached the setpoint
        Assert.Equal(setpointValue, temperatureSensor.Value, 1);
    }

    public void Dispose()
    {
        source?.Cancel();
    }
};