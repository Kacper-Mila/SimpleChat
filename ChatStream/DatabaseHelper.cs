using System.Data.SQLite;

namespace ChatStream;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var databasePath = Path.Combine(documentsPath, "SimpleChat", "simple_chat.db");

        // Ensure the directory exists, does nothing if exists
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath));

        _connectionString = $"Data Source={databasePath};Version=3;";
        Console.WriteLine($"Data will be stored in {databasePath}");
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();

            const string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Messages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Sender TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    MessageSent DATETIME NOT NULL
                );";
            
            const string createUsersTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Color TEXT NOT NULL
            );";

            using (var command = new SQLiteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
            
            using (var command = new SQLiteCommand(createUsersTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public void AddMessage(MessageEntity messageEntity)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        const string insertQuery = @"
                INSERT INTO Messages (Sender, Content, MessageSent)
                VALUES (@Sender, @Content, @MessageSent);";

        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Sender", messageEntity.Sender);
        command.Parameters.AddWithValue("@Content", messageEntity.Content);
        command.Parameters.AddWithValue("@MessageSent", messageEntity.MessageSent);

        command.ExecuteNonQuery();
    }

    public List<MessageEntity> GetAllMessages()
    {
        var messages = new List<MessageEntity>();
        
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        const string selectQuery = @"
                SELECT * FROM Messages;";
        
        using var command = new SQLiteCommand(selectQuery, connection);
        using var reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            var message = new MessageEntity
            {
                Sender = reader.GetString(1),
                Content = reader.GetString(2),
                MessageSent = reader.GetDateTime(3)
            };
            messages.Add(message);
        }
        
        return messages;
    }
    
    public void AddUser(string username, string color)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        const string insertQuery = @"
                INSERT INTO Users (Username, Color)
                VALUES (@Username, @Color);";

        using var command = new SQLiteCommand(insertQuery, connection);
        command.Parameters.AddWithValue("@Username", username);
        command.Parameters.AddWithValue("@Color", color);

        command.ExecuteNonQuery();
    }
    
    public UserEntity GetUser(string username)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();
        
        const string selectQuery = @"
                SELECT * FROM Users WHERE Username = @Username;";
        
        using var command = new SQLiteCommand(selectQuery, connection);
        command.Parameters.AddWithValue("@Username", username);
        
        using var reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            return new UserEntity(reader.GetString(1), (ConsoleColor)Enum.Parse(typeof(ConsoleColor), reader.GetString(2)));
        }
        
        return null;
    }
}