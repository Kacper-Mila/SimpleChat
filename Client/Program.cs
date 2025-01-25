using System.Net.Sockets;
using ChatStream;


async Task MainAsync()
{
    // Define the TCP Client
    TcpClient client = new TcpClient();

    Console.Write("Enter your username: ");
    string username = Console.ReadLine();

    // Connect the client to the Server using defined Address and Port in Constants
    await ConnectAsync(client);
    Console.WriteLine("Connected to the server!");

    // Define instances of Messenger for stream reading and writing
    Messenger inStream = new Messenger();
    Messenger outStream = new Messenger();

    // List to store outgoing messages from the client
    List<MessageEntity> outgoingMessages = new List<MessageEntity>();

    // Start tasks for reading and sending packets
    var readTask = ReadPacketsAsync(client, inStream);
    var sendTask = SendPacketsAsync(client, outStream, outgoingMessages);

    // Task to continuously read input from the console and add it to the outgoingMessages list
    while (true)
    {
        var content = Console.ReadLine();
        MessageEntity messageEntity = new MessageEntity(username, content, DateTime.Now);
        outgoingMessages.Add(messageEntity);
    }
}

async Task ConnectAsync(TcpClient client)
{
    while (!client.Connected)
    {
        try
        {
            await client.ConnectAsync(Constants.Address, Constants.PORT);
        }
        catch
        {
            Console.WriteLine("Reconnecting...");
            await Task.Delay(5000); // Wait for 5 seconds before retrying
        }
    }
}

async Task ReadPacketsAsync(TcpClient client, Messenger inStream)
{
    var stream = client.GetStream();
    while (true)
    {
        if (client.Connected && stream.DataAvailable)
        {
            try
            {
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    (int opcode, MessageEntity message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
                    Console.WriteLine($"{message}");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Disconnected from server. Attempting to reconnect...");
                await ConnectAsync(client);
                stream = client.GetStream();
            }
        }
        else
        {
            await Task.Delay(10); // Sleep for a short period to avoid busy-waiting
        }
    }
}

async Task SendPacketsAsync(TcpClient client, Messenger outStream, List<MessageEntity> outgoingMessages)
{
    while (true)
    {
        if (client.Connected && outgoingMessages.Count > 0)
        {
            try
            {
                var message = outgoingMessages[0];
                var packet = outStream.CreateMessagePacket(10, message);
                await client.GetStream().WriteAsync(packet);
                outgoingMessages.RemoveAt(0);
            }
            catch (IOException)
            {
                Console.WriteLine("Disconnected from server. Attempting to reconnect...");
                await ConnectAsync(client);
            }
        }
        else
        {
            await Task.Delay(10); // Sleep for a short period to avoid busy-waiting
        }
    }
}

await MainAsync();