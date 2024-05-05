using UnityEngine;

namespace ThirdPersonFramework
{
    public class BaseEntityComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            m_ThisEntity = GetComponent<Entity>();
            Initialise();
        }
        protected virtual void Update() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }
        protected virtual void Start() { }
        protected virtual void Initialise() { }

        protected Entity m_ThisEntity;
    }
}