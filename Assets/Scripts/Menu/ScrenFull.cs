using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrenFull : MonoBehaviour //Делаем скрипт для изменения разрешения экрана на dropdown
{
    [SerializeField] private Dropdown _resolution;

    private void Start()
    {
        _resolution.ClearOptions();
    }

    private void Update()
    {
        List<string> options = new List<string>()
        {
           "1280:720",
           "1366:768",
           "1600:900",
           "1920:1080",
        };

        _resolution.AddOptions(options);
        _resolution.onValueChanged.AddListener(SetResolution);
        SetResolution(_resolution.value);
    }


    private void SetResolution(int index)
    {
        switch(index)
        {
            case 0:
              Screen.SetResolution(1280, 720, Screen.fullScreen);
                    break;
            case 1:
                Screen.SetResolution(1366, 768, Screen.fullScreen);
                break;

            case 2:
                Screen.SetResolution(1600, 900, Screen.fullScreen);
                break;
            case 3:
               Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
        }
    }
        
}
