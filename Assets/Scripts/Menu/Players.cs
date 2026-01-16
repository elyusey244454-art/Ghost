using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Players : MonoBehaviour
{
    [SerializeField] private Dropdown _resol; // для выбора количвство героев

    private void Start()
    {
        _resol.ClearOptions();
    }

    private void Update()
    {
        List<string> options = new List<string>()
        {
           "1",
           "2",
           "3",
           "4",
           "5",
           "6",
        };
        _resol.AddOptions(options);
    }  
}
