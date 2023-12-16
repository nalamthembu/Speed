using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace RaceSystem
{
    //Race Base Class
    public class Race : MonoBehaviour
    {
        public static Race instance;

        protected int PlayerRacerStatIndex = 0;

        protected Racer[] allRacers;

        const int COUNT_DOWN_TIMER = 3;

        float countdownTimer = 3;

        protected bool raceHasStarted = false;

        protected bool raceIsComplete = false;

        public bool RaceHasStarted { get { return raceHasStarted; } }

        protected float timeElasped;

        [SerializeField] protected Transform StartingPositionTransform;

        protected void InitialiseRace()
        {
            if (instance is null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            PlacePlayerAtStart();
            StartCoroutine(DoCountdown());
            
            MusicManager.instance.PlayRandomSong();
        }

        protected void PlacePlayerAtStart()
        {
            print("placing player at start");

            Player.instance.Vehicle.transform.SetPositionAndRotation
                     (
                         StartingPositionTransform.position,
                         StartingPositionTransform.rotation
                     );
        }

        protected void ProcessTimers()
        {
            if (GameStateMachine.instance.gameState_Current is GameStatePaused)
                return;

            timeElasped += Time.deltaTime;
            FEManager.instance.FE_Leaderboard.timeElapsed.text = HelperMethods.StopWatchFormattedTime(timeElasped);

            if (this is CheckpointRace checkpointRace)
            {
                //reduce remaining time
                checkpointRace.ReduceRemainingTime(Time.deltaTime);

                //show remaining time.
            }
        }

        public IEnumerator DoCountdown()
        {
            FEManager.instance.FE_Racing.countdownTimer.text = string.Empty;

            while (FEManager.instance.FE_Racing.countdownTimer.alpha < 1)
            {
                FEManager.instance.FE_Racing.countdownTimer.alpha = Time.deltaTime;

                FEManager.instance.FE_Racing.countdownTimer.alpha = Mathf.Ceil(FEManager.instance.FE_Racing.countdownTimer.alpha);

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();

            while (countdownTimer > 0)
            {
                countdownTimer -= Time.deltaTime;

                FEManager.instance.FE_Racing.countdownTimer.text = Mathf.Ceil(countdownTimer).ToString();

                yield return new WaitForEndOfFrame();
            }

            raceHasStarted = true;

            raceIsComplete = false;

            FEManager.instance.FE_Racing.countdownTimer.text = "GO!";

            FEManager.instance.FE_Racing.countdownTimer.CrossFadeAlpha(0, 3, true);
        }
    }

    internal static class HelperMethods
    {
        public static string StopWatchFormattedTime(float currentTime)
        {
            TimeSpan t = TimeSpan.FromSeconds(currentTime);

            var sb = new StringBuilder();

            return sb.Append(string.Format
                (
                    "{0:00}:{1:00}:{2:000}",
                     t.Minutes,
                     t.Seconds,
                     Mathf.FloorToInt(t.Milliseconds) / 10f
                )).ToString();
        }
    }
}