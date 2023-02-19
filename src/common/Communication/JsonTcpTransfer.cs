using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

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
    
    private string prevJson = "";

    private JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
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

        var json = JsonConvert.SerializeObject(obj, settings);

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
        
        var json = prevJson + Encoding.UTF8.GetString(jsonBytes);
        prevJson = "";
        
        var jsons = json.Split("}{");

        // Handle multiple objects in one message
        if (jsons.Length > 1) json = jsons[0] + "}";
        for (var i = 1; i < jsons.Length; i++) prevJson += "{" + jsons[i] + "}";
        if (prevJson.Length > 0) prevJson = prevJson[..^1];
        
        var obj = JsonConvert.DeserializeObject(json, settings);
        
        if (obj is T t)
            return t;
        
        // TODO Unable to cast object of type 'OkoCommon.Communication.PlayerNotif' to type 'OkoCommon.Communication.INotification`1[System.Object]'.
        // ...that will solve new players
        
        // throw new InvalidCastException($"Unable to cast object of type '{obj.GetType()}' to type '{typeof(T)}'.");

        return default!;
    }

    public void Dispose()
    {
        stream.Dispose();
        client.Dispose();
    }
}