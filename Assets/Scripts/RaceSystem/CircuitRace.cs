using UnityEngine;
using System.Collections.Generic;

namespace RaceSystem
{
    public class CircuitRace : Race
    {
        [SerializeField] int lapCount;

        [SerializeField] CircuitRaceCheckpoint[] checkpoints;

        private RacerStats_Circuit[] racerStats;

        [SerializeField] OpponentPositions[] opponents;

        [SerializeField] [Range(0, 10)] float distanceApart = 1;


        private void Start()
        {
            print("start_circuit");

            SpawnOpponents();

            InitialiseCheckpoints();

            InitRacerStats();

            InitialiseRace();

            InitUI();
        }

        private void SpawnOpponents()
        {
            for (int i = 0; i < opponents.Length; i++)
            {
                VehicleManager.instance.SpawnRandomRacer(StartingPositionTransform.position + (Vector3) opponents[i].position * distanceApart,
                    Vector3.up * opponents[i].YRotation);
            }
        }

        private void InitialiseCheckpoints()
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].SetRace(this);
            }
        }

        private void InitUI()
        {
            //Initialise UI

            StartCoroutine(FEManager.instance.MakeVisible(FEManager.instance.FE_Leaderboard.canvasGroup, true));

            FEManager.instance.UpdateText(ref FEManager.instance.FE_Leaderboard.raceProgress, racerStats[PlayerRacerStatIndex].currentLap + "/" + lapCount);

        }

        private void InitRacerStats()
        {
            allRacers = FindObjectsOfType<Racer>();

            racerStats = new RacerStats_Circuit[allRacers.Length];

            for (int i = 0; i < allRacers.Length; i++)
            {
                RacerStats_Circuit racer = new()
                {
                    racer = allRacers[i],
                    currentLap = 1
                };

                if (allRacers[i].TryGetComponent<AIDriver>(out var driver))
                {
                    driver.SetTarget(checkpoints[racer.currentNodeIndex].transform);
                }

                racer.racer.SetRacerIndex(i);

                racerStats[i] = racer;
            }
        }

        private void Update()
        {
            if (raceHasStarted && !raceIsComplete)
            {
                UpdateLeaderBoard();
                ProcessTimers();
            }
        }

        public void IncrementRacerNodeIndex(Racer racer)
        {
            for (int i = 0; i < racerStats.Length; i++)
            {
                if (racerStats[i].racer.gameObject == racer.gameObject)
                {
                    racerStats[i].currentNodeIndex++;

                    if (racer.TryGetComponent<AIDriver>(out var driver))
                    {
                        driver.SetTarget(checkpoints[racerStats[i].currentNodeIndex].transform);
                    }

                    break;
                }
            }
        }

        public void IncrementRaceNodeIndex(int index) => racerStats[index].currentNodeIndex++;

        private void UpdateLeaderBoard()
        {
            for (int i = 0; i < racerStats.Length; i++)
            {
                if (racerStats[i].currentLap < lapCount)
                {
                    if (racerStats[i].currentNodeIndex > checkpoints.Length - 1)
                    {
                        racerStats[i].currentLap++;

                        racerStats[i].currentNodeIndex = 0;

                        if (racerStats[i].racer is Player)
                        {
                            FEManager.instance.UpdateText(ref FEManager.instance.FE_Leaderboard.raceProgress, racerStats[i].currentLap + "/" + lapCount);
                        }
                    }
                }
                else
                {
                    //END THE RACE
                    raceIsComplete = true;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < opponents.Length; i++)
            {
                Vector3 position = StartingPositionTransform.position + new Vector3(opponents[i].position.x, 0) * distanceApart;

                position.y = opponents[i].position.y;

                Gizmos.DrawWireCube(position, Vector3.one);
            }
        }
    }

    [System.Serializable]
    public struct OpponentPositions
    {
        public Vector2 position;

        public float YRotation;
    }

    [System.Serializable]
    public struct RacerStats_Circuit
    {
        public string racerName;

        public int currentNodeIndex;

        public int currentLap;

        public Racer racer;

        public RacerStats_Circuit(string name, Racer racer, int currentNodeIndex = 0, int currentLap = 0)
        {
            this.currentNodeIndex = currentNodeIndex;
            this.currentLap = currentLap;
            this.racer = racer;
            this.racerName = name;
        }

        public override string ToString()
        {
            string info = string.Format(
                "Current Node Index : {0} : " +
                "Current Lap : {1} : " +
                "Racer Name : {2}", currentNodeIndex, currentLap, racer);

            return info;
        }
    }
}