using UnityEngine;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Данные о детали для UI
    /// </summary>
    [System.Serializable]
    public class PartData
    {
        [Tooltip("Название детали")]
        public string partName;
        
        [Tooltip("Тип детали")]
        public AttachmentPoint.PartType partType;
        
        [Tooltip("Префаб детали")]
        public GameObject partPrefab;
        
        [Tooltip("Иконка для UI")]
        public Sprite partIcon;
        
        [Tooltip("Индекс колеса (если это колесо: 0 = Simple Wheel, 1 = Spiked Wheel)")]
        public int wheelIndex = -1;
    }
}

