using Lidgren.Network;

namespace design_patterns.Messaging; 

public class NetWorker {
    private readonly NetPeer _peer;
    private object _customContext;

    public NetWorker(NetPeer peer, object context)
    {
        this._peer = peer;
        this._customContext = context;
    }

    
}
