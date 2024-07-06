using System.Net;
using System.Net.Sockets;
using System.Text;

TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
Socket socket = server.AcceptSocket(); // wait for client

string response = "HTTP/1.1 200 OK\r\n\r\n";
byte[] bytesResponse = Encoding.ASCII.GetBytes(response);
socket.Send(bytesResponse);

