using Prometheus.Game.Components;
using Prometheus.Shared.Utils;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Prometheus.Game
{
    /// <summary>
    /// This class is our root ECS class, that starts ECS.
    /// </summary>
    public class ECSManager
    {
        private static ECSManager _instance;
        public static ECSManager Instance => _instance ?? (_instance = new ECSManager());

        public EntityManager EntityManager;

        /// <summary>
        /// Starts the ECS World
        /// </summary>
        public void Start()
        {
            EntityManager = World.Active.EntityManager;

            CreatePlayer(true);
            CreatePlayer(false);
            CreateBall();
        }

        /// <summary>
        /// Creates player entity and components.
        /// </summary>
        /// <param name="isOne">True for player1 false for player2</param>
        private void CreatePlayer(bool isOne)
        {
            var pos      = isOne ? Constants.PlayerOneStartPos : Constants.PlayerTwoStartPos;
            var entities = new NativeArray<Entity>(1, Allocator.Temp);
            // Creating `ArchType`
            var entityArchetype = EntityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Scale),
                typeof(RenderMesh),
                typeof(State),
                typeof(RenderSortingOrderComp),
                typeof(MoveState),
                typeof(PlayerInfoComp),
                typeof(InterpolateComp),
                typeof(PlayerPosHistory),
                isOne ? typeof(PlayerComp) : typeof(RivalComp)
            );
            EntityManager.CreateEntity(entityArchetype, entities);

            // Initializing components.
            foreach (var entity in entities)
            {
                EntityManager.SetComponentData(entity, new Translation
                {
                    Value = pos
                });
                EntityManager.SetComponentData(entity, new Scale
                {
                    Value = Constants.DefaultPlayerScale
                });
                EntityManager.SetComponentData(entity, new State
                {
                    Id = 0
                });
                EntityManager.SetComponentData(entity, new RenderSortingOrderComp
                {
                    Value = 1
                });
                EntityManager.SetComponentData(entity, new MoveState
                {
                    State = 0
                });
                EntityManager.SetComponentData(entity, new PlayerInfoComp
                {
                    Id          = (ulong) (isOne ? 0 : 1),
                    IsOwner     = isOne,
                    IsConnected = false
                });
                EntityManager.SetComponentData(entity, new InterpolateComp
                {
                    Start    = pos,
                    End      = pos,
                    Duration = 0,
                    Progress = 0
                });
                EntityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh     = ResourceRefs.Instance.PlayerMesh,
                    material = ResourceRefs.Instance.PlayerMaterial
                });
            }

            // After all we dont need native array so that we dispose it.
            entities.Dispose();
        }

        /// <summary>
        /// Creates Ball entity and its components.
        /// </summary>
        private void CreateBall()
        {
            var pos      = Constants.BallStartPos;
            var entities = new NativeArray<Entity>(1, Allocator.Temp);
            // Create ArchType
            var entityArchetype = EntityManager.CreateArchetype(
                typeof(LocalToWorld),
                typeof(Translation),
                typeof(Scale),
                typeof(RenderMesh),
                typeof(State),
                typeof(RenderSortingOrderComp),
                typeof(InterpolateComp),
                typeof(BallComp)
            );
            // Create entity with that ArcType
            EntityManager.CreateEntity(entityArchetype, entities);

            // Initialize components.
            foreach (var entity in entities)
            {
                EntityManager.SetComponentData(entity, new Translation
                {
                    Value = pos
                });
                EntityManager.SetComponentData(entity, new Scale
                {
                    Value = Constants.DefaultBallScale
                });
                EntityManager.SetComponentData(entity, new State
                {
                    Id = 0
                });
                EntityManager.SetComponentData(entity, new RenderSortingOrderComp
                {
                    Value = 0
                });
                EntityManager.SetComponentData(entity, new BallComp
                {
                    Direction = float2.zero,
                    Pace      = Constants.BallPace
                });
                EntityManager.SetComponentData(entity, new InterpolateComp
                {
                    Start    = pos,
                    End      = pos,
                    Duration = 0,
                    Progress = 0
                });
                EntityManager.SetSharedComponentData(entity, new RenderMesh
                {
                    mesh     = ResourceRefs.Instance.PlayerMesh,
                    material = ResourceRefs.Instance.BallMaterial
                });
            }

            // Remove native array.
            entities.Dispose();
        }
    }
}