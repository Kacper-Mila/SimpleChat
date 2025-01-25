using System.Net.Sockets;
using ChatStream;
using static ChatStream.Utils;

var serverMessageColor = ConsoleColor.Green;
var serverUser = "Server";

// Messenger instances for data sending and receiving
var inStream = new Messenger();
var outStream = new Messenger();
// Initialize the database
var database = new DatabaseHelper();

// A list to keep track of all connected clients
List<TcpClient> _clients = new List<TcpClient>();

// TCP listener for accepting client connections
TcpListener listener = new TcpListener(Constants.Address, Constants.PORT);
listener.Start();

Console.WriteLine("Server started!");

while (true)
{
    AcceptClients();
    ReceiveMessage();
}

// Function to accept client connections (max 5 clients)
void AcceptClients()
{
    for (int i = 0; i < 5; i++)
    {
        if (!listener.Pending()) continue;

        // If a client connection is pending, accept it and add to the _clients list
        var client = listener.AcceptTcpClient();
        _clients.Add(client);
        Console.WriteLine("Client accepted!");
        LoadChatHistory(client);
    }
}

// Function to receive message from each client
void ReceiveMessage()
{
    var clientsCopy = _clients.ToList(); // Create a copy of the _clients list

    foreach (var client in clientsCopy)
    {
        NetworkStream stream = client.GetStream();

        if (stream.DataAvailable)
        {
            try
            {
                // Read data from the client and parse it into a message packet
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                (int opcode, MessageEntity message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());

                database.AddMessage(message);

                Console.WriteLine($"Received: [{opcode}] | {message}");

                // Broadcast received message to all other connected clients
                Broadcast(message, client);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error receiving packets: {ex.Message}");
                RemoveClient(client);
            }
        }
    }
}

void Broadcast(MessageEntity message, TcpClient sender = null)
{
    var clientsCopy = _clients.ToList();
    
    if (sender == null)
    {
        foreach (var client in _clients)
        {
            SendPacket(message, client);

        }
    }
    else
    {
        foreach (var client in clientsCopy.Where(x => x != sender))
        {
            try
            {
                SendPacket(message, client);
            }
            catch (IOException)
            {
                RemoveClient(client);
            }
        }
    }
}

// Create a message packet and send it to the client
void SendPacket(MessageEntity message, TcpClient client)
{
    try
    {
        var packet = outStream.CreateMessagePacket(10, message);
        var stream = client.GetStream();
        stream.Write(packet);
        stream.Flush();
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Error sending packet: {ex.Message}");
        RemoveClient(client);
    }
}

void LoadChatHistory(TcpClient client)
{
    List<MessageEntity> messages = new List<MessageEntity>();
    
    try
    { 
        Task.Run(() => messages = database.GetAllMessages()).Wait();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
    
    foreach (MessageEntity message in messages)
    {
        Task.Run(async () => { await Task.Delay(10); SendPacket(message, client); }).Wait();
    }
}

// Remove disconnected clients
void RemoveClient(TcpClient client)
{
    _clients.Remove(client);
    client.Close();
    Console.WriteLine($"Client disconnected! {_clients.Count} clients connected.");
}