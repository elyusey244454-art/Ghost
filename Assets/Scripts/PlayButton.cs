using UnityEngine;
using UnityEngine.UI;

namespace VehicleConstructor
{
    /// <summary>
    /// Управление кнопкой Play
    /// SOLID: Single Responsibility - отвечает только за UI кнопки запуска
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class PlayButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private VehicleConstructorManager constructorManager;
        [SerializeField] private AutoVehicleMovement vehicleMovement;
        [SerializeField] private GameObject partHolder; // PartHolder для проверки активных Part_preview
        [SerializeField] private CameraFollow cameraFollow; // Камера для следования за транспортом

        [Header("Settings")]
        [SerializeField] private bool hideOnStart = true; // Скрывать кнопку при старте
        [SerializeField] private float checkInterval = 0.5f; // Интервал проверки в секундах

        private Button button;
        private CanvasGroup canvasGroup;
        private float nextCheckTime = 0f;
        private bool gameStarted = false; // Флаг, что игра уже началась

        void Awake()
        {
            button = GetComponent<Button>();
            
            // Получаем или добавляем CanvasGroup для плавного показа/скрытия
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            // Подписываемся на событие нажатия кнопки
            if (button != null)
            {
                button.onClick.AddListener(OnPlayButtonClicked);
            }
        }

        void Start()
        {
            // Ищем компоненты, если не назначены (используем Singleton где возможно)
            if (constructorManager == null)
            {
                constructorManager = VehicleConstructorManager.Instance ?? FindObjectOfType<VehicleConstructorManager>();
            }

            if (vehicleMovement == null)
            {
                vehicleMovement = FindObjectOfType<AutoVehicleMovement>();
            }

            if (partHolder == null)
            {
                partHolder = GameObject.Find("PartHolder");
            }

            if (cameraFollow == null)
            {
                cameraFollow = FindObjectOfType<CameraFollow>();
            }

            // Скрываем кнопку при старте
            if (hideOnStart)
            {
                HideButton();
            }
        }

        void Update()
        {
            // Если игра уже началась - больше не проверяем
            if (gameStarted) return;

            // Проверяем только раз в checkInterval секунд
            if (Time.time < nextCheckTime) return;
            nextCheckTime = Time.time + checkInterval;

            // Проверяем, все ли детали установлены
            if (!IsButtonVisible() && AreAllPartsPlaced())
            {
                ShowButton();
            }
        }

        /// <summary>
        /// Проверка, видна ли кнопка
        /// </summary>
        private bool IsButtonVisible()
        {
            return button != null && button.interactable;
        }

        /// <summary>
        /// Проверка, все ли детали установлены (все Part_preview скрыты)
        /// </summary>
        private bool AreAllPartsPlaced()
        {
            if (partHolder == null) return false;

            // Получаем все PartSlot в PartHolder
            PartSlot[] partSlots = partHolder.GetComponentsInChildren<PartSlot>(true);

            if (partSlots.Length == 0) return false;

            // Проверяем, все ли Part_preview скрыты (детали установлены)
            foreach (var slot in partSlots)
            {
                // Ищем Part_preview внутри слота (несколько вариантов имени)
                Transform previewTransform = slot.transform.Find("Part_preview");
                
                // Если не найден, пробуем найти по другому имени
                if (previewTransform == null)
                {
                    // Ищем любой дочерний объект с "preview" в имени (игнорируя регистр)
                    foreach (Transform child in slot.transform)
                    {
                        if (child.name.ToLower().Contains("preview"))
                        {
                            previewTransform = child;
                            break;
                        }
                    }
                }
                
                if (previewTransform != null && previewTransform.gameObject.activeSelf)
                {
                    // Если хотя бы один Part_preview активен - не все детали установлены
                    return false;
                }
            }

            // Все Part_preview скрыты - все детали установлены!
            return true;
        }

        /// <summary>
        /// Обработка нажатия на кнопку Play
        /// </summary>
        private void OnPlayButtonClicked()
        {
            // Помечаем, что игра началась
            gameStarted = true;

            // Запускаем движение транспорта
            if (vehicleMovement != null)
            {
                vehicleMovement.StartMoving();
            }

            // Включаем следование камеры
            if (cameraFollow != null)
            {
                cameraFollow.StartFollowing();
            }

            // Скрываем кнопку после нажатия
            HideButton();
        }

        /// <summary>
        /// Показать кнопку
        /// </summary>
        private void ShowButton()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            button.interactable = true;
        }

        /// <summary>
        /// Скрыть кнопку
        /// </summary>
        private void HideButton()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            button.interactable = false;
        }

        void OnDestroy()
        {
            // Отписываемся от события
            if (button != null)
            {
                button.onClick.RemoveListener(OnPlayButtonClicked);
            }
        }
    }
}

