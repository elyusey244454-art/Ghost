using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scaring : MonoBehaviour
{
    [SerializeField] private Dropdown _resoly; //для выобра сложностей игры

    private void Start()
    {
        _resoly.ClearOptions();
    }

    private void Update()
    {
        List<string> options = new List<string>()
        {
           "Легко",
           "Нормально",
           "Сложно",
        };
        _resoly.AddOptions(options);
    }
}
