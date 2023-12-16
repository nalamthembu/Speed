using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public MusicScriptable musicScriptable;

    private int nowPlaying = -1;

    private AudioSource source;

    public bool DEBUG_MODE = false;

    public static MusicManager instance;

    private void Awake()
    {
        if (instance is null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        source = gameObject.AddComponent<AudioSource>();

        source.outputAudioMixerGroup = musicScriptable.musicMixerGroup;

        source.bypassReverbZones = true;
    }

    public void PlayRandomSong()
    {
        nowPlaying = Random.Range(0, musicScriptable.music.Length - 1);

        int MAX_ITERATIONS = 100;

        if (GameManager.instance.IsInRace)
        {
            for (int i = 0; i < MAX_ITERATIONS; i++)
            {
                switch (musicScriptable.music[nowPlaying].type)
                {
                    case MUSIC_TYPE.MUSIC_BOTH:
                    case MUSIC_TYPE.MUSIC_RACING:

                        Song song = musicScriptable.music[nowPlaying];

                        source.clip = song.clip;

                        FEManager.instance.ShowNotification(song.artist + "\n" + song.title);

                        source.time = song.startTimeDuringRace;

                        source.Play();

                        print("RANDOM RACE SONG :" + song.title + " by : " + song.artist);

                        return;

                    default:

                        nowPlaying = Random.Range(0, musicScriptable.music.Length - 1);

                        break;
                }
            }

            Debug.LogWarning("Could not find a Race song and have run out of iterations");
        }
        else
        {
            Song song = musicScriptable.music[nowPlaying];

            source.clip = song.clip;

            FEManager.instance.ShowNotification(song.artist + "\n" + song.title);

            source.time = song.startTimeDuringRace;

            source.Play();
        }
    }


    private void Update()
    {
        if (DEBUG_MODE)
        {
            if (Input.GetKeyDown(KeyCode.RightBracket))
            {
                NextSong();
            }

            if (Input.GetKeyDown(KeyCode.LeftBracket))
            {
                PreviousSong();
            }
        }

        if (!source.isPlaying)
        {
            NextSong();
        }
    }

    public void NextSong()
    {
        nowPlaying++;

        if (nowPlaying > musicScriptable.music.Length - 1)
            nowPlaying = 0;

        while (musicScriptable.music[nowPlaying].type == MUSIC_TYPE.MUSIC_GARAGE && GameManager.instance.IsInRace)
        {
            nowPlaying++;

            if (nowPlaying > musicScriptable.music.Length - 1)
                nowPlaying = 0;
        }

        Song song = musicScriptable.music[nowPlaying];

        source.clip = song.clip;

        FEManager.instance.ShowNotification(song.artist + "\n" + song.title);

        if (GameManager.instance.IsInRace)
        {
            source.time = song.startTimeDuringRace;
        }

        source.Play();
    }

    public void PreviousSong()
    {
        nowPlaying--;

        if (nowPlaying < 0)
            nowPlaying = musicScriptable.music.Length - 1;

        while (musicScriptable.music[nowPlaying].type == MUSIC_TYPE.MUSIC_GARAGE && GameManager.instance.IsInRace)
        {
            nowPlaying--;

            if (nowPlaying < 0)
                nowPlaying = musicScriptable.music.Length - 1;
        }

        Song song = musicScriptable.music[nowPlaying];

        source.clip = song.clip;

        FEManager.instance.ShowNotification(song.artist + "\n" + song.title);

        if (GameManager.instance.IsInRace)
        {
            source.time = song.startTimeDuringRace;
        }

        source.Play();
    }
}
