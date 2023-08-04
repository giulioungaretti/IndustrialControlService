namespace IndustrialControlService.Components;

public abstract class Sensor : IDisposable
{
    private double value;
    private double setpoint;
    private readonly object lockObject = new();
    private readonly Timer timer;

    public double SafetyMin { get; }
    public double SafetyMax { get; }
    public int ScanRate { get; }

    public Sensor(double safetyMin, double safetyMax, int scanRate)
    {
        SafetyMin = safetyMin;
        SafetyMax = safetyMax;
        ScanRate = scanRate;


        // Initialize the timer to call the UpdateValue method every 1 second.  
        timer = new Timer(_ => UpdateValue(), null, TimeSpan.FromMicroseconds(ScanRate), TimeSpan.FromMicroseconds(ScanRate));
        ScanRate = scanRate;
    }

    public double Setpoint
    {
        get
        {
            lock (lockObject)
            {
                return setpoint;
            }
        }
        set
        {
            lock (lockObject)
            {
                // Ensure the value stays within the safety range.  
                if (value >= SafetyMin && value <= SafetyMax)
                {
                    setpoint = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", $"Value must be between {SafetyMin} and {SafetyMax}.");
                }

            }
        }
    }


    private void UpdateValue()
    {
        lock (lockObject)
        {
            // Calculate the new value.  
            // This simulates a system with inertia, where the value gradually approaches the setpoint.  
            value += (setpoint - value) * 0.1;
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}



public class PressureSensor : Sensor
{
    public PressureSensor(double safetyMin, double safetyMax, int scanRate) : base(safetyMin, safetyMax, scanRate)
    {
    }
}

public class TemperatureSensor : Sensor
{
    public TemperatureSensor(double safetyMin, double safetyMax, int scanRate) : base(safetyMin, safetyMax, scanRate)
    {
    }
}

