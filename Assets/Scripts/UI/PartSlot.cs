using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Слот детали в UI с поддержкой drag and drop
    /// </summary>
    public class PartSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("UI элементы")]
        [Tooltip("Изображение иконки детали")]
        public Image iconImage;
        
        private PartData partData;
        private CarBuilder carBuilder;
        private GameObject dragObject;
        private Canvas canvas;
        
        /// <summary>
        /// Инициализирует слот детали
        /// </summary>
        public void Initialize(PartData data, CarBuilder builder)
        {
            partData = data;
            carBuilder = builder;
            
            // Находим Canvas
            canvas = GetComponentInParent<Canvas>();
            
            // Устанавливаем иконку
            if (iconImage != null && data.partIcon != null)
            {
                iconImage.sprite = data.partIcon;
            }
        }
        
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (partData == null || carBuilder == null) return;
            
            // Показываем подсветку точек крепления
            if (carBuilder != null)
            {
                carBuilder.ShowHighlightForPartType(partData.partType);
            }
            
            // Создаем визуальный объект для перетаскивания
            CreateDragVisual();
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (dragObject != null && canvas != null)
            {
                // Обновляем позицию перетаскиваемого объекта
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    eventData.position,
                    canvas.worldCamera,
                    out Vector2 localPoint);
                
                dragObject.transform.position = canvas.transform.TransformPoint(localPoint);
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            // Убираем подсветку
            if (carBuilder != null)
            {
                carBuilder.HideAllHighlights();
            }
            
            // Удаляем визуальный объект
            if (dragObject != null)
            {
                Destroy(dragObject);
            }
            
            // Проверяем, попали ли мы на точку крепления
            CheckDropOnAttachmentPoint(eventData);
        }
        
        /// <summary>
        /// Создает визуальный объект для перетаскивания
        /// </summary>
        private void CreateDragVisual()
        {
            if (iconImage == null || canvas == null) return;
            
            dragObject = new GameObject("DragPart");
            dragObject.transform.SetParent(canvas.transform, false);
            
            Image dragImage = dragObject.AddComponent<Image>();
            dragImage.sprite = iconImage.sprite;
            dragImage.raycastTarget = false;
            
            RectTransform rectTransform = dragObject.GetComponent<RectTransform>();
            rectTransform.sizeDelta = iconImage.rectTransform.sizeDelta;
            rectTransform.position = iconImage.transform.position;
        }
        
        /// <summary>
        /// Проверяет, была ли деталь сброшена на точку крепления
        /// </summary>
        private void CheckDropOnAttachmentPoint(PointerEventData eventData)
        {
            // Используем Raycast для проверки попадания на точку крепления
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (RaycastResult result in results)
            {
                AttachmentPointHighlighter highlighter = result.gameObject.GetComponent<AttachmentPointHighlighter>();
                if (highlighter != null && highlighter.attachmentPoint != null)
                {
                    // Проверяем совместимость типов
                    if (highlighter.attachmentPoint.partType == partData.partType)
                    {
                        // Прикрепляем деталь
                        AttachPartToPoint(highlighter.attachmentPoint);
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// Прикрепляет деталь к точке крепления
        /// </summary>
        private void AttachPartToPoint(AttachmentPoint point)
        {
            if (carBuilder == null || partData == null || point == null) return;
            
            // Прикрепляем деталь в зависимости от типа
            switch (partData.partType)
            {
                case AttachmentPoint.PartType.Wheel:
                    if (point == carBuilder.frontWheelPoint)
                    {
                        carBuilder.AttachFrontWheel(partData.wheelIndex);
                    }
                    else if (point == carBuilder.backWheelPoint)
                    {
                        carBuilder.AttachBackWheel(partData.wheelIndex);
                    }
                    break;
                    
                case AttachmentPoint.PartType.Propeller:
                    if (point == carBuilder.propellerPoint)
                    {
                        carBuilder.AttachPropeller();
                    }
                    break;
                    
                case AttachmentPoint.PartType.Wing:
                    if (point == carBuilder.topAttachmentPoint)
                    {
                        carBuilder.AttachWings();
                    }
                    break;
                    
                case AttachmentPoint.PartType.Rocket:
                    if (point == carBuilder.topAttachmentPoint)
                    {
                        carBuilder.AttachRocket();
                    }
                    break;
            }
            
            // Убираем подсветку после прикрепления
            carBuilder.HideAllHighlights();
        }
    }
}

