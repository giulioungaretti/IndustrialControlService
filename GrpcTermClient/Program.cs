using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcPressureTemperatureService;

using var channel = GrpcChannel.ForAddress("https://localhost:7219");


var client = new PressureTemperatureController.PressureTemperatureControllerClient(channel);
var req = new StartRequest();
var reply = client.Start(req);
Console.WriteLine("Greeting: " + reply.Success);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
