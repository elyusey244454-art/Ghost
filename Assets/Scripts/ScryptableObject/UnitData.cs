using UnityEngine;

[CreateAssetMenu (fileName = "NewItem" ,menuName ="ScryptebleObject", order = 51)] //Cоздание Scryptable Object
public class UnitData : ScriptableObject
{
    [SerializeField] public GameObject unitPrefab;

    [SerializeField] public string unitName;

    [SerializeField] public float unitSpeed;

    [SerializeField] public int unitHP;

    [SerializeField] public int priceResurces;

    [SerializeField] public int componentResistance;

    public GameObject UnitPrefab()
    { return unitPrefab; }

    public string UnitName()
    { return unitName; }

    public float UnitSpeed() 
    { return unitSpeed;}

    public int UnitHP() 
    { return unitHP;}

    public int PriceResources()
    { return priceResurces;}

    public int ComponentResistance()
    { return componentResistance;}
}
