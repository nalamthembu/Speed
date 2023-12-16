using UnityEngine;

namespace RaceSystem
{
    public class Racer : MonoBehaviour
    {
        protected Vehicle vehicle;

        public int RacerIndex { get; private set; }

        public Vehicle Vehicle { get { return vehicle; } }

        protected virtual void Awake()
        {
            if (vehicle is null)
                vehicle = GetComponent<Vehicle>();
        }

        public void SetRacerIndex(int index) => RacerIndex = index;
    }
}