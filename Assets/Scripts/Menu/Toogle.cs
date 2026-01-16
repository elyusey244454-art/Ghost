using UnityEngine;
using UnityEngine.UI;

public class Toogle : MonoBehaviour //Делаем тоглы с условиями
{
    [SerializeField] private Toggle _toggle;

    [SerializeField] private Text _toggleText;
    
    private void Start()
    {
        _toggle.onValueChanged.AddListener(OnToggleValueChanget);
    }

    private void OnToggleValueChanget(bool ison)
    {
        if (ison)
            _toggleText.text = "ON";
        else
            _toggleText.text = "OFF";
    }
}
