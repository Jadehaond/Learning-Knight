using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] _soundtrack;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private GameObject _audioObject;
    [SerializeField, Range(0f,1f)] private float _volume = 0.2f;
    private AudioSource _audioSource;

    public float audioVolume {
        get
        {
            return PlayerPrefs.GetFloat(".audioVolume");
        }
        set
        {
            PlayerPrefs.SetFloat(".audioVolume", _volume);
        }
    }
    
    void Awake()
    {
        DontDestroyOnLoad(_audioObject.gameObject);
    }

    void Start()
    {
    	
        _audioSource = _audioObject.GetComponent<AudioSource>();

        if (!_audioSource.playOnAwake)
        {
            _audioSource.clip = _soundtrack[Random.Range(0, _soundtrack.Length)];
            _audioSource.Play();
        }
    }

    void Update()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = _soundtrack[Random.Range(0, _soundtrack.Length)];
            _audioSource.Play();
        }
    }

    void OnEnable()
    {
        _volumeSlider.onValueChanged.AddListener(delegate { changeVolume(_volumeSlider.value); });
    }

    void changeVolume(float sliderValue)
    {
    	_volume = sliderValue;
        _audioSource.volume = sliderValue;
    }

    void OnDisable()
    {
        _volumeSlider.onValueChanged.RemoveAllListeners();
    }
}
