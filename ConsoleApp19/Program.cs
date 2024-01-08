using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();


string deploymentName = config["OpenAI:DeploymentName"] ?? throw new InvalidOperationException("OpenAI:DeploymentName is not set.");
string modelId = config["OpenAI:ModelId"] ?? throw new InvalidOperationException("OpenAI:ModelId is not set.");
string endpoint = config["OpenAI:Endpoint"] ?? throw new InvalidOperationException("OpenAI:BaseUrl is not set.");
string key = config["OpenAI:Key"] ?? throw new InvalidOperationException("OpenAI:Key is not set.");

Kernel kernel = Kernel.CreateBuilder()
 .AddAzureOpenAIChatCompletion(
    deploymentName,
    endpoint,
    key).Build();

var chat = kernel.CreateFunctionFromPrompt(
@"MM-dd-yyyy を入力として受け取って yyyy/MM/dd を出力する。
結果のみ出力してください。
変換前：{{$message}}");

ChatHistory history = [];
while (true)
{
    Console.Write("User: ");
    string? message = Console.ReadLine();
    if (message == "exit")
    {
        break;
    }
    else
    {
        var result = await kernel.InvokeAsync(chat, new() {
            { "message", message},
        });
        Console.WriteLine(result.ToString());
        history.AddUserMessage(message!);
        history.AddAssistantMessage(result.ToString());
    }


}
