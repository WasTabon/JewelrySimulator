using UnityEngine;
using UnityEngine.UI;

public class MusicController : MonoBehaviour
{
    [SerializeField] private Image _musicButtonIcon;
    [SerializeField] private Image _soundButtonIcon;

    [SerializeField] private Sprite _soundButtonIconOn;
    [SerializeField] private Sprite _soundButtonIconOff;

    [SerializeField] private AudioSource _audioSourceMusic;
    [SerializeField] private AudioSource _audioSourceSound;

    [SerializeField] private AudioClip _uiClickSound;

    private bool _isMusicOn;
    private bool _isSoundOn;

    public void PlayClickSound()
    {
        _audioSourceSound.PlayOneShot(_uiClickSound);
    }

    public void PlaySpecificSound(AudioClip audioClip)
    {
        _audioSourceSound.PlayOneShot(audioClip);
    }

    public void HandleMusicOn()
    {
        if (_isMusicOn)
        {
            _isMusicOn = false;
            _audioSourceMusic.mute = true;
            _musicButtonIcon.sprite = _soundButtonIconOn;
        }
        else
        {
            _isMusicOn = true;
            _audioSourceMusic.mute = false;
            _musicButtonIcon.sprite = _soundButtonIconOff;
        }
    }
    public void HandleSoundOn()
    {
        if (_isMusicOn)
        {
            _isSoundOn = false;
            _audioSourceSound.mute = true;
            _soundButtonIcon.sprite = _soundButtonIconOn;
        }
        else
        {
            _isSoundOn = true;
            _audioSourceSound.mute = false;
            _soundButtonIcon.sprite = _soundButtonIconOff;
        }
    }
}
