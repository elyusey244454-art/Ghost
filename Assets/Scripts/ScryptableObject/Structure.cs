using UnityEngine;

[CreateAssetMenu(fileName = "new building", menuName = "ScryptableObject Building", order = 52)]
public class Structure : ScriptableObject //Адаптируем здания по scryptable object
{
    [SerializeField] public GameObject prefab;

    [SerializeField] public string structurePrice;
    
    public GameObject Prefab()
      { return prefab; }

    public string StructurePrice()
    { return structurePrice; }
}
