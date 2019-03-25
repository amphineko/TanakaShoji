using System.IO;
using System.Net.WebSockets;

namespace TanakaShoji.Discord.Gateway
{
    public delegate void WebSocketMessageHandler(MemoryStream stream, WebSocketMessageType type);
}