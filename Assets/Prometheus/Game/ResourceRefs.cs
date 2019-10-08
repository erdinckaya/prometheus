using UnityEngine;

namespace Prometheus.Game
{
    /// <summary>
    /// This MonoBehaviour is responsible for Meshes and Material to pass them to ECS.
    /// </summary>
    public class ResourceRefs : MonoBehaviour
    {
        public static ResourceRefs Instance;
        
        
        [SerializeField]
        public Mesh PlayerMesh;
        [SerializeField]
        public Material PlayerMaterial;
        
        [SerializeField]
        public Material BallMaterial;

        private void Awake()
        {
            Instance = this;
        }
    }
}