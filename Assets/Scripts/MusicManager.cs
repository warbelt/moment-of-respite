using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] AudioLowPassFilter _lowPassFilter;

    [SerializeField] AudioClip _menuMusic;
    [SerializeField] AudioClip _gameMusic;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_lowPassFilter != null)
        {
            _lowPassFilter.enabled = false;
        }
    }

    public void ActivateMenuMusic()
    {
        StartCoroutine(TransitionMusic(_menuMusic, 0.5f, 0.5f, 0.5f));
    }

    public void ActivateGamePlayMusic()
    {
        StartCoroutine(TransitionMusic(_gameMusic, 0.5f, 0.5f, 0.5f));
    }

    private IEnumerator TransitionMusic(AudioClip clip, float fadeOutTime, float voidTime, float fadeInTime)
    {
        float startVolume = _audioSource.volume;
        float elapsed = 0;

        while (elapsed < fadeOutTime)
        {
            _audioSource.volume = startVolume * (fadeOutTime - elapsed) / fadeOutTime;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _audioSource.volume = 0;
        yield return new WaitForSeconds(voidTime);
        _audioSource.clip = clip;
        _audioSource.Play();

        elapsed = 0;
        while (elapsed < fadeInTime)
        {
            _audioSource.volume = elapsed / fadeInTime;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _audioSource.volume = 1;
    }

    public IEnumerator DimMusic(float fadeOutTime)
    {
        float elapsed = 0;

        while (elapsed < fadeOutTime)
        {
            _audioSource.volume = 1 - 0.5f * elapsed / fadeOutTime;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _audioSource.volume = 0.5f;
        if(_lowPassFilter != null)
        {
            _lowPassFilter.enabled = true;
        }
    }

    public IEnumerator RestoreMusic(float fadeInTime)
    {
        float elapsed = 0;

        while (elapsed < fadeInTime)
        {
            _audioSource.volume = 0.5f + 0.5f * elapsed / fadeInTime;
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _audioSource.volume = 1f;
        if (_lowPassFilter != null)
        {
            _lowPassFilter.enabled = false;
        }
    }
}
