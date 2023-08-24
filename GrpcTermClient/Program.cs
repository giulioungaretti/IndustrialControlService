using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcPressureTemperatureService;

using var channel = GrpcChannel.ForAddress("http://localhost:32791");


var client = new PressureTemperatureController.PressureTemperatureControllerClient(channel);
var req = new StartRequest();
var reply = client.Start(req);
Console.WriteLine("Greeting: " + reply.Success);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
