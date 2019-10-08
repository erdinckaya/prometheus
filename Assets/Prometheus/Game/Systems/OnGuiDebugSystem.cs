using Prometheus.Game.Components;
using Prometheus.Game.Debug;
using Prometheus.Game.Groups;
using Unity.Entities;
using Unity.Transforms;

namespace Prometheus.Game.Systems
{
    /// <summary>
    /// This is debug system that passes necessary values to OnGui function.
    /// </summary>
    [UpdateInGroup(typeof(DebugGroup))]
    public class OnGuiDebugSystem : ComponentSystem
    {
        private int _times;
        protected override void OnUpdate()
        {
            // Since it is debug function it does not need to work full precision.
            _times++;
            _times %= 5;
            
            if (_times != 0)
            {
                return;
            }
            
            // Get player pos
            Entities.WithAll<PlayerComp>().ForEach((Entity entity, ref Translation translation) =>
            {
                OnGuiDebugger.Instance.PlayerPos = translation.Value;
            });
            // Get rival pos
            Entities.WithAll<RivalComp>().ForEach((Entity entity, ref Translation translation) =>
            {
                OnGuiDebugger.Instance.RivalPos = translation.Value;
            });
            
            // Get ball pos and speed.
            Entities.WithAll<BallComp>().ForEach((Entity entity, ref Translation translation, ref BallComp ballComp) =>
            {
                OnGuiDebugger.Instance.BallPos  = translation.Value;
                OnGuiDebugger.Instance.BallPace = ballComp.Pace;
            });
        }
    }
}