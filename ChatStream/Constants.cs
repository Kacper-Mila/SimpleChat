using System.Net;

namespace ChatStream;

public class Constants
{
    //Choose a port that's not already occupied.
    public static int PORT = 4789;
    //We're using .Loopback for testing, and .Any for when making it public.
    public static IPAddress Address = IPAddress.Loopback; 
}
