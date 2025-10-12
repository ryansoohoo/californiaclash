using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // ===== Singleton =====
    public static SoundManager I { get; private set; }
    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        BuildEnemyLookup();
        BuildSfxPool();
        StartCoroutine(CreepyLoop());
    }

    // ===== Enums you can reference from game code =====
    public enum EnemyType { None, EnemyA, EnemyB, EnemyC, Boss }
    public enum RoundOutcome { Win, Lose, Tie }
    public enum GameOutcome { Win, Lose }

    // ===== Mixer routing (optional but recommended) =====
    [Header("Mixer Routing")]
    public AudioMixerGroup bgmMixer;
    public AudioMixerGroup sfxMixer;

    // ===== BGM (crossfade) =====
    [Header("Background Music")]
    public AudioClip defaultMusic;
    [Range(0f, 5f)] public float bgmFadeSeconds = 1.0f;

    [System.Serializable]
    public struct EnemyMusic
    {
        public EnemyType enemy;
        public AudioClip clip;
        [Tooltip("Higher number = higher priority if multiple enemies are active.")]
        public int priority;
    }
    public List<EnemyMusic> enemyMusicTable = new List<EnemyMusic>();

    private Dictionary<EnemyType, EnemyMusic> enemyMusicLookup;
    private HashSet<EnemyType> activeEnemies = new HashSet<EnemyType>();

    private AudioSource bgmA, bgmB;   // two sources for smooth crossfades
    private AudioSource currentBgmSrc, nextBgmSrc;
    private Coroutine bgmFadeCo;

    // ===== SFX (one-shots with small pool) =====
    [Header("Round SFX")]
    public AudioClip sfxRoundWin;
    public AudioClip sfxRoundLose;
    public AudioClip sfxRoundTie;

    [Header("Game SFX")]
    public AudioClip sfxGameWin;
    public AudioClip sfxGameLose;

    [Header("SFX Settings")]
    [Range(1, 32)] public int sfxPoolSize = 10;
    [Tooltip("Small random pitch variance to add life to repeated SFX.")]
    [Range(0f, 0.5f)] public float sfxPitchJitter = 0.05f;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private int sfxIndex = 0;

    // ===== Creepy randomly timed stingers =====
    [Header("Creepy Stingers")]
    public List<AudioClip> creepyClips = new List<AudioClip>();
    [Tooltip("Seconds between creepy stingers (min, max).")]
    public Vector2 creepyIntervalRange = new Vector2(20f, 60f);
    [Range(0f, 1f)] public float creepyVolume = 0.6f;
    public bool creepyEnabled = true;

    // ===== Initialization helpers =====
    void BuildEnemyLookup()
    {
        enemyMusicLookup = new Dictionary<EnemyType, EnemyMusic>();
        foreach (var item in enemyMusicTable)
            if (!enemyMusicLookup.ContainsKey(item.enemy))
                enemyMusicLookup.Add(item.enemy, item);

        // Build & prime BGM sources
        bgmA = gameObject.AddComponent<AudioSource>();
        bgmB = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { bgmA, bgmB })
        {
            s.loop = true;
            s.playOnAwake = false;
            s.outputAudioMixerGroup = bgmMixer;
            s.volume = 0f;
        }
        currentBgmSrc = bgmA;
        nextBgmSrc = bgmB;

        // Start default music (if provided)
        if (defaultMusic != null)
        {
            currentBgmSrc.clip = defaultMusic;
            currentBgmSrc.volume = 1f;
            currentBgmSrc.Play();
        }
    }

    void BuildSfxPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var a = gameObject.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.loop = false;
            a.outputAudioMixerGroup = sfxMixer;
            sfxPool.Add(a);
        }
    }

    AudioSource NextSfxSource()
    {
        // Round-robin; no allocation; allows overlapping SFX.
        var a = sfxPool[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxPool.Count;
        return a;
    }

    // ===== Public API: Enemy-driven BGM =====
    public void RegisterEnemy(EnemyType enemy)
    {
        if (enemy == EnemyType.None) return;
        if (activeEnemies.Add(enemy))
            RefreshBgmForActiveEnemies();
    }

    public void UnregisterEnemy(EnemyType enemy)
    {
        if (enemy == EnemyType.None) return;
        if (activeEnemies.Remove(enemy))
            RefreshBgmForActiveEnemies();
    }

    void RefreshBgmForActiveEnemies()
    {
        AudioClip target = defaultMusic;
        int bestPriority = int.MinValue;

        foreach (var e in activeEnemies)
        {
            if (enemyMusicLookup.TryGetValue(e, out var em) && em.clip != null)
            {
                if (em.priority > bestPriority)
                {
                    bestPriority = em.priority;
                    target = em.clip;
                }
            }
        }

        if (currentBgmSrc.clip == target) return;
        CrossfadeTo(target);
    }

    // ===== Public API: Round & Game outcome SFX =====
    public void PlayRoundOutcome(RoundOutcome outcome)
    {
        switch (outcome)
        {
            case RoundOutcome.Win:  PlaySfx(sfxRoundWin);  break;
            case RoundOutcome.Lose: PlaySfx(sfxRoundLose); break;
            case RoundOutcome.Tie:  PlaySfx(sfxRoundTie);  break;
        }
    }

    public void PlayGameOutcome(GameOutcome outcome)
    {
        switch (outcome)
        {
            case GameOutcome.Win:  PlaySfx(sfxGameWin);  break;
            case GameOutcome.Lose: PlaySfx(sfxGameLose); break;
        }
    }

    // ===== Public API: Trigger a creepy stinger on demand =====
    public void PlayCreepyNow()
    {
        if (creepyClips == null || creepyClips.Count == 0) return;
        var clip = creepyClips[Random.Range(0, creepyClips.Count)];
        PlaySfx(clip, creepyVolume);
    }

    // ===== Internals =====
    void CrossfadeTo(AudioClip nextClip)
    {
        if (nextClip == null) return;

        // swap
        var old = currentBgmSrc;
        var incoming = nextBgmSrc;

        incoming.clip = nextClip;
        incoming.time = 0f;
        incoming.volume = 0f;
        incoming.Play();

        // next swap targets
        currentBgmSrc = incoming;
        nextBgmSrc = old;

        if (bgmFadeCo != null) StopCoroutine(bgmFadeCo);
        bgmFadeCo = StartCoroutine(CrossfadeRoutine(incoming, old, bgmFadeSeconds));
    }

    IEnumerator CrossfadeRoutine(AudioSource fadeIn, AudioSource fadeOut, float seconds)
    {
        if (seconds <= 0.0001f)
        {
            fadeOut.Stop();
            fadeOut.volume = 0f;
            fadeIn.volume = 1f;
            yield break;
        }

        float t = 0f;
        float startOut = fadeOut.volume;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // ignore timescale for UI/menus
            float k = Mathf.Clamp01(t / seconds);
            fadeIn.volume = k;
            fadeOut.volume = Mathf.Lerp(startOut, 0f, k);
            yield return null;
        }
        fadeOut.Stop();
        fadeOut.volume = 0f;
        fadeIn.volume = 1f;
    }

    void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        var a = NextSfxSource();
        a.pitch = 1f + Random.Range(-sfxPitchJitter, sfxPitchJitter);
        a.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    IEnumerator CreepyLoop()
    {
        while (true)
        {
            if (!creepyEnabled || creepyClips == null || creepyClips.Count == 0)
            {
                yield return null;
                continue;
            }

            float wait = Random.Range(creepyIntervalRange.x, creepyIntervalRange.y);
            yield return new WaitForSeconds(wait);

            // Small chance to skip to avoid predictability
            if (Random.value < 0.15f) continue;

            PlayCreepyNow();
        }
    }

    // ===== Convenience: manual BGM control if you ever need it =====
    public void PlayDefaultMusic() => CrossfadeTo(defaultMusic);
    public void PlayEnemyMusic(EnemyType enemy)
    {
        if (enemyMusicLookup.TryGetValue(enemy, out var em) && em.clip != null)
            CrossfadeTo(em.clip);
    }
}
