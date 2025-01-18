using System.Net.Sockets;
using ChatStream;

// Messenger instances for data sending and receiving
var inStream = new Messenger();
var outStream = new Messenger();

// A list to keep track of all connected clients
List<TcpClient> _clients = new List<TcpClient>();

// TCP listener for accepting client connections
TcpListener listener = new TcpListener(Constants.Address, Constants.PORT);
listener.Start();

Console.WriteLine("Server started!");

while (true)
{
    // Accept up to 5 pending client connections
    AcceptClients();
    
    // Receive message from each connected client
    ReceiveMessage();
}

// Function to accept client connections
void AcceptClients()
{
    for (int i = 0; i < 5; i++)
    {
        if (!listener.Pending()) continue;

        // If a client connection is pending, accept it and add to the _clients list
        var client = listener.AcceptTcpClient();
        _clients.Add(client);
        Console.WriteLine("Client accepted!");
    }
}

// Function to receive message from each client
void ReceiveMessage()
{
    foreach (var client in _clients)
    {
        NetworkStream stream = client.GetStream();

        if (stream.DataAvailable)
        {
            // Read data from the client and parse it into a message packet
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            (int opcode, Message message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
            
            Console.WriteLine($"Received: [{opcode}] | {message}");
            
            // Broadcast received message to all other connected clients
            Broadcast(client, message);
        }
    }
}

// Function to broadcast a message from one client to all other connected clients
void Broadcast(TcpClient sender, Message message)
{
    foreach (var client in _clients.Where(x => x != sender))
    {
        // Create a message packet and send it to the client
        var packet = outStream.CreateMessagePacket(10, message);
        client.GetStream().Write(packet);
    }
}