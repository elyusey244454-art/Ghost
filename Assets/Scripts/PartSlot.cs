using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VehicleConstructor
{
    /// <summary>
    /// UI слот детали - управляет drag-and-drop визуальной модели
    /// SOLID: Single Responsibility - отвечает только за UI взаимодействие
    /// </summary>
    public class PartSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Part Info")]
        [SerializeField] private PartType partType;
        
        [Header("Visual References")]
        [SerializeField] private GameObject visualPrefab; // 3D модель для drag
        [SerializeField] private GameObject partPreview; // Part_preview объект для скрытия
        
        private VehicleConstructorManager constructorManager;
        private GameObject draggedVisual;
        private Camera mainCamera;
        private bool isDragging = false;
        private Vector2 currentPointerPosition;
        private bool isUsed = false; // Флаг, что деталь уже установлена
        private Plane dragPlane; // Кэшируем плоскость для drag

        /// <summary>
        /// Тип детали в этом слоте
        /// </summary>
        public PartType PartType => partType;

        void Awake()
        {
            mainCamera = Camera.main;
            
            // Кэшируем плоскость для drag (Y = 1)
            dragPlane = new Plane(Vector3.up, new Vector3(0, 1, 0));
        }

        void Start()
        {
            // Используем singleton pattern или внедрение зависимости
            constructorManager = VehicleConstructorManager.Instance;
            if (constructorManager == null)
            {
                constructorManager = FindFirstObjectByType<VehicleConstructorManager>();
                Debug.LogWarning("[PartSlot] VehicleConstructorManager найден через FindFirstObjectByType - рекомендуется использовать Singleton");
            }

            // Ищем Part_preview, если не назначен
            if (partPreview == null)
            {
                partPreview = transform.Find("Part_preview")?.gameObject;
                if (partPreview == null)
                {
                    Debug.LogWarning($"[PartSlot] Part_preview не найден у {gameObject.name}");
                }
            }
        }

        /// <summary>
        /// Обработка перемещения указателя (вместо Update)
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && draggedVisual != null && mainCamera != null)
            {
                currentPointerPosition = eventData.position;
                UpdateDragPosition();
            }
        }

        /// <summary>
        /// Нажатие на слот - начало drag
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (constructorManager == null || isUsed) return;

            // Создаём 3D визуал для drag
            if (visualPrefab != null)
            {
                draggedVisual = Instantiate(visualPrefab);
                draggedVisual.transform.localScale = Vector3.one * 0.5f; // Уменьшаем для drag
                isDragging = true;

                // Сохраняем начальную позицию
                currentPointerPosition = eventData.position;

                // Отключаем физику у визуала
                DisablePhysics(draggedVisual);

                // Скрываем Part_preview во время drag
                HidePartPreview();

                // Уведомляем конструктор о начале drag
                constructorManager.OnPartDragStart(partType);
                
                // Сразу обновляем позицию
                UpdateDragPosition();
            }
        }

        /// <summary>
        /// Отпускание - конец drag
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;

            isDragging = false;
            
            // Сохраняем финальную позицию
            currentPointerPosition = eventData.position;

            // Пытаемся активировать деталь
            bool partAttached = false;
            if (constructorManager != null && draggedVisual != null)
            {
                partAttached = constructorManager.OnPartDragEnd(partType, draggedVisual.transform.position);
            }

            // Если деталь успешно установлена - помечаем слот как использованный
            if (partAttached)
            {
                isUsed = true;
            }
            else
            {
                // Если не установлена - показываем Part_preview обратно
                ShowPartPreview();
            }

            // Удаляем визуал drag
            if (draggedVisual != null)
            {
                Destroy(draggedVisual);
                draggedVisual = null;
            }
        }

        /// <summary>
        /// Обновление позиции drag визуала
        /// </summary>
        private void UpdateDragPosition()
        {
            // Raycast от курсора на кэшированную плоскость
            Ray ray = mainCamera.ScreenPointToRay(currentPointerPosition);
            
            if (dragPlane.Raycast(ray, out float distance))
            {
                draggedVisual.transform.position = ray.GetPoint(distance);
            }
        }

        /// <summary>
        /// Отключение физики у визуала
        /// </summary>
        private void DisablePhysics(GameObject obj)
        {
            // Отключаем все Rigidbody
            var rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
            
            var rb2d = obj.GetComponent<Rigidbody2D>();
            if (rb2d != null) rb2d.simulated = false;

            // Отключаем все коллайдеры
            foreach (var col in obj.GetComponentsInChildren<Collider>())
                col.enabled = false;
            
            foreach (var col in obj.GetComponentsInChildren<Collider2D>())
                col.enabled = false;
        }

        /// <summary>
        /// Скрыть Part_preview
        /// </summary>
        private void HidePartPreview()
        {
            if (partPreview != null)
            {
                partPreview.SetActive(false);
            }
        }

        /// <summary>
        /// Показать Part_preview
        /// </summary>
        private void ShowPartPreview()
        {
            if (partPreview != null)
            {
                partPreview.SetActive(true);
            }
        }
    }
}

