using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Package.WS.Core;

public class WebSocketConnectionManager
{
    /// <summary>
    /// Key: ConnectionId <br/>
    /// Value: WebSockets
    /// </summary>
    private readonly ConcurrentDictionary<string, List<WebSocket>> _connections = new();
    
    private static string NormalizeId(string id) => id.ToLower();

    public ConcurrentDictionary<string, List<WebSocket>> Connections() => _connections;

    public IEnumerable<WebSocket>? GetConnection(string id) => _connections.GetValueOrDefault(NormalizeId(id));
    
    public string? GetConnectionId(WebSocket ws)
    {
        var connectionId = _connections
            .FirstOrDefault(x => x.Value.Any(s => s == ws))
            .Key;
        return string.IsNullOrEmpty(connectionId) ? null : NormalizeId(connectionId);
    }

    public void AddConnection(string id, WebSocket ws)
    {
        var existedConnections = _connections.GetValueOrDefault(id);
        if (existedConnections is null)
        {
            _connections[NormalizeId(id)] = [ws];
            return;
        }
        existedConnections.Add(ws);
    }
    
    public async Task RemoveConnectionAsync(WebSocket ws)
    {
        foreach (var pair in _connections)
        {
            var removed = pair.Value.Remove(ws);
            if (removed)
            {
                await ws.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "The server closes this connection",
                    CancellationToken.None);
            }
            if (pair.Value.Count == 0) _connections.TryRemove(pair);
        }
    }
}