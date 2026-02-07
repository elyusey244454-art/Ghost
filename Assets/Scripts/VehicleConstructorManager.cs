using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VehicleConstructor
{
    /// <summary>
    /// Главный менеджер конструктора транспорта
    /// SOLID: Single Responsibility - управление конструктором
    /// Упрощенная версия без избыточного кэширования
    /// </summary>
    public class VehicleConstructorManager : MonoBehaviour
    {
        // Singleton
        public static VehicleConstructorManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform vehicleRoot;

        [Header("Settings")]
        [SerializeField] private float activationRadius = 2f;
        [SerializeField] private float vehicleLiftHeight = 0.5f;

        // Простые списки компонентов
        private List<PartActivator> allActivators = new List<PartActivator>();
        private List<HintController> allHints = new List<HintController>();
        
        private bool vehicleLifted = false;
        private Vector3 originalVehiclePosition;

        void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (vehicleRoot != null)
            {
                originalVehiclePosition = vehicleRoot.position;
            }
            
            Initialize();
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Инициализация - собираем все компоненты
        /// </summary>
        private void Initialize()
        {
            if (vehicleRoot == null)
            {
                Debug.LogError("[VehicleConstructor] vehicleRoot не установлен!");
                return;
            }

            allActivators = vehicleRoot.GetComponentsInChildren<PartActivator>(true).ToList();
            allHints = vehicleRoot.GetComponentsInChildren<HintController>(true).ToList();

            Debug.Log($"[VehicleConstructor] Найдено: {allActivators.Count} активаторов, {allHints.Count} подсказок");
        }

        /// <summary>
        /// Начало перетаскивания - показываем совместимые hints
        /// </summary>
        public void OnPartDragStart(PartType type)
        {
            foreach (var hint in allHints)
            {
                // Показываем только совместимые и неактивированные
                if (hint.CompatiblePartType == type && !IsHintActivatorActive(hint))
                {
                    hint.Show();
                }
                else
                {
                    hint.Hide();
                }
            }
        }

        /// <summary>
        /// Конец перетаскивания - пытаемся активировать деталь
        /// </summary>
        public bool OnPartDragEnd(PartType type, Vector3 dropPosition)
        {
            bool activated = TryActivatePart(type, dropPosition);
            
            if (!activated)
            {
                // Не удалось - показываем все hints обратно
                ShowAllInactiveHints();
            }
            
            return activated;
        }

        /// <summary>
        /// Попытка активировать ближайшую деталь
        /// </summary>
        private bool TryActivatePart(PartType type, Vector3 position)
        {
            PartActivator nearest = FindNearestInactivePart(type, position);
            
            if (nearest != null)
            {
                nearest.Activate();
                HideHintForPart(nearest);
                
                // Поднимаем транспорт при первом колесе
                if (!vehicleLifted && IsWheelType(type))
                {
                    LiftVehicle();
                }
                
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Найти ближайшую неактивную деталь
        /// </summary>
        private PartActivator FindNearestInactivePart(PartType type, Vector3 position)
        {
            PartActivator nearest = null;
            float minDistance = activationRadius;

            foreach (var activator in allActivators)
            {
                if (activator.PartType == type && !activator.IsActivated)
                {
                    float distance = Vector3.Distance(activator.transform.position, position);
                    
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = activator;
                    }
                }
            }

            return nearest;
        }

        /// <summary>
        /// Скрыть hint для активированной детали
        /// </summary>
        private void HideHintForPart(PartActivator activator)
        {
            foreach (var hint in allHints)
            {
                if (hint.CompatiblePartType == activator.PartType && 
                    hint.transform.IsChildOf(activator.transform.parent))
                {
                    hint.Hide();
                    break;
                }
            }
        }

        /// <summary>
        /// Показать все hints для неактивированных деталей
        /// </summary>
        private void ShowAllInactiveHints()
        {
            foreach (var hint in allHints)
            {
                if (!IsHintActivatorActive(hint))
                {
                    hint.Show();
                }
            }
        }

        /// <summary>
        /// Проверить, активирован ли activator для данного hint
        /// </summary>
        private bool IsHintActivatorActive(HintController hint)
        {
            // Ищем activator того же типа рядом с hint
            foreach (var activator in allActivators)
            {
                if (activator.PartType == hint.CompatiblePartType && 
                    hint.transform.IsChildOf(activator.transform.parent) &&
                    activator.IsActivated)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Проверка типа колеса
        /// </summary>
        private bool IsWheelType(PartType type)
        {
            return type == PartType.Wheel || type == PartType.SpikedWheel;
        }

        /// <summary>
        /// Поднять транспорт
        /// </summary>
        private void LiftVehicle()
        {
            if (vehicleRoot == null || vehicleLifted) return;

            vehicleRoot.position = originalVehiclePosition + Vector3.up * vehicleLiftHeight;
            vehicleLifted = true;
        }

        /// <summary>
        /// Опустить транспорт
        /// </summary>
        public void LowerVehicle()
        {
            if (vehicleRoot == null || !vehicleLifted) return;

            vehicleRoot.position = originalVehiclePosition;
            vehicleLifted = false;
        }

        /// <summary>
        /// Проверка готовности транспорта
        /// </summary>
        public bool IsVehicleComplete()
        {
            int activeWheels = allActivators.Count(a => a.PartType == PartType.Wheel && a.IsActivated);
            return activeWheels >= 2;
        }
    }
}
