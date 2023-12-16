using UnityEngine;

namespace RaceSystem 
{
    [RequireComponent(typeof(BoxCollider))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] float additionalTime = 10.0F;

        bool isCollected = false;

        public bool IsCollected { get { return isCollected; } }

        private CheckpointRace currentRace;

        new private BoxCollider collider;

        private void Awake()
        {
            if (collider is null)
            {
                collider = GetComponent<BoxCollider>();
            }

            collider.isTrigger = true;
        }

        public void SetRace(CheckpointRace currentRace) => this.currentRace = currentRace;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerVehicleInput>(out var player))
            {
                if (player.playerControlEnabled == true)
                {
                    isCollected = true;

                    currentRace.AddToRemainingTime(additionalTime);

                    currentRace.IncrementPlayerNodeIndex();

                    gameObject.SetActive(false);
                }
            }
        }
    }
}