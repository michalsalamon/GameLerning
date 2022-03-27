using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, Sfx, Music};

    public AudioClip mainTheme;
    public AudioClip menuTheme;

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex = 0;
    private string sceneName;

    private Transform audioListener;
    private Transform player;
    private SoundLibrary library;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        musicSources = new AudioSource[2];
        for (int i = 0; i < musicSources.Length; i ++)
        {
            GameObject newMusicSource = new GameObject("Music source " + (i + 1));
            newMusicSource.transform.parent = transform;
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
        }

        GameObject newsfx2DSource = new GameObject("2D sfx source");
        newsfx2DSource.transform.parent = transform;
        sfx2DSource = newsfx2DSource.AddComponent<AudioSource>();

        audioListener = FindObjectOfType<AudioListener>().transform;
        library = GetComponent<SoundLibrary>();

        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
        sfxVolumePercent =  PlayerPrefs.GetFloat("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);

        sceneName = SceneManager.GetActiveScene().name;

        OnSceneLoad();
    }

    private void Start()
    {
        PlayMusic(menuTheme, 2);
    }

    private void Update()
    {
        if (player != null)
        {
            audioListener.position = player.position;
        }

        if (sceneName != SceneManager.GetActiveScene().name)
        {
            OnSceneLoad();

            sceneName = SceneManager.GetActiveScene().name;
            ChangeMusic();

        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].loop = true;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;
        float fadeSpeed = 1 / duration;

        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
        musicSources[1 - activeMusicSourceIndex].Stop();
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }
    
    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    public void OnSceneLoad()
    {
        if (FindObjectOfType<Player>() != null)
        {
            player = FindObjectOfType<Player>().transform;
        }
    }

    private void ChangeMusic()
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu")
        {
            clipToPlay = menuTheme;
        }
        else if (sceneName == "Game")
        {
            clipToPlay = mainTheme;
        }

        if (clipToPlay != null)
        {
            PlayMusic(clipToPlay, 2);
        }
    }

}
