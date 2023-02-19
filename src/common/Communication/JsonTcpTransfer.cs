using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace OkoCommon.Communication;

public interface IObjectTransfer : IDisposable
{
    void Send<T>(T obj);
    T Receive<T>();
}

public class JsonTcpTransfer : IObjectTransfer
{
    private readonly NetworkStream stream;
    private readonly TcpClient client;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
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

        var json = JsonSerializer.Serialize(obj, JsonSerializerOptions);

        var jsonBytes = Encoding.UTF8.GetBytes(json);
        stream.Write(jsonBytes, 0, jsonBytes.Length);
    }

    public T Receive<T>()
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


        var jsonBytes = ms.ToArray();
        var json = Encoding.UTF8.GetString(jsonBytes);
        
        return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions) ?? throw new InvalidOperationException("The deserialized object is null");
    }

    public void Dispose()
    {
        stream.Dispose();
        client.Dispose();
    }
}