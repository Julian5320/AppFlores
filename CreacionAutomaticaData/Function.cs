using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CreacionAutomaticaData;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="requestitem">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<string> FunctionHandler(ILambdaContext context)
    {

            Random random = new Random();
            double minTemp = -10;
            double maxTemp = 40;
            double minHumedad = 0;
            double maxHumedad = 100;
            double minCorte = 0;
            double maxCorte = 1;

            double randomTemp = random.NextDouble() * (maxTemp - minTemp) + minTemp;
            double randomHumedad = random.NextDouble() * (maxHumedad - minHumedad) + minHumedad;
            double randomCorte = random.NextDouble() * (maxCorte - minCorte) + minCorte;


            string tableName = "datosapp";

            AmazonDynamoDBConfig config = new AmazonDynamoDBConfig
            {
                // Configura la región
                RegionEndpoint = Amazon.RegionEndpoint.USWest2,
                ProxyCredentials = new NetworkCredential("{Key-Password}", "{Ket-Secret-Password}")
             };
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(config);

        var fecha = DateTime.UtcNow.ToString("O");
        var request = new PutItemRequest
            {
                TableName = tableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                  { "ValorCorte", new AttributeValue { S = randomCorte.ToString() }},
                  { "ValorHumedad", new AttributeValue { S = randomHumedad.ToString() }},
                  { "ValorTemp", new AttributeValue { S = randomTemp.ToString() }},
                  { "Id", new AttributeValue { S = fecha } }
                }
            };
            try
            {
                PutItemResponse response = await client.PutItemAsync(request);
                Console.WriteLine("Elemento insertado correctamente.");
            var Datos = new AppData()
            {
                Date = fecha.ToString(),
                TemperatureC = randomTemp.ToString(),
                Cortada = randomCorte.ToString(),
                Humedad = randomHumedad.ToString()
            };
            var options = new JsonSerializerOptions { WriteIndented = false };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(Datos, options);

            //Console.WriteLine("Temperatura aleatoria: {0}", randomTemp);
            return jsonString;
            }
            catch (AmazonDynamoDBException ex)
            {
                return("Error al insertar el elemento: " + ex.Message);

            }
    }
    private class AppData
    {
        public string? Date { get; set; }
        public string? TemperatureC { get; set; }
        public string? Humedad { get; set; }
        public string? Cortada { get; set; }

    }

}
