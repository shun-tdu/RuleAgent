using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I { get; private set; }
    [Header("UI SFX")] [SerializeField] private AudioSource templateSource;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    [SerializeField, Tooltip("同時に再生できるAudioSourceの最大数")]
    private int maxSource = 8;

    private class PooledAudioSource
    {
        public AudioSource source;
        public bool inUse;
    }

    private List<PooledAudioSource> _pool = new List<PooledAudioSource>();

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        templateSource.gameObject.SetActive(false);
    }

    private AudioSource GetAvailableSource()
    {
        foreach (var item in _pool)
            if (!item.inUse)
            {
                item.inUse = true;
                return item.source;
            }

        //最大再生を超える場合は音を再生しない
        if (_pool.Count >= maxSource)
        {
            return null;
        }

        //プール切れ→テンプレートを複製
        var go = Instantiate(templateSource.gameObject, transform);
        go.SetActive(true);
        var newSrc = go.GetComponent<AudioSource>();
        var pooled = new PooledAudioSource { source = newSrc, inUse = true };
        _pool.Add(pooled);
        return newSrc;
    }

    /// <summary>
    /// clipをoffsetSeconds秒だけシークして再生
    /// </summary>
    public void PlayUISfx(AudioClip clip, float startOffsetSec, float endOffsetSec, float volume = 1f)
    {
        if (!clip) return;
        var src = GetAvailableSource();
        if (src == null) return;

        src.clip = clip;
        src.volume = Mathf.Clamp01(volume);
        src.time = Mathf.Clamp(startOffsetSec, 0f, clip.length);
        src.Play();
        StartCoroutine(DeactiveAfterPlay(src, endOffsetSec - startOffsetSec));
    }

    private IEnumerator DeactiveAfterPlay(AudioSource src, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (src != null)
        {
            src.Stop();
            src.clip = null;

            foreach (var item in _pool)
            {
                if (item.source == src)
                {
                    item.inUse = false;
                    break;
                }
            }
        }
    }

    public void PlayHover(float startOffsetSec, float endOffsetSec) =>
        PlayUISfx(hoverClip, startOffsetSec, endOffsetSec, 0.05f);

    public void PlayClick(float startOffsetSec, float endOffsetSec) =>
        PlayUISfx(clickClip, startOffsetSec, endOffsetSec, 0.2f);
}