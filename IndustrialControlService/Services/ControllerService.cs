using Grpc.Core;
using GrpcPressureTemperatureService ;

namespace IndustrialControlService.Services;

public class ControllerService : PressureTemperatureController.PressureTemperatureControllerBase
{
    //TODO remove state from grpc controller service
    private double pressure;
    private double temperature;

    private readonly ILogger<ControllerService> _logger;
    private readonly LoopService _lp;

    public ControllerService(ILogger<ControllerService> logger, LoopService lp)
    {
        _logger = logger;
        _lp = lp;
    }

    public override Task<StatusResponse> Start(StartRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Controller Service started.");
        _lp.Start();

        return Task.FromResult(new StatusResponse
        {
            Success = true
        });
    }

    public override Task<StatusResponse> Stop(StopRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Controller Service started.");

        return Task.FromResult(new StatusResponse
        {
            Success = true
        });
    }

    public override Task<PressureTemperatureResponse> GetPressureTemperature(PressureTemperatureRequest request, ServerCallContext context)
    {
        return Task.FromResult(new PressureTemperatureResponse
        {
            RequestId = request.RequestId,
            Pressure = this.pressure,
            Temperature = this.temperature
        });
    }

    public override Task<PressureTemperatureSetResponse> SetPressure(SetRequest request, ServerCallContext context)
    {
        this.pressure = request.Value;

        return Task.FromResult(new PressureTemperatureSetResponse
        {
            RequestId = request.RequestId,
            Success = true
        });
    }

    public override Task<PressureTemperatureSetResponse> SetTemperature(SetRequest request, ServerCallContext context)
    {
        this.temperature = request.Value;

        return Task.FromResult(new PressureTemperatureSetResponse
        {
            RequestId = request.RequestId,
            Success = true
        });
    }

    public override async Task StreamPressureTemperature(StreamRequest request, IServerStreamWriter<PressureTemperatureResponse> responseStream, ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new PressureTemperatureResponse
            {
                RequestId = request.RequestId,
                Pressure = this.pressure,
                Temperature = this.temperature
            });

            await Task.Delay(10);  //  10 millis
        }
    }
}