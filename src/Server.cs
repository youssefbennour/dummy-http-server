using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();

while (true)
{
    Socket socket = server.AcceptSocket(); // wait for client

    var buffer = new byte[256];
    await socket.ReceiveAsync(buffer);

    var request = Encoding.UTF8.GetString(buffer);
    var requestPath = request.Split("\r\n")
        .FirstOrDefault()?
        .Split(" ").ElementAt(1);


    string? response = String.Empty;
    if (requestPath is "/")
    {
        response = "HTTP/1.1 200 OK\r\n\r\n";
    }
    else if (requestPath is "/user-agent")
    {
        response = HeaderExtractionResponse(request);
    }else if (requestPath.StartsWith("/echo"))
    {
        response = ParameterExtractionResponse(requestPath);
    }
    else
    {
        response = "HTTP/1.1 404 Not Found\r\n\r\n";
    }

    var bytesResponse = Encoding.ASCII.GetBytes(response ?? string.Empty);
    socket.Send(bytesResponse);
}

string ParameterExtractionResponse(string? requestPath)
{
    var parameter = requestPath?.Split("/")
            .LastOrDefault();
    
    var parameterLengthInBytes = Encoding.ASCII.GetByteCount(parameter ?? string.Empty);
    return
        $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {parameterLengthInBytes}\r\n\r\n{parameter}";
}

string? HeaderExtractionResponse(string request)
{
    var headers = request[(request.IndexOf("\r\n", StringComparison.InvariantCultureIgnoreCase) + 1)..]
        .Split("/r/n/r/n")
        .FirstOrDefault();
    
    var userAgentHeader = headers?.Split("\r\n")
    .FirstOrDefault(x => x.StartsWith("User-Agent"));

    var userAgentValue = userAgentHeader?.Split(':')?
        .ElementAtOrDefault(1)?
        .Trim();

    var userAgentValueLengthInBytes = Encoding.ASCII.GetByteCount(userAgentValue ?? string.Empty);
    return $"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {userAgentValueLengthInBytes}\r\n\r\n{userAgentValue}";
}