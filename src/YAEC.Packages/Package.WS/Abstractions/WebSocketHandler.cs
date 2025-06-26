using System.Net.WebSockets;
using System.Text;
using Package.WS.Core;

namespace Package.WS.Abstractions;

public abstract class WebSocketHandler
{
    protected WebSocketConnectionManager ConnectionManager { get; set; }
    
    protected WebSocketHandler(WebSocketConnectionManager connectionManager)
    {
        ConnectionManager = connectionManager;
    }
    
    public virtual Task OnConnectedAsync(string id, WebSocket socket)
    {
        ConnectionManager.AddConnection(id, socket);
        return Task.CompletedTask;
    }

    public virtual async Task OnDisconnectedAsync(string id, WebSocket socket)
    {
        await ConnectionManager.RemoveConnectionAsync(socket);
    }
    
    protected abstract Task ReceiveAsync(string socketId, WebSocket socket, string message);
    
    public async Task ReceiveAsync(WebSocket ws, WebSocketReceiveResult result, byte[] buffer)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        var connectionId = ConnectionManager.GetConnectionId(ws);
        if (string.IsNullOrEmpty(connectionId)) return;
        await ReceiveAsync(connectionId, ws, message);
    }
    
    public static async Task SendMessageAsync(WebSocket socket, string message, CancellationToken cancellationToken = default)
    {
        if (socket.State is not WebSocketState.Open) return;
        await socket.SendAsync(
            buffer: new ArraySegment<byte>(
                array: Encoding.UTF8.GetBytes(message),
                offset: 0,
                count: message.Length),
            messageType: WebSocketMessageType.Text,
            endOfMessage: true,
            cancellationToken: cancellationToken);
    }
}