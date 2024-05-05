using UnityEngine;

namespace ThirdPersonFramework
{
    public class EntityManager : MonoBehaviour
    {
        [Tooltip("Anything below this will be deleted unless otherwise state.")]
        [SerializeField] float m_KillZoneHeight = -1000;

        public static EntityManager Instance;

        public float GetKillZoneHeight() => m_KillZoneHeight;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // Useful for cutscenes or just clearing off the area.
        public void ClearEntitiesInArea(Vector3 centre, float radius)
        {
            Collider[] result = null;

            int colliderCount = Physics.OverlapSphereNonAlloc(centre, radius, result);

            if (colliderCount <= 0)
                return;
            else
            {
                int numberOfEntitiesDestroyed = 0;

                foreach (Collider collider in result)
                {
                    if (collider.TryGetComponent<Entity>(out var entity))
                    {
                        // TODO : Ignore the Player Vehicle

                        Destroy(entity.gameObject);

                        numberOfEntitiesDestroyed++;
                    }
                }

                Debug.Log($"Cleared off {numberOfEntitiesDestroyed} entities.");
            }
        }
    }
}