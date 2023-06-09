using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace OkoCommon.Communication;

/// <summary>
///     Interface for sending and receiving objects over a stream
/// </summary>
public interface IObjectTransfer : IDisposable
{
    void Send<T>(T obj);
    T Receive<T>();
}

/// <summary>
///     Implementation of IObjectTransfer using JSON over TCP
/// </summary>
public class JsonTcpTransfer : IObjectTransfer
{
    private readonly TcpClient client;

    private readonly JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
    };

    private readonly NetworkStream stream;
    private string lastJson = "";

    private string prevJson = "";

    public JsonTcpTransfer(string ip, int port)
    {
        client = new TcpClient(ip, port);
        stream = client.GetStream();

        if (!stream.CanRead || !stream.CanWrite)
            throw new ArgumentException("The stream must be readable and writable");
    }

    public JsonTcpTransfer(TcpClient client)
    {
        this.client = client;
        stream = client.GetStream();

        if (!stream.CanRead || !stream.CanWrite)
            throw new ArgumentException("The stream must be readable and writable");
    }

    public void Send<T>(T obj)
    {
        if (stream == null)
            throw new InvalidOperationException("The stream is null");

        var json = JsonConvert.SerializeObject(obj, settings);
        lastJson = json;

        var jsonBytes = Encoding.UTF8.GetBytes(json);
        stream.Write(jsonBytes, 0, jsonBytes.Length);
    }

    public T Receive<T>()
    {
        string json;

        if (prevJson.Length == 0)
        {
            if (stream == null)
                throw new InvalidOperationException("The stream is null");

            var ms = new MemoryStream();
            var buffer = new byte[1024];

            do
            {
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, bytesRead);
            } while (stream.DataAvailable);

            /* NOTE timeout version
             
            var timeoutMilliseconds = 0;
            var cts = new CancellationTokenSource(timeoutMilliseconds);
            var token = cts.Token;

            try
            {
                Task.Run(() =>
                {
                    do
                    {
                        var bytesRead = stream.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, bytesRead);
                    } while (stream.DataAvailable);

                }, token).Wait(token);
            }
            catch (OperationCanceledException)
            {
                // send timeout notification
            
                var lastJsonBytes = Encoding.UTF8.GetBytes(lastJson);
                stream.Write(lastJsonBytes, 0, lastJsonBytes.Length);
                
                return Receive<T>();
            }
            */

            var jsonBytes = ms.ToArray();

            json = prevJson + Encoding.UTF8.GetString(jsonBytes);
            prevJson = "";
        }
        else
        {
            json = prevJson;
            prevJson = "";
        }

        var jsons = json.Split("}{");

        // Handle multiple objects in one messagei
        if (jsons.Length > 1) json = jsons[0] + "}";
        for (var i = 1; i < jsons.Length; i++) prevJson += "{" + jsons[i] + "}";
        if (prevJson.Length > 0) prevJson = prevJson[..^1];

        var obj = JsonConvert.DeserializeObject(json, settings) ?? throw new InvalidOperationException();
        if (obj is T t) return t;

        throw new InvalidCastException($"Cannot cast {obj.GetType()} to {typeof(T)}");
    }

    public void Dispose()
    {
        stream.Dispose();
        client.Dispose();
    }
}