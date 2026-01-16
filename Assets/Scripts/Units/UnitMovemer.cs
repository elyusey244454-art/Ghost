using UnityEngine;

public class UnitMovemer : MonoBehaviour //Делаю крипт на движение юнита 
{
    public UnitData[] _stick; //Адаптирование под Scryptable Obkect

    private Vector3 _targetposition;
    private void Start()
    {
        _targetposition = transform.position; //Преобразуем позицию таргета к трансформу
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))//Условие при нажатии на кнопку 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
                _targetposition = hit.point;
        }

        MoveUnit();
        RotateUnit();
    }

    public void MoveUnit()
    {
       UnitData selectedData = _stick[0];
        transform.position = Vector3.MoveTowards(transform.position, _targetposition, selectedData.unitSpeed * Time.deltaTime);
    }

    public void RotateUnit()
    {
        Vector3 direction = _targetposition - transform.position;
        direction.y = 0;
        if(direction.magnitude > 0.1f)
        {
           Quaternion rotation = Quaternion.LookRotation( - direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
           
        }
    }
}
