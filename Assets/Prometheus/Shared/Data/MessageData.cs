using System;
using Client;
using networkprotocol;

namespace Prometheus.Shared.Data
{
    /// <summary>
    /// MessageData is a temporary data class to keep messages and client indexes.
    /// Server keeps that data in priority queue to sort them up in terms of
    /// their priority which is message's type namely `GameMessageType` 
    /// </summary>
    public struct MessageData : IComparable<MessageData>
    {
        public          Message         m;
        public readonly int             clientIndex;
        public readonly GameChannelType channelType;

        public MessageData(Message m, int ci, GameChannelType gameChannelType)
        {
            this.m      = m;
            clientIndex = ci;
            channelType = gameChannelType;
        }

        public int CompareTo(MessageData other)
        {
            return m.Type.CompareTo(other.m.Type);
        }
    }
}