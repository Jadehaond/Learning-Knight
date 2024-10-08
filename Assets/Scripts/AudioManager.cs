using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _soundtrack;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField, Range(0f, 1f)] private float _volume = 0.2f;
    private AudioSource _audioSource;

    [SerializeField] private Button _musicToggleButton;
    [SerializeField] private Sprite _musicOnSprite;
    [SerializeField] private Sprite _musicOffSprite;

    public float audioVolume
    {
        get { return PlayerPrefs.GetFloat(".audioVolume", _volume); }
        set { PlayerPrefs.SetFloat(".audioVolume", value); }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _audioSource = this.GetComponent<AudioSource>();
        Debug.Log("AudioSource initialized: " + _audioSource);
    }

    void Start()
    {
        if (!_audioSource.playOnAwake)
        {
            _audioSource.clip = _soundtrack[Random.Range(0, _soundtrack.Length)];
            _audioSource.Play();
        }

        _audioSource.volume = _volume;
        _volumeSlider.value = _volume;
    }

    void OnEnable()
    {
        _volumeSlider.onValueChanged.AddListener(delegate { changeVolume(_volumeSlider.value); });
    }

    private void OnDisable()
    {
        if (_volumeSlider != null)
        {
            _volumeSlider.onValueChanged.RemoveAllListeners();
        }
    }

    void changeVolume(float sliderValue)
    {
        _volume = sliderValue;
        _audioSource.volume = sliderValue;
        audioVolume = sliderValue;
    }

    public void ToggleMusic()
    {
        Debug.Log("AudioSource currently being controlled: " + _audioSource.clip.name);
        if (_audioSource == null) {
           Debug.LogWarning("AudioSource is null. Trying to find the AudioSource component again.");
            _audioSource = this.GetComponent<AudioSource>();
        }

        Debug.Log(_musicToggleButton);
        Debug.Log(_musicOffSprite);
        Debug.Log(_musicOnSprite);

        if (_audioSource.isPlaying)
        {
            Debug.Log("pause");
            _musicToggleButton.image.sprite = _musicOffSprite;
            _audioSource.Pause();
        }
        else
        {
            Debug.Log("play");
            _musicToggleButton.image.sprite = _musicOnSprite;
            _audioSource.Play();
        }
    }

    void OnDestroy()
    {
        Debug.Log("AudioManager destroyed");
    }
}
