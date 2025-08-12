using UnityEngine;

public class EnemyDetectorForAudioSwitch : MonoBehaviour
{
    public AdaptiveMusicController music;
    public LayerMask enemyLayers;
    public float radius = 18f;
    public int enterCombatCount = 1;
    public int exitCombatCount = 0;
    public float pollInterval = 0.25f;

    private int _currentCount;
    private bool _inCombat;

    void Awake()
    {
        if (music == null) music = AdaptiveMusicController.Instance;
    }

    private void OnEnable()
    {
        if (music == null)
        {
            StartCoroutine(WaitForMusicThenStart());
            return;
        }

        music.EnsureRunning();
        Poll();

        InvokeRepeating(nameof(Poll), 0.1f, pollInterval);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Poll));
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator WaitForMusicThenStart()
    {
        float timeout = 2f;
        float timee = 0f;

        while (music == null && timee < timeout)
        {
            music = AdaptiveMusicController.Instance;
            if (music != null) break;
            timee += Time.unscaledDeltaTime;
            yield return null;
        }

        if (music != null)
        {
            music.EnsureRunning();
            Poll(); // first evaluation
            InvokeRepeating(nameof(Poll), 0.1f, pollInterval);
        }
    }

    private void Poll()
    {
        if (music == null) return;

        _currentCount = Physics.OverlapSphere(transform.position, radius, enemyLayers).Length;

        if (!_inCombat && _currentCount >= enterCombatCount)
        {
            _inCombat = true;
            music.SetCombat(true);
            return;
        }

        if (_inCombat && _currentCount <= exitCombatCount)
        {
            _inCombat = false;
            music.SetCombat(false);
        }
    }
}