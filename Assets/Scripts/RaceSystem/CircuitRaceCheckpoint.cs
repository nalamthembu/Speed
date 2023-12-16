using UnityEngine;

namespace RaceSystem
{
    [RequireComponent(typeof(BoxCollider))]

    public class CircuitRaceCheckpoint : MonoBehaviour
    {
        private CircuitRace currentRace;

        new private BoxCollider collider;

        private bool passedByPlayer;

        [SerializeField] float targetSpeed = 90.0F; 

        private void Awake()
        {
            if (collider is null)
            {
                collider = GetComponent<BoxCollider>();
            }

            collider.isTrigger = true;
        }

        public void SetRace(CircuitRace currentRace) => this.currentRace = currentRace;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.parent != null)
            {
                if (other.transform.parent.TryGetComponent<Player>(out var player))
                {
                    passedByPlayer = true;

                    currentRace.IncrementRacerNodeIndex(player);

                    return;
                }
            }

            if (other.TryGetComponent<AIDriver>(out var racer))
            {
                //I'm not quite sure why these numbers are so big
                if (racer.remainingDistance <= 15)
                {
                    currentRace.IncrementRacerNodeIndex(racer);
                    racer.SetTargetSpeed(this.targetSpeed);
                }
            }
        }
    }
}