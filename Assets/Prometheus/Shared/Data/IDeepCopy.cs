using networkprotocol;

namespace Prometheus.Shared.Data
{
    /// <summary>
    /// Interface to make deep copy of Network Messages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeepCopy<out T> where T : Message
    {
        T Copy();
    }
}