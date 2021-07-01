# Connections
Connections is a simple TCP wrapper for easy Server/Client communication. It is easy to use, yet is extremely customizable and powerful. While very simple,
connections contains a few starting features to help quickstart you network project:

- Thread Safety
- Persistent TCP Connections
- Multiple Users Connected To Server Support
- Built-in Client Lists and Count via `server.GetClients()` and `server.ClientCount`
- Very little startup code required

## Examples
Note that these examples use strings, however you can use any type of data supported by BinaryReader/Writer

### Client Setup Example
```c#
//Connect To server
Client server = Client.Connect("127.0.0.1", 12345); 

//If the connections succeeds you can read or write data:
server.Writer.Write("Hello!");

//To receive data, it is recommended to activate a second thread:
Task.Run(delegate() {
  while (true){
    string readable = server.Reader.ReadString();
    //Do Something with the data  
  }
});
```
Obviously, you will need to be reponsible for handling exceptions if a disconnect occurs.

### Server Setup Example
```c#
//Initialize the Server Delegate. (You can obviously use a separate method)
Server server = new Server(delegate(Client connection) {
    string ip = connection.GetIPAddress().ToString();
    while (true){
        string input = connection.Reader.ReadString();
        connection.Writer.Write("Hello!");
    }
}, 12345);

//Start the server
server.Start();
```
Note this code is spawned on a separate thread so use the lock statement if dealing with shared data!
### Extra Features
Included features are a Client list that can indicate the amount of users connected. Obviously, you can still implement your own:
```c#
static void ProcessRequestDelegate(Client client)
{
  bool ClientAuthorized = false;
  
  //The max users property is in a lock statement ensuring thread safety
  if (server.ClientCount > MaxUsers)
    client.Writer.Write("Sorry, The Server is full!");
  else ClientAuthorized = true;

  while (ClientAuthorized){
    //Get a message and relay it to the other connected clients
    string str = client.Reader.ReadString();
    
    //This delegate uses a lock statement, which will ensure mulithreading safety
    server.GetClients(delegate(List<Client> clients){
      //Cycle through all the connected clients but ignore the sending client.
      for (int i = 0; i < clients.Count; i++)
        if (!object.ReferenceEquals(clients[i], client))
          clients[i].Writer.Write(str);
    });
  }
}
```
Note that you can always kick clients by breaking the while loop!
