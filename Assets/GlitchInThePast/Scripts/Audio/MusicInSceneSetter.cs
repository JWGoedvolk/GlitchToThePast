using UnityEngine;

namespace Audio
{
    public class MusicInSceneSetter : MonoBehaviour
    {
        #region Variables
        public AudioClip musicClip;
        public bool loop = true;
        [Range(0f, 3f)] public float fadeSeconds = 0.4f;
        public bool restartIfSame = false;
        #endregion

        void Start()
        {
            if (musicClip is not null)
            {
                AudioManager.Instance?.PlayMusic(musicClip, loop, fadeSeconds, restartIfSame);
            }
        }
    }
}