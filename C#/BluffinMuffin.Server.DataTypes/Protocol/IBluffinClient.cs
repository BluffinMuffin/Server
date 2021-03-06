﻿using System;
using BluffinMuffin.Protocol;

namespace BluffinMuffin.Server.DataTypes.Protocol
{
    public interface IBluffinClient
    {
        string PlayerName { get; set; }
        string ClientIdentification { get; set; }
        Version SupportedProtocol { get; set; }

        void SendCommand(AbstractCommand command);

        void AddPlayer(IPokerPlayer p);
        void RemovePlayer(IPokerPlayer p);
    }
}
