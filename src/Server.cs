using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
Socket socket = server.AcceptSocket(); // wait for client

var buffer = new byte[256];
await socket.ReceiveAsync(buffer);

var request = Encoding.UTF8.GetString(buffer);
var path = request.Split("\r\n")
    .FirstOrDefault()?
    .Split(" ")[1];

var response = path is "/" 
    ? "HTTP/1.1 200 OK\r\n\r\n" 
    : "HTTP/1.1 404 Not Found\r\n\r\n";

var bytesResponse = Encoding.ASCII.GetBytes(response);
socket.Send(bytesResponse);

