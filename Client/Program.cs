// Define the TCP Client
using System.Net.Sockets;
using ChatStream;

TcpClient client = new TcpClient();

Console.Write("Enter your username: ");
string username = Console.ReadLine();

// Connect the client to the Server using defined Address and Port in Constants
client.Connect(Constants.Address, Constants.PORT);
Console.WriteLine("Connected to the server!");

// Define instances of Messenger for stream reading and writing
Messenger inStream = new Messenger();
Messenger outStream = new Messenger();

// List to store outgoing messages from the client
List<Message> outgoingMessages = new List<Message>();

// Task to continuously read input from the console and add it to the outgoingMessages list
new TaskFactory().StartNew(() =>
{
    while (true)
    {
        var content = Console.ReadLine();
        Message message = new Message(username, content, DateTime.Now);
        outgoingMessages.Add(message);
    }
});

// Main loop to read incoming packets and send outgoing packets 
while (true)
{
    ReadPackets();
    SendPackets();
}

// Function to read incoming packets from the server
void ReadPackets()
{
    var stream = client.GetStream();
    for (int i = 0; i < 10; i++)
    {
        if (stream.DataAvailable)
        {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            (int opcode, Message message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
            Console.WriteLine(message);
        }
    }
}

// Function to send packets to the server
void SendPackets()
{
    if (outgoingMessages.Count > 0)
    {
        var message = outgoingMessages[0];
        var packet = outStream.CreateMessagePacket(10, message);
        client.GetStream().Write(packet);
        outgoingMessages.RemoveAt(0);
    }
}