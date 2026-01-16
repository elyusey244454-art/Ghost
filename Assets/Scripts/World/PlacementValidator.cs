using UnityEngine;
using UnityEngine.UIElements;

public class PlacementValidator : MonoBehaviour
{
    public readonly LayerMask _buildinglayer;

    public PlacementValidator(LayerMask buildinglayer)
    {  
        _buildinglayer = buildinglayer; 
    }

    public bool CanPlace(Vector3 position, Vector3 size)
    {
        Collider[] coliders = Physics.OverlapBox(position, size / 2, Quaternion.identity, _buildinglayer);
        return coliders.Length == 0;
    }
}
