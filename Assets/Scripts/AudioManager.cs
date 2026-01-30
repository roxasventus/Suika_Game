using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum EAudioMixerType { Master, BGM, SFX }
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Source")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Slider")]
    [SerializeField] Slider MasterSlider;
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SFXSlider;

    [Header("BGM")]
    [SerializeField] AudioClip[] BGM_Clips;

    [Header("SFX")]
    [SerializeField] AudioClip[] SFX_Clips;

    private bool[] isMute = new bool[3];
    private float[] audioVolumes = new float[3];
    private void Awake()
    {
        instance = this;
        MasterSlider.onValueChanged.AddListener(value => { ChangeVolume(EAudioMixerType.Master, value); });
        BGMSlider.onValueChanged.AddListener(value => { ChangeVolume(EAudioMixerType.BGM, value); });
        SFXSlider.onValueChanged.AddListener(value => { ChangeVolume(EAudioMixerType.SFX, value); });
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        // 오디오 믹서의 값은 -80 ~ 0까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }

    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType;
        if (!isMute[type]) // 뮤트 
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume;
            SetAudioVolume(audioMixerType, 0.001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }

    public void Mute()
    {
        AudioManager.instance.SetAudioMute(EAudioMixerType.BGM);
    }

    public void ChangeVolume(EAudioMixerType type, float volume)
    {
        AudioManager.instance.SetAudioVolume(type, volume);
    }

    public void SetBGM(string name)
    {
        BGMSource.clip = GetClipByName(BGM_Clips, name);
        BGMSource.Play();
    }

    public void SetSFX(string name)
    {
        SFXSource.clip = GetClipByName(SFX_Clips, name);
        SFXSource.Play();
    }


    public AudioClip GetClipByName(AudioClip[] clips, string clipName)
    {
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] != null && clips[i].name == clipName)
            {
                return clips[i];
            }
        }

        Debug.LogWarning($"AudioClip not found : {clipName}");
        return null;
    }

}



