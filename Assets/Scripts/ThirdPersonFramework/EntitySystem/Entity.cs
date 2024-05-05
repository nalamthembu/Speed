using ThirdPersonFramework.UserInterface;
using UnityEngine;

namespace ThirdPersonFramework
{
    /*[PURPOSE]
     * This is the base class of every object 
     * the player can interact with, that includes
     * characters, pickups, props, etc.
     */

    public class Entity : MonoBehaviour
    {
        protected virtual void Awake() { Initialise(); }
        protected virtual void Update()
        {
            if (EntityManager.Instance != null && transform.position.y <= EntityManager.Instance.GetKillZoneHeight())
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnEnable()
        {
            PauseMenu.OnPauseMenuOpened += OnGamePaused;
            PauseMenu.OnPauseMenuClosed += OnGameResume;
        }
        protected virtual void OnDisable()
        {
            PauseMenu.OnPauseMenuOpened -= OnGamePaused;
            PauseMenu.OnPauseMenuClosed -= OnGameResume;
        }

        protected virtual void FixedUpdate() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnGamePaused() { }
        protected virtual void OnGameResume() { }
        protected virtual void Start() { }
        protected virtual void Initialise() { }
        protected virtual void OnDrawGizmosSelected() { }
        protected virtual void OnDrawGizmos() { }
    }
}
