using UnityEngine;
using Audio;

public class EnemyDetectorForAudioSwitch : MonoBehaviour
{
    #region Variables
    [Header("Audio Clips")]
    public AudioClip calmClip;
    public AudioClip tenseClip;

    [Header("Enemy Detection")]
    public LayerMask enemyLayers;
    public float radius = 8f;
    public int enterCombatCount = 1;
    public int exitCombatCount = 0;
    public float pollInterval = 0.25f;

    [Tooltip("Should triggers hitCount as enemies?")]
    public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

    [Header("Audio Behaviour")]
    [Range(0f, 2f)] public float minSecondsBetweenSwitches = 0.35f;

    [Tooltip("Re-evaluate whether we should change the music or not.")]
    public bool evaluateInitialState = true;

    [Tooltip("If set to true the detector will only change music on state changes.")]
    public bool respectSceneMusic = true;

    private bool inCombat;
    private float lastSwitchTime;
    private bool running;

    private const int MaxHits = 64;
    private readonly Collider[] _hits = new Collider[MaxHits];
    #endregion

    private void OnEnable()
    {
        running = true;
        if (evaluateInitialState) EvaluateAndMaybweSwitchInitial();
        StartCoroutine(PollLoop());
    }

    private void OnDisable()
    {
        running = false;
        StopAllCoroutines();
    }

    #region Polling
    private System.Collections.IEnumerator PollLoop()
    {
        var wait = new WaitForSecondsRealtime(pollInterval);
        while (running)
        {
            PollOnce();
            yield return wait;
        }
    }
    #endregion

    #region Private Functions
    private void EvaluateAndMaybweSwitchInitial()
    {
        int initial = Physics.OverlapSphereNonAlloc(
            transform.position, radius, _hits, enemyLayers, triggerInteraction);

        bool desiredCombat = initial >= enterCombatCount;

        var audioManagerScript = AudioManager.Instance;
        var currentClip = audioManagerScript != null && audioManagerScript.Music != null ? audioManagerScript.Music.clip : null;
        var desiredClip = desiredCombat ? tenseClip : calmClip;

        bool shouldSwitchNow = desiredClip != null && (currentClip != desiredClip) && (!respectSceneMusic || (respectSceneMusic && currentClip != null));

        inCombat = desiredCombat;

        if (shouldSwitchNow)
        {
            Play(desiredClip);
            lastSwitchTime = Time.unscaledTime;
        }
    }

    private void PollOnce()
    {
        var mgr = AudioManager.Instance;
        if (mgr == null || mgr.Music == null)
        {
            Debug.LogWarning("AudioManager.Instance or Music source missing. If AudioManager is missing, make sure you are testing the game from the main menu onwards.");
            return;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, radius, _hits, enemyLayers, triggerInteraction);


        if (Time.unscaledTime - lastSwitchTime < minSecondsBetweenSwitches)
            return;

        if (!inCombat && hitCount >= enterCombatCount)
        {
            inCombat = true;
            if (tenseClip != null)
            {
                Play(tenseClip);
                lastSwitchTime = Time.unscaledTime;
            }
            else
            {
                Debug.LogWarning("Tense clip isn't assigned.");
            }
            return;
        }

        if (inCombat && hitCount <= exitCombatCount)
        {
            inCombat = false;
            if (calmClip != null)
            {
                Play(calmClip);
                lastSwitchTime = Time.unscaledTime;
            }
            else
            {
                Debug.LogWarning("Calm clip isn't assigned");
            }
        }
    }

    private static void Play(AudioClip clip)
    {
        AudioManager.Instance?.PlayMusic(clip, loop: true, fadeSeconds: 0.3f, restartIfSame: false);
    }
#endregion
}