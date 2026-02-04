using UnityEngine;
using UnityEngine.UI;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Компонент для подсветки точек крепления в UI
    /// </summary>
    public class AttachmentPointHighlighter : MonoBehaviour
    {
        [Header("Настройки")]
        [Tooltip("Точка крепления, которую подсвечиваем")]
        public AttachmentPoint attachmentPoint;
        
        [Tooltip("Изображение подсветки (dotted_square)")]
        public Image highlightImage;
        
        [Tooltip("Спрайт для подсветки")]
        public Sprite highlightSprite;
        
        private Camera mainCamera;
        private Canvas uiCanvas;
        
        private void Start()
        {
            mainCamera = Camera.main;
            uiCanvas = FindObjectOfType<Canvas>();
            
            // Создаем изображение подсветки, если его нет
            if (highlightImage == null)
            {
                CreateHighlightImage();
            }
            
            // Скрываем подсветку по умолчанию
            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Создает изображение подсветки
        /// </summary>
        private void CreateHighlightImage()
        {
            if (uiCanvas == null || highlightSprite == null) return;
            
            GameObject highlightObj = new GameObject("Highlight");
            highlightObj.transform.SetParent(uiCanvas.transform, false);
            
            highlightImage = highlightObj.AddComponent<Image>();
            highlightImage.sprite = highlightSprite;
            highlightImage.color = new Color(1f, 1f, 1f, 0.7f);
            highlightImage.raycastTarget = true;
            
            RectTransform rectTransform = highlightObj.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100f, 100f);
        }
        
        /// <summary>
        /// Показывает подсветку
        /// </summary>
        public void ShowHighlight()
        {
            if (highlightImage == null || attachmentPoint == null) return;
            
            highlightImage.gameObject.SetActive(true);
            UpdateHighlightPosition();
        }
        
        /// <summary>
        /// Скрывает подсветку
        /// </summary>
        public void HideHighlight()
        {
            if (highlightImage != null)
            {
                highlightImage.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// Обновляет позицию подсветки в UI
        /// </summary>
        private void UpdateHighlightPosition()
        {
            if (mainCamera == null || uiCanvas == null || attachmentPoint == null) return;
            
            // Преобразуем мировую позицию точки крепления в экранные координаты
            Vector3 worldPos = attachmentPoint.transform.position;
            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            
            // Проверяем, находится ли точка перед камерой
            if (mainCamera.WorldToScreenPoint(worldPos).z < 0)
            {
                highlightImage.gameObject.SetActive(false);
                return;
            }
            
            // Преобразуем экранные координаты в локальные координаты Canvas
            RectTransform canvasRect = uiCanvas.transform as RectTransform;
            Vector2 localPoint;
            
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                uiCanvas.worldCamera != null ? uiCanvas.worldCamera : null,
                out localPoint))
            {
                highlightImage.rectTransform.anchoredPosition = localPoint;
            }
        }
        
        private void Update()
        {
            // Обновляем позицию подсветки, если она активна
            if (highlightImage != null && highlightImage.gameObject.activeSelf)
            {
                UpdateHighlightPosition();
            }
        }
    }
}

