using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Управляет отображением доступных деталей в UI
    /// </summary>
    public class PartHolder : MonoBehaviour
    {
        [Header("Настройки")]
        [Tooltip("Количество слотов для деталей")]
        public int slotCount = 3;
        
        [Tooltip("Префаб слота детали")]
        public GameObject partSlotPrefab;
        
        [Header("Все доступные детали")]
        [Tooltip("Список всех доступных деталей")]
        public PartData[] allParts;
        
        [Header("Ссылки")]
        [Tooltip("Контейнер для слотов деталей")]
        public Transform slotsContainer;
        
        [Tooltip("Ссылка на CarBuilder")]
        public CarBuilder carBuilder;
        
        private List<PartSlot> currentSlots = new List<PartSlot>();
        
        /// <summary>
        /// Генерирует случайные детали для отображения
        /// </summary>
        public void GenerateRandomParts()
        {
            // Очищаем старые слоты
            ClearSlots();
            
            // Выбираем случайные детали
            List<PartData> selectedParts = new List<PartData>();
            List<PartData> availableParts = new List<PartData>(allParts);
            
            for (int i = 0; i < slotCount && availableParts.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availableParts.Count);
                selectedParts.Add(availableParts[randomIndex]);
                availableParts.RemoveAt(randomIndex);
            }
            
            // Создаем слоты для выбранных деталей
            foreach (PartData part in selectedParts)
            {
                CreatePartSlot(part);
            }
        }
        
        /// <summary>
        /// Создает слот для детали
        /// </summary>
        private void CreatePartSlot(PartData partData)
        {
            if (partSlotPrefab == null || slotsContainer == null)
            {
                Debug.LogError("PartSlotPrefab или SlotsContainer не назначены!");
                return;
            }
            
            GameObject slotObj = Instantiate(partSlotPrefab, slotsContainer);
            PartSlot slot = slotObj.GetComponent<PartSlot>();
            
            if (slot == null)
            {
                slot = slotObj.AddComponent<PartSlot>();
            }
            
            slot.Initialize(partData, carBuilder);
            currentSlots.Add(slot);
        }
        
        /// <summary>
        /// Очищает все слоты
        /// </summary>
        private void ClearSlots()
        {
            foreach (PartSlot slot in currentSlots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            currentSlots.Clear();
        }
        
        private void Start()
        {
            // Автоматически генерируем детали при старте
            if (allParts.Length > 0)
            {
                GenerateRandomParts();
            }
        }
    }
}

