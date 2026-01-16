using UnityEngine;
using UnityEngine.UI;

public class Ui : MonoBehaviour //скрипт для создание перехода по кнопкам и показывать подсказки при наведении на мышь
{
    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject _setings;
    [SerializeField] private GameObject _hint;

    [SerializeField] private Text _hintText;

    private void Start()
    {
        ShowPanel(_buttons);
        _hint.SetActive(false);
    }
    private void ShowPanel(GameObject panelToShow)
    {
        _buttons.SetActive(false);
        _setings.SetActive(false);
        panelToShow.SetActive(true);
    }

    public void ShowHint(string message)
    {
        _hintText.text = message;
        _hint.SetActive(true);
    }

    public void ShowButtons() => ShowPanel(_buttons);
    public void ShowSetings() => ShowPanel(_setings);
    public void CloseApp() => Application.Quit();
    public void HideHint() => _hint.SetActive(false);
}
