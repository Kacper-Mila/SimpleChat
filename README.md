# SimpleChat

SimpleChat is a basic chat application built using C# and .NET 9.0. It consists of a server and client that communicate over TCP. The server stores chat messages and user information in a SQLite database.

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQLite

### Building the Project

To build the project, navigate to the root directory and run:
``` sh
dotnet build
```

Running the Server
To start the server, navigate to the Server directory and run:
``` sh
dotnet run
```

Running the Client
To start the client, navigate to the Client directory and run:
``` sh
dotnet run
```

## Project Details
### ChatStream
The ChatStream project contains the core logic for the chat application, including message handling, database operations, and utility functions.

- ChatStream/Constants.cs: Contains constants for the application, such as the port number and IP address.
- ChatStream/DatabaseHelper.cs: Provides methods for interacting with the SQLite database, including adding messages and users, and retrieving messages.
- ChatStream/Messenger.cs: Handles the creation and parsing of message packets for network communication.
- ChatStream/Program.cs: The entry point for the ChatStream project.
- ChatStream/entities/MessageEntity.cs: Represents a chat message entity.
- ChatStream/entities/UserEntity.cs: Represents a user entity.
- ChatStream/utils/Utils.cs: Contains utility functions, such as generating random colors.

### Client
The Client project contains the client-side logic for connecting to the server, sending messages, and receiving messages.

- Client/Program.cs: The entry point for the client application. Handles user input, connects to the server, and manages message sending and receiving.

### Server
The Server project contains the server-side logic for accepting client connections, receiving messages, and broadcasting messages to all connected clients.

- Server/Program.cs: The entry point for the server application. Manages client connections, receives messages, and broadcasts messages to clients.
