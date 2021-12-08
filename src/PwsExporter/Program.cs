using System.Text.Json;

if (args.Length < 3)
{
    Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} <api-key> <station-id> <start-date> [<end-date>]");
    Console.Error.WriteLine("Missing arguments!");
    return 1;
}

var apiKey = args[0];
var stationId = args[1];
if (!DateOnly.TryParse(args[2], out var startDate))
{
    Console.Error.WriteLine("Bad format: <start-date>");
    return 1;
}
DateOnly endDate;
if (args.Length >= 4)
{
    if (!DateOnly.TryParse(args[3], out endDate))
    {
        Console.Error.WriteLine("Bad format: <end-date>");
        return 1;
    }
}
else
{
    endDate = DateOnly.FromDateTime(DateTime.Today);
}

const int throttle = 2200; // 2200 ms (API limit: 1500 calls/day or 30 calls/minute)
const string outputDirectory = "./out";
const string format = "json";
const string units = "m";
const string dataNodeName = "observations";
using var client = new HttpClient();
var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true, };

var currentDate = startDate;
while (currentDate <= endDate)
{
    Console.WriteLine($"{currentDate.ToString("yyyy-MM-dd")}...");

    try
    {
        var url = $"https://api.weather.com/v2/pws/history/all?apiKey={apiKey}&stationId={stationId}&format={format}&numericPrecision=decimal&units={units}&date={currentDate.ToString("yyyyMMdd")}";
        await Task.Delay(throttle);
        var jsonStream = await client.GetStreamAsync(url);
        var document = await JsonDocument.ParseAsync(jsonStream);
        if (document.RootElement.TryGetProperty(dataNodeName, out var observationsElement))
        {
            if (observationsElement.GetArrayLength() > 0)
            {
                var directory = outputDirectory + $"/{stationId}/{currentDate.Year:D4}/{currentDate.Month:D2}";
                Directory.CreateDirectory(directory);

                var path = directory + $"/{currentDate.ToString("yyyy-MM-dd")}.json";
                await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(fileStream, document, jsonSerializerOptions);
            }
        }
        else
        {
            Console.Error.WriteLine("Bad data: " + JsonSerializer.Serialize(document, jsonSerializerOptions));
            return 3;
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Error:");
        Console.Error.WriteLine(ex);
        return 2;
    }

    currentDate = currentDate.AddDays(1);
}

return 0;
