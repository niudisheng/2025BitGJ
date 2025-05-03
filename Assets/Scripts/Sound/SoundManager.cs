using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource musicSource;
    public AudioSource effectSource;
    public AudioClip[] musicClips;

    public AudioClip[] effectClips;

    // 音频源池
    // 音频源的ID作为字典的键，AudioSource作为字典的值
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    // 音频剪辑池
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保SoundManager在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 加载音频剪辑（一次性）
    public void LoadClip(string clipName, AudioClip clip)
    {
        if (!audioClips.ContainsKey(clipName))
        {
            audioClips.Add(clipName, clip);
        }
    }
    
    private AudioSource CreateAudioSource(string sourceName = "")
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        if (string.IsNullOrEmpty(sourceName))
        {
            // 创建新的AudioSource
            audioSources.Add(newSource.GetInstanceID().ToString(), newSource);
        }
        else
        {
            audioSources.Add(sourceName, newSource);
        }

        return newSource;
    }

    private AudioSource GetAudioSource(string sourceName)
    {
        if (audioSources.ContainsKey(sourceName))
        {
            // 找到已有的AudioSource
            return audioSources[sourceName];
        }
        else
        {
            return CreateAudioSource(sourceName);
        }
    }
    
    // 播放音效,并指定音频源
    public void PlaySound(AudioClip clip, bool loop = false, float pitch = 1f, float volume = 1f,
        string sourceName = "")
    {
        AudioSource source = GetAudioSource(sourceName);
        source.clip = clip;
        source.loop = loop;
        source.pitch = pitch;
        source.volume = volume;
        source.Play();
        // StartCoroutine(CheckAudioPlayback(source));
    }

    public void PlaySound(Sound sound)
    {
        PlaySound(sound.clip, sound.loop, sound.getRandomPitch(), sound.volume, sourceName: sound.AudioSource);
    }

    public void PlaySoundGroup(SoundGroup soundGroup)
    {
        int index = Random.Range(0, soundGroup.clips.Length);
        AudioClip clip = soundGroup.clips[index];
        Debug.Log("Play sound group: " + soundGroup.name + " index: " + index + " clip: " + clip.name);
        PlaySound(clip, soundGroup.loop, soundGroup.getRandomPitch(), soundGroup.volume,
            sourceName: soundGroup.AudioSource);
    }

    // 停止播放指定的音频
    public void StopSound(string clipName)
    {
        foreach (var source in audioSources.Values)
        {
            if (source.clip != null && source.clip.name == clipName && source.isPlaying)
            {
                source.Stop();
            }
        }
    }

    // 清理所有音频源
    public void StopAllSounds()
    {
        foreach (var source in audioSources.Values)
        {
            source.Stop();
        }
    }
    private void LateUpdate()
    {
        // 每两秒检查一次音频源状态
        CheckAndDestroyAudioSources();
    }

    private float checkInterval = 2f; // 检查间隔
    private float checkTimer = 0f;

    private void CheckAndDestroyAudioSources()
    {
        checkTimer += Time.deltaTime;
        if (checkTimer < checkInterval) return; // 未到检查时间，直接返回

        checkTimer = 0f; // 重置计时器

        List<string> keysToRemove = new List<string>();

        foreach (var kvp in audioSources)
        {
            AudioSource source = kvp.Value;

            // 销毁不用的音频源
            if (source != null && !source.isPlaying && !source.loop)
            {
                keysToRemove.Add(kvp.Key); // 标记为需要销毁
                Destroy(source);
            }
        }

        // 从字典中移除已销毁的音频源
        foreach (var key in keysToRemove)
        {
            audioSources.Remove(key);
        }
    }
}