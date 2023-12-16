using UnityEngine;

namespace RaceSystem
{
    public class CheckpointRace : Race
    {
        [SerializeField] Checkpoint[] checkpoints;

        [SerializeField] float StartingTime = 10.0F;

        private RacerStats_CheckPoint[] racerStats;

        public RacerStats_CheckPoint[] RacerStats { get { return racerStats; } }

        private float remainingTime;
        public void AddToRemainingTime(float amount) => remainingTime += amount;
        public void ReduceRemainingTime(float amount) => remainingTime -= amount;

        private void Start()
        {
            print("start_checkpoint");

            InitialiseRace();

            InitialiseCheckpoints();

            AddToRemainingTime(StartingTime);

            InitialiseRacerStats();

            InitUI();
        }

        private void InitUI()
        {
            StartCoroutine(FEManager.instance.MakeVisible(FEManager.instance.FE_Leaderboard.canvasGroup, true));
        }

        private void InitialiseRacerStats()
        {
            racerStats = new RacerStats_CheckPoint[allRacers.Length];

            for (int i = 0; i < allRacers.Length; i++)
            {
                racerStats[i] = new RacerStats_CheckPoint(allRacers[i]);
            }
        }

        private void InitialiseCheckpoints()
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].SetRace(this);
            }
        }

        private void Update()
        {
            if (raceHasStarted && !raceIsComplete)
            {
                //TO-DO : SHOW REMAINING TIME.

                UpdateLeaderBoard();

                ProcessTimers();

                UpdateCheckpoints();

                if (remainingTime <= 0)
                {
                    //YOU RAN OUT OF TIME!

                    //Show Slo-Mo Failed Screen.

                    Time.timeScale = 0.25F;

                    raceIsComplete = true;
                }
            }
        }

        public void IncrementPlayerNodeIndex()
        {
            racerStats[PlayerRacerStatIndex].currentCheckpointIndex++;
        }

        private void UpdateCheckpoints()
        {
            for (int i = 0; i < checkpoints.Length; i++)
            {
                checkpoints[i].gameObject.SetActive(i == racerStats[PlayerRacerStatIndex].currentCheckpointIndex);
            }
        }

        public float GetRemainingTime() => remainingTime;

        private void UpdateLeaderBoard()
        {
            for (int i = 0; i < racerStats.Length; i++)
            {
                if (racerStats[i].racer is Player)
                {
                    FEManager.instance.UpdateText(ref FEManager.instance.FE_Leaderboard.raceProgress, (racerStats[i].currentCheckpointIndex) + "/" + (checkpoints.Length));
                }

                if (racerStats[i].currentCheckpointIndex >= checkpoints.Length)
                {
                    raceIsComplete = true;

                    print("race is complete");

                    return;
                }
            }
        }

        Vector3[] debug_V3List;

        private void OnDrawGizmos()
        {
            if (debug_V3List.Length <= checkpoints.Length)
            {
                debug_V3List = new Vector3[checkpoints.Length];

                for (int i = 0; i < debug_V3List.Length; i++)
                {
                    debug_V3List[i] = checkpoints[i].transform.position;
                }
            }

            Gizmos.DrawLineList(debug_V3List);
        }
    }

    [System.Serializable]
    public struct RacerStats_CheckPoint
    {
        public int currentCheckpointIndex;

        public Racer racer;

        public RacerStats_CheckPoint(Racer racer, int currentCheckpointIndex = 0)
        {
            this.currentCheckpointIndex = currentCheckpointIndex;
            this.racer = racer;
        }

        public override string ToString()
        {
            string info = string.Format(
                "Current Node Index : {0} : " +
                "Racer Name : {2}", currentCheckpointIndex, racer);

            return info;
        }
    }
}