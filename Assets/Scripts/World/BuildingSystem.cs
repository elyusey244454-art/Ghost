using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private Structure[] _structureData;
    //Весь скрипт по созданию поставки зданий
   
    [SerializeField] private GameObject _outline;

    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _buildingLayer;

    [SerializeField] private Color _redcolor = Color.red;
    [SerializeField] private Color _greencolor = Color.green;

    [SerializeField] private IBuildable _curentBuilding;
    [SerializeField] private IOutline _curentOutline;

    private PlacementValidator _placementValidator;
    private void Awake()
    {
        _placementValidator = new PlacementValidator(_buildingLayer);
    }
    // методы по реализации поставки зданий

    private void Update()
    {
        if(_curentBuilding != null)
        {
            CheckPlacement();
            TryPlacingBuilding();

            if(Input.GetMouseButtonDown(0))
            {
                MoveWithCursor();
            }

        }
    }

    private void MoveWithCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer)) 

        _outline.transform.position = hit.point;
        _curentOutline?.UpdatePosition(hit.point);
    
    }

    private void CheckPlacement()
    {
        bool canplace = _placementValidator.CanPlace(_curentBuilding.Build.transform.position, _curentBuilding.Size);
        
        _curentOutline?.SetColor(canplace ? _redcolor : _greencolor);
    }

    private void TryPlacingBuilding()
    {
        bool canplace = _placementValidator.CanPlace(_curentBuilding.Build.transform.position, _curentBuilding.Size);
        if(canplace)
        {
            _curentBuilding.Place();
            _curentOutline.Destroy();
            _curentBuilding = null;
            _curentOutline = null;
        }
    }

    public void SelectBuilding(int index)
    {
       if(_curentBuilding  != null)
        {
            Destroy(_curentBuilding.Build);
            _curentOutline.Destroy();
        }

        Structure selectedStructure = _structureData[index];
        GameObject structureInstance = Instantiate(selectedStructure.prefab);
        _curentBuilding = structureInstance.GetComponent<IBuildable>();

        if(_outline != null)
        {
            GameObject outlineInstance = Instantiate(_outline);
            _curentOutline = outlineInstance.GetComponent<IOutline>();
            _curentOutline.UpdatePosition(_curentBuilding.Build.transform.position);
        }
    }
}      