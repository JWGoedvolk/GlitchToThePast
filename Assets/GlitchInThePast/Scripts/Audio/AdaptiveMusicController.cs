using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AdaptiveMusicController : MonoBehaviour
{
    public static AdaptiveMusicController Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource calmSource;
    public AudioSource tenseSource;

    [Header("Clips")]
    public AudioClip calmClip;
    public AudioClip tenseClip;

    [Header("Mixer")]
    public AudioMixer mixer;
    public AudioMixerSnapshot calmSnapshot;
    public AudioMixerSnapshot tenseSnapshot;
    [Tooltip("Seconds for crossfade between snapshots")]
    public float snapshotFadeSeconds = 1.5f;

    [Header("Beat Sync (optional)")]
    public bool barSyncTransitions = true;
    public double bpm = 120.0;
    public int beatsPerBar = 4;

    [Header("Lifecycle")]
    public bool dontDestroyOnLoad = true;
    public bool autoStart = false;

    double _dspStart;
    bool _started;
    bool _inCombat;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"Duplicate {nameof(AdaptiveMusicController)} found on {name}. Destroying this one.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (calmSnapshot == null || tenseSnapshot == null)
        {
            Debug.LogError("AdaptiveMusicController: Missing mixer snapshots.");
            return;
        }

        if (autoStart)
        {
            Activate();
        }
    }

    public void ConfigureClips(AudioClip calm, AudioClip tense)
    {
        if (calm != null) calmClip = calm;
        if (tense != null) tenseClip = tense;

        if (calmSource != null && calmSource.clip != calmClip) calmSource.clip = calmClip;
        if (tenseSource != null && tenseSource.clip != tenseClip) tenseSource.clip = tenseClip;
    }

    public void Activate()
    {
        if (_started) return;

        if (calmSource == null || tenseSource == null)
        {
            Debug.LogError("AdaptiveMusicController: Missing AudioSources.");
            return;
        }

        if (calmSource.clip == null) calmSource.clip = calmClip;
        if (tenseSource.clip == null) tenseSource.clip = tenseClip;

        if (calmSource.clip == null || tenseSource.clip == null)
        {
            Debug.LogError("AdaptiveMusicController: Missing clips. Assign calmClip and tenseClip or call ConfigureClips.");
            return;
        }

        calmSource.loop = true;
        tenseSource.loop = true;

        _dspStart = AudioSettings.dspTime + 0.05;
        calmSource.PlayScheduled(_dspStart);
        tenseSource.PlayScheduled(_dspStart);

        calmSnapshot.TransitionTo(0.01f);
        _started = true;
    }

    public void Deactivate()
    {
        if (!_started) return;
        calmSource.Stop();
        tenseSource.Stop();
        _started = false;
        _inCombat = false;
    }

    public void SetCombat(bool inCombat)
    {
        if (!_started || _inCombat == inCombat) return;
        _inCombat = inCombat;

        if (barSyncTransitions)
        {
            StopCoroutine(nameof(TransitionOnNextBar));
            StartCoroutine(TransitionOnNextBar(inCombat));
        }
        else
        {
            (inCombat ? tenseSnapshot : calmSnapshot).TransitionTo(snapshotFadeSeconds);
        }
    }
    public void EnsureRunning()
    {
        if (!_started) Activate();
    }

    IEnumerator TransitionOnNextBar(bool toCombat)
    {
        double secondsPerBeat = 60.0 / bpm;
        double secondsPerBar = secondsPerBeat * beatsPerBar;

        double now = AudioSettings.dspTime;
        double elapsed = now - _dspStart;
        double barsElapsed = elapsed / secondsPerBar;
        double nextBarIndex = System.Math.Floor(barsElapsed + 1.0);
        double nextBarTime = _dspStart + nextBarIndex * secondsPerBar;

        while (AudioSettings.dspTime + 0.001 < nextBarTime)
            yield return null;

        (toCombat ? tenseSnapshot : calmSnapshot).TransitionTo(snapshotFadeSeconds);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}