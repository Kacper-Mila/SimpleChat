namespace ChatStream;

// Message class to represent a chat message
public class MessageEntity
{
    public string Sender { get; set; }
    public string Content { get; set; }
    public DateTime MessageSent { get; set; }
    
    // Parameterless constructor for deserialization
    public MessageEntity()
    {
        MessageSent = DateTime.Now;
    }

    // Constructor that takes parameters
    public MessageEntity(string sender, string content, DateTime messageSent)
    {
        Sender = sender;
        Content = content;
        MessageSent = messageSent;
    }
    
    public override string ToString() => $"{MessageSent} - {Sender}: {Content}";
}