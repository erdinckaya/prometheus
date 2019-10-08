using Unity.Entities;

namespace Prometheus.Game.Groups
{
    // These are basic groups that encapsulates systems
    // And determines their execution order to finish
    // all of the game logic in one frame.
    
    
    /// <summary>
    /// Network group includes `BallStateUpdateSystem` and `PlayerStateUpdateSystem`
    /// </summary>
    public class NetworkGroup : ComponentSystemGroup
    {
    }
    
    /// <summary>
    /// Input group includes `PlayerInputSystem`
    /// </summary>
    [UpdateAfter(typeof(NetworkGroup))]
    public class InputGroup : ComponentSystemGroup
    {
    }
    
    /// <summary>
    /// Move group includes `BallMoveSystem` `RivalMoveSystem` and `RenderSortingOrderSystem`
    /// </summary>
    [UpdateAfter(typeof(InputGroup))]
    public class MoveGroup : ComponentSystemGroup
    {
    }
    
    
    /// <summary>
    /// Interpolate group includes `InterpolateSystem`
    /// </summary>
    [UpdateAfter(typeof(MoveGroup))]
    public class InterpolateGroup : ComponentSystemGroup
    {
    }
    
    /// <summary>
    /// Debug group includes `OnGuiDebugSystem`
    /// </summary>
    [UpdateAfter(typeof(InterpolateGroup))]
    public class DebugGroup : ComponentSystemGroup
    {
    }
}