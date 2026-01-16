using UnityEngine;

public class Building : MonoBehaviour, IBuildable //делаем слой для поствки зданий
{
    public GameObject Build => gameObject; 
    
    public Vector3 Size => GetComponent<BoxCollider>().bounds.size;

    public void Place()
    {
        Build.layer = LayerMask.NameToLayer("Building");
    }    
}
