using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public const string MASTER_VOLUME_PARAMETER_NAME = "MasterVolume";
    public const string MUSIC_VOLUME_PARAMETER_NAME = "MusicVolume";
    public const string SFX_VOLUME_PARAMETER_NAME = "SFXVolume";
    public const float MUTED_VOLUME_LEVEL = -80f;

    private const string SFX_PARENT_NAME = "SFX";

    public static char[] SFX_NAME_FORMAT_CONTAINERS = new char[] { '[', ']' };
    private static string SFX_NAME_FORMAT = $"SFX - {SFX_NAME_FORMAT_CONTAINERS[0]}" + "{0}" + $"{SFX_NAME_FORMAT_CONTAINERS[1]}";

    public const float TRACK_TRANSITION_SPEED = 1f;

    public static AudioManager Instance { get; private set; }

    public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup masterMixer;

    [Header("3D Audio Settings")]
    [SerializeField] private float _audio3DMinDistance = 1f;
    [SerializeField] private float _audio3DMaxDistance = 50f;

    private AnimationCurve audioFalloffCurve;
    public float minDecibels = -60f;
    [Tooltip("When no curve is assigned, use Mathf.Pow(slider, volumeExponent). Values <1 boost mid/high; >1 make top less sensitive.")]
    [Range(0.2f, 3f)]
    public float volumeExponent = 0.5f;

    // Map a 0..1 slider value to a perceptual volume value (0..1).
    // If an `audioFalloffCurve` is provided in the inspector, use it; otherwise apply a configurable power curve.
    private float MapSliderToPerceptual(float sliderValue) {
        if (audioFalloffCurve != null) {
            return Mathf.Clamp01(audioFalloffCurve.Evaluate(sliderValue));
        }

        // Use configurable power mapping (default 0.5 => sqrt) so mid slider positions remain audible.
        return Mathf.Clamp01(Mathf.Pow(Mathf.Clamp01(sliderValue), volumeExponent));
    }

    private Transform sfxRoot;

    public AudioSource[] allSFX => sfxRoot.GetComponentsInChildren<AudioSource>();

    private void Awake() {
        if (Instance == null) {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else {
            DestroyImmediate(gameObject);
            return;
        }

        // Inicializa a curva de falloff padrão se não estiver atribuída
        if (audioFalloffCurve == null) {
            audioFalloffCurve = new AnimationCurve(
                new Keyframe(0f, 0f, 0f, 2f),
                new Keyframe(0.5f, 0.5f, 1f, 1f),
                new Keyframe(1f, 1f, 2f, 0f)
            );
        }

        sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
        sfxRoot.SetParent(transform);
    }

    public AudioSource PlaySound(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, bool is3D = true) {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null) {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory!");
            return null;
        }

        return PlaySound(clip, mixer, volume, pitch, loop, filePath, is3D);
    }

    public AudioSource PlaySound(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, string filePath = "", bool is3D = false) {
        string fileName = clip.name;
        if (filePath != string.Empty)
            fileName = filePath;

        AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, fileName)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = is3D ? 1f : 0f;
        effectSource.pitch = pitch;
        effectSource.loop = loop;

        // Configurar propriedades 3D se ativado
        if (is3D) {
            effectSource.minDistance = _audio3DMinDistance;
            effectSource.maxDistance = _audio3DMaxDistance;
            effectSource.dopplerLevel = 1f;
        }

        effectSource.Play();

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    // Voice playback removed from AudioManager. Use PlaySoundEffect / PlayTrack and provide mixer if needed.

    public void StopSound(AudioClip clip) => StopSound(clip.name);

    public void StopSound(string soundName) {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources) {
            if (source.clip.name.ToLower() == soundName) {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public bool IsPlayingSoundEffect(string soundName) {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources) {
            if (source.clip.name.ToLower() == soundName)
                return true;
        }

        return false;
    }

    public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f) {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null) {
            Debug.LogError($"Could not load audio file '{filePath}'. Please make sure this exists in the Resources directory!");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "") {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
        return track;
    }

    public void StopTrack(int channel) {
        AudioChannel c = TryGetChannel(channel, createIfDoesNotExist: false);

        if (c == null)
            return;

        c.StopTrack();
    }

    public void StopTrack(string trackName) {
        trackName = trackName.ToLower();

        foreach (var channel in channels.Values) {
            if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName) {
                channel.StopTrack();
                return;
            }
        }
    }

    public void StopAllTracks() {
        foreach (AudioChannel channel in channels.Values) {
            channel.StopTrack();
        }
    }

    public void StopAllSoundEffects() {
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach (var source in sources) {
            Destroy(source.gameObject);
        }
    }

    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false) {
        AudioChannel channel = null;

        if (channels.TryGetValue(channelNumber, out channel)) {
            return channel;
        }
        else if (createIfDoesNotExist) {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }

        return null;
    }

    public void SetMusicVolume(float volume, bool muted) {
        if (muted) {
            musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, MUTED_VOLUME_LEVEL);
            return;
        }

        float mapped = MapSliderToPerceptual(volume);
        float db = Mathf.Lerp(minDecibels, 0f, mapped);
        musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, db);
    }

    public void SetSFXVolume(float volume, bool muted) {
        if (muted) {
            sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, MUTED_VOLUME_LEVEL);
            return;
        }

        float mapped = MapSliderToPerceptual(volume);
        float db = Mathf.Lerp(minDecibels, 0f, mapped);
        sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, db);
    }


    public void SetMasterVolume(float volume, bool muted) {
        if (muted) {
            masterMixer.audioMixer.SetFloat(MASTER_VOLUME_PARAMETER_NAME, MUTED_VOLUME_LEVEL);
            return;
        }

        float mapped = MapSliderToPerceptual(volume);
        float db = Mathf.Lerp(minDecibels, 0f, mapped);
        masterMixer.audioMixer.SetFloat(MASTER_VOLUME_PARAMETER_NAME, db);
    }
}
