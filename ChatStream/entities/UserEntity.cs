namespace ChatStream;

public class UserEntity
{
    public string Username { get; set; }
    public ConsoleColor Color { get; set; }
    public UserEntity(string username, ConsoleColor color)
    {
        Username = username;
        Color = color;
    }
}