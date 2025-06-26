using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Package.WS.Abstractions;

namespace Package.WS.Middlewares;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly WebSocketHandler _wsHandler;
    
    public WebSocketMiddleware(RequestDelegate next, WebSocketHandler wsHandler)
    {
        _next = next;
        _wsHandler = wsHandler;
    }
    
    private static async Task Echo(WebSocket ws, Action<WebSocketReceiveResult, byte[]> callback)
    {
        var buffer = new byte[1024 * 4];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(
                buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);
            callback(result, buffer);
        }
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await _next(context);
            return;
        }
        var id = context.Request.Query["userId"].ToString();
        if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
        var ws = await context.WebSockets.AcceptWebSocketAsync();
        await _wsHandler.OnConnectedAsync(id, ws);
        await Echo(ws, async void (result, buffer) =>
        {
            try
            {
                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                        await _wsHandler.ReceiveAsync(ws, result, buffer);
                        break;
                    case WebSocketMessageType.Close:
                        await _wsHandler.OnDisconnectedAsync(id, ws);
                        break;
                    case WebSocketMessageType.Binary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                throw; // TODO handle exception
            }
        });
    }
}