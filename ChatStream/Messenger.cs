using System.Text;
using System.Text.Json;

namespace ChatStream;

public class Messenger
{
    // Function to create a message packet
    // This function takes in an opcode (operation code) and a message,
    // and generates a byte array. This byte array can be directly sent over the network.
    public byte[] CreateMessagePacket(int opcode, MessageEntity messageEntity)
    {
        // Use a memory stream to hold the data
        // BinaryWriter is then used to write the opcode and the message to this stream
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(ms);

        writer.Write(opcode);     // Write opcode into the stream
        var messageJson = JsonSerializer.Serialize(messageEntity);
        writer.Write(messageJson);    // Write message into the stream
        
        // Get the byte array to send over the network
        return ms.ToArray();
    }

    // Function to parse incoming data into opcode and message
    // This function takes in a byte array, which is the format as created by the
    // CreateMessagePacket function, and extracts the opcode and the message from it
    public (int opcode, MessageEntity message) ParseMessagePacket(byte[] data)
    {
        // Use a memory stream to hold the data
        // BinaryReader is then used to read the opcode and the message from this stream
        using MemoryStream ms = new MemoryStream(data);
        using BinaryReader reader = new BinaryReader(ms, Encoding.ASCII);

        int opcode = reader.ReadInt32();        // Read opcode from the stream
        string messageJson = reader.ReadString();   // Read message JSON from the stream
        
        // Deserialize the message JSON into a Message object
        MessageEntity messageEntity = JsonSerializer.Deserialize<MessageEntity>(messageJson);
        
        // Return the opcode and the message
        return (opcode, messageEntity);
    }
}
