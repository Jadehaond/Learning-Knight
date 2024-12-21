using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public class UIElement
    {
        public string elementName;
        public GameObject elementReference;
    }

    private enum UIState { Background, Intro, Levels, Knight, Controls, ControlsInGame, Questions, Results, Resume, Menu }
    private UIState _currentState = UIState.Background;
    private UIState _oldState = UIState.Background;

    [SerializeField] private List<UIElement> uiElements = new List<UIElement>();
    //[SerializeField] private List<UIElement> btnElements = new List<UIElement>();

    private Dictionary<string, GameObject> _uiElementsDict;
    private Dictionary<string, GameObject> _btnElementsDict;

    private Coroutine _fadeCoroutine = null;
    [SerializeField] private TextMeshProUGUI _timerIntroText;
    private Coroutine _startIntroTimerCoroutine;
    private GameManager _instance;
    private GameManager GameManager => _instance ??= GameManager.Instance;

    private void Awake()
    {
        // Initialize dictionaries for faster element lookups
        _uiElementsDict = new Dictionary<string, GameObject>();
        foreach (var uiElement in uiElements)
        {
            _uiElementsDict[uiElement.elementName] = uiElement.elementReference;
        }

        /*_btnElementsDict = new Dictionary<string, GameObject>();
        foreach (var btnElement in btnElements)
        {
            _btnElementsDict[btnElement.elementName] = btnElement.elementReference;
        }*/
    }

    private void Start()
    {
        SetUIState(UIState.Background);
        //_fadeCoroutine = StartCoroutine(DisplayIntro()); //Commenser par l'intro
        SetUIState(UIState.Menu); //Skippe l'intro
    }

    public bool IsCurrentState(string stateName)
    {
        return _currentState.ToString() == stateName;
    }

    private IEnumerator Fade(float fadeTime, bool fadeIn)
    {
        float elapsedTime = 0.0f;
        Image img = uiElements[1].elementReference.GetComponent<Image>();
        TextMeshProUGUI _textResume = uiElements[1].elementReference.GetComponentInChildren<TextMeshProUGUI>();
        SetAlpha(uiElements[1].elementReference, _textResume, 0f);
        TextMeshProUGUI text = _textResume.GetComponent<TextMeshProUGUI>();
        Color img_c = img.color;
        Color text_c = text.color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeIn ? Mathf.Clamp01(elapsedTime / fadeTime) : 1f - Mathf.Clamp01(elapsedTime / fadeTime);
            img_c.a = alpha;
            text_c.a = alpha;
            img.color = img_c;
            text.color = text_c;
            yield return null;
        }
    }

    private IEnumerator StartFadeIn()
    {
        uiElements[1].elementReference.SetActive(true);
        StartFade(3f, true);

        yield return new WaitForSeconds(3f);
    }

    private IEnumerator StartFadeOut()
    {
        StartFade(1f, false);
        yield return new WaitForSeconds(1f);
        
        _fadeCoroutine = null;
        uiElements[1].elementReference.SetActive(false);

        SetUIState(UIState.Menu);
    }

    private IEnumerator DisplayIntro()
    {
        //_currentState = UIState.Intro;
        ShowElement(UIState.Intro.ToString());

        yield return StartFadeIn();
        yield return StartFadeOut();
    }

    private void StartFade(float fadeTime, bool fadeIn)
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(Fade(fadeTime, fadeIn));
    }

    private void SetAlpha(GameObject imageObject, TextMeshProUGUI textObject, float alpha)
    {
        var img = imageObject.GetComponent<Image>();
        var text = textObject.GetComponent<TextMeshProUGUI>();
        if (img != null && text != null)
        {
            Color img_c = img.color;
            Color text_c = text.color;
            img_c.a = alpha;
            text_c.a = alpha;
            img.color = img_c;
            text.color = text_c;
        }
    }

    private void ShowElement(string elementName)
    {
        if (_uiElementsDict.TryGetValue(elementName, out var element))
        {
            element.SetActive(true);
        }
    }

    public void ClearElement(string elementName)
    {
        if (_uiElementsDict.TryGetValue(elementName, out var element))
        {
            element.SetActive(false);
        }
    }

    private void ClearElements()
    {
        foreach (var element in _uiElementsDict.Values)
        {
            if (element.name != "Background") {
                element.SetActive(false);
            }
        }
    }

    private void BtnElementsInGame()
    {
        foreach (var btnElement in _btnElementsDict.Values)
        {
            btnElement.SetActive(_currentState == UIState.Menu);
        }
    }

    private void SetUIState(UIState newState)
    {
        ClearElements();
        _oldState = _currentState;
        _currentState = newState;
        ShowElement(_currentState.ToString());
    }

    public void DisplayMain() => SetUIState(UIState.Background);
    public void DisplayKnight() => SetUIState(UIState.Knight);
    public void DisplayLevels() => SetUIState(UIState.Levels);
    public void DisplayControlsInGame() => SetUIState(UIState.ControlsInGame);
    public void DisplayQuestions() => SetUIState(UIState.Questions);
    public void DisplayResults() => SetUIState(UIState.Results);
    public void DisplayControls() => SetUIState(UIState.Controls);
    public void DisplayResume() => SetUIState(UIState.Resume);
    public void DisplayMenu() => SetUIState(UIState.Menu);

    private void PauseParameters(bool pause)
    {
        GameManager.PauseManager.Pause(pause);
        //GameManager.LevelManager.Knight.GetComponent<CharacterManager>().enabled = !pause;
    }

    public void GoBack()
    {
        SetUIState(_oldState);
    }

    public void DisplayIntroTimer()
    {
        _startIntroTimerCoroutine = StartCoroutine(StartIntroTimer(3f));
    }
 
    private IEnumerator StartIntroTimer(float countdownTime)
    {
        _timerIntroText.transform.parent.gameObject.SetActive(true);
        float remainingTime = countdownTime;

        while (remainingTime >= 0f)
        {
            _timerIntroText.text = remainingTime.ToString();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        _timerIntroText.transform.parent.gameObject.SetActive(false);
        GameManager.LevelManager.ChangeLevelStateByString("Running");
    }
}
