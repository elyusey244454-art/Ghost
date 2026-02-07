using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VehicleConstructor
{
    /// <summary>
    /// Главный менеджер конструктора транспорта
    /// SOLID: 
    /// - Single Responsibility: управление конструктором
    /// - Open/Closed: легко расширяется добавлением новых PartActivator и HintController
    /// - Dependency Inversion: зависит от абстракций (компонентов), а не конкретных классов
    /// Паттерн: Singleton для глобального доступа
    /// </summary>
    public class VehicleConstructorManager : MonoBehaviour
    {
        // Singleton instance
        private static VehicleConstructorManager instance;
        public static VehicleConstructorManager Instance => instance;

        [Header("References")]
        [SerializeField] private Transform vehicleRoot; // Корень транспорта
        
        [Header("Settings")]
        [SerializeField] private float activationRadius = 2f; // Радиус для активации детали
        [SerializeField] private float vehicleLiftHeight = 0.5f; // На сколько поднять транспорт при установке колёс
        [SerializeField] private bool autoLiftOnFirstWheel = true; // Автоматически поднимать при первом колесе
        
        // Кэш всех активаторов и подсказок
        private Dictionary<PartType, List<PartActivator>> partActivators = new Dictionary<PartType, List<PartActivator>>();
        private Dictionary<PartType, List<HintController>> hintsByType = new Dictionary<PartType, List<HintController>>();
        private List<HintController> allHints = new List<HintController>();
        private Dictionary<HintController, PartActivator> hintToActivatorCache = new Dictionary<HintController, PartActivator>(); // Кэш связей hint ↔ activator
        private Dictionary<PartActivator, HintController> activatorToHintCache = new Dictionary<PartActivator, HintController>(); // Кэш связей activator ↔ hint
        
        private PartType? currentDraggingType = null;
        private bool vehicleLifted = false; // Поднят ли уже транспорт
        private Vector3 originalVehiclePosition; // Исходная позиция транспорта

        void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Debug.LogWarning("[VehicleConstructor] Обнаружен дубликат VehicleConstructorManager! Уничтожаем...");
                Destroy(gameObject);
                return;
            }
            instance = this;

            // Сохраняем исходную позицию транспорта
            if (vehicleRoot != null)
            {
                originalVehiclePosition = vehicleRoot.position;
            }
            
            InitializeComponents();
        }

        void OnDestroy()
        {
            // Очищаем singleton при уничтожении
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Инициализация всех компонентов (SOLID: Interface Segregation)
        /// </summary>
        private void InitializeComponents()
        {
            if (vehicleRoot == null)
            {
                Debug.LogError("[VehicleConstructor] vehicleRoot не установлен!");
                return;
            }

            // Находим все активаторы деталей
            var activators = vehicleRoot.GetComponentsInChildren<PartActivator>(true);
            foreach (var activator in activators)
            {
                if (!partActivators.ContainsKey(activator.PartType))
                {
                    partActivators[activator.PartType] = new List<PartActivator>();
                }
                partActivators[activator.PartType].Add(activator);
                Debug.Log($"[VehicleConstructor] Найден активатор: {activator.PartType}");
            }

            // Находим все подсказки
            allHints = vehicleRoot.GetComponentsInChildren<HintController>(true).ToList();
            foreach (var hint in allHints)
            {
                if (!hintsByType.ContainsKey(hint.CompatiblePartType))
                {
                    hintsByType[hint.CompatiblePartType] = new List<HintController>();
                }
                hintsByType[hint.CompatiblePartType].Add(hint);
                Debug.Log($"[VehicleConstructor] Найдена подсказка для: {hint.CompatiblePartType}");
            }

            // Создаём кэш связей hint ↔ activator (один раз при инициализации)
            BuildHintActivatorCache();

            Debug.Log($"[VehicleConstructor] Инициализация завершена. Активаторов: {activators.Length}, Подсказок: {allHints.Count}");
        }

        /// <summary>
        /// Создание кэша связей между hints и activators (оптимизация)
        /// </summary>
        private void BuildHintActivatorCache()
        {
            foreach (var hint in allHints)
            {
                // Ищем связанный activator один раз
                PartActivator activator = FindActivatorForHint(hint);
                if (activator != null)
                {
                    hintToActivatorCache[hint] = activator;
                    activatorToHintCache[activator] = hint;
                }
            }
        }

        /// <summary>
        /// Поиск activator для hint (вызывается один раз при инициализации)
        /// </summary>
        private PartActivator FindActivatorForHint(HintController hint)
        {
            // 1. У родителя
            var activator = hint.GetComponentInParent<PartActivator>();
            
            // 2. У siblings
            if (activator == null && hint.transform.parent != null)
            {
                activator = hint.transform.parent.GetComponentInChildren<PartActivator>();
            }
            
            // 3. На самом hint
            if (activator == null)
            {
                activator = hint.GetComponent<PartActivator>();
            }
            
            return activator;
        }

        /// <summary>
        /// Начало перетаскивания детали
        /// </summary>
        public void OnPartDragStart(PartType type)
        {
            currentDraggingType = type;
            
            // Скрываем все подсказки
            HideAllHints();
            
            // Показываем только совместимые подсказки
            ShowCompatibleHints(type);
        }

        /// <summary>
        /// Конец перетаскивания детали
        /// </summary>
        public bool OnPartDragEnd(PartType type, Vector3 dropPosition)
        {
            currentDraggingType = null;
            
            // Пытаемся активировать ближайшую неактивную деталь этого типа
            bool activated = TryActivateNearestPart(type, dropPosition);
            
            if (!activated)
            {
                // Показываем все подсказки снова, так как деталь не установлена
                ShowAllHints();
            }
            
            return activated;
        }

        /// <summary>
        /// Попытка активировать ближайшую деталь
        /// </summary>
        private bool TryActivateNearestPart(PartType type, Vector3 position)
        {
            if (!partActivators.ContainsKey(type))
            {
                Debug.LogWarning($"[VehicleConstructor] Нет активаторов для {type}");
                return false;
            }

            // Находим ближайший неактивный активатор
            PartActivator nearest = null;
            float minDistance = float.MaxValue;

            foreach (var activator in partActivators[type])
            {
                if (!activator.IsActivated)
                {
                    float distance = Vector3.Distance(activator.transform.position, position);
                    
                    if (distance < minDistance && distance <= activationRadius)
                    {
                        minDistance = distance;
                        nearest = activator;
                    }
                }
            }

            // Активируем найденную деталь
            if (nearest != null)
            {
                nearest.Activate();
                
                // Скрываем подсказку для этой детали
                HideHintForActivator(nearest);
                
                // Поднимаем транспорт при установке первого колеса
                if (autoLiftOnFirstWheel && !vehicleLifted && IsWheelType(type))
                {
                    LiftVehicle();
                }
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Скрыть подсказку для активированной детали
        /// </summary>
        private void HideHintForActivator(PartActivator activator)
        {
            // Используем кэш вместо множественных GetComponent
            if (activatorToHintCache.TryGetValue(activator, out HintController hint))
            {
                hint.Hide();
            }
        }

        /// <summary>
        /// Показать все подсказки
        /// </summary>
        private void ShowAllHints()
        {
            foreach (var hint in allHints)
            {
                // Используем кэш вместо GetComponent
                if (hintToActivatorCache.TryGetValue(hint, out PartActivator parentActivator))
                {
                    if (!parentActivator.IsActivated)
                    {
                        hint.Show();
                    }
                    else
                    {
                        hint.Hide();
                    }
                }
                else
                {
                    // Если нет связанного activator - показываем hint
                    hint.Show();
                }
            }
        }

        /// <summary>
        /// Скрыть все подсказки
        /// </summary>
        private void HideAllHints()
        {
            foreach (var hint in allHints)
            {
                hint.Hide();
            }
        }

        /// <summary>
        /// Показать только совместимые подсказки
        /// </summary>
        private void ShowCompatibleHints(PartType type)
        {
            if (hintsByType.ContainsKey(type))
            {
                foreach (var hint in hintsByType[type])
                {
                    // Используем кэш вместо GetComponent
                    if (hintToActivatorCache.TryGetValue(hint, out PartActivator parentActivator))
                    {
                        if (!parentActivator.IsActivated)
                        {
                            hint.Show();
                        }
                    }
                    else
                    {
                        // Если нет связанного activator - показываем hint
                        hint.Show();
                    }
                }
            }
        }

        /// <summary>
        /// Проверка, все ли обязательные детали активированы
        /// </summary>
        public bool IsVehicleComplete()
        {
            // Проверяем, есть ли хотя бы одно активированное колесо каждого типа
            bool hasLeftWheel = false;
            bool hasRightWheel = false;

            if (partActivators.ContainsKey(PartType.Wheel))
            {
                int activeWheels = partActivators[PartType.Wheel].Count(a => a.IsActivated);
                hasLeftWheel = activeWheels >= 1;
                hasRightWheel = activeWheels >= 2;
            }

            return hasLeftWheel && hasRightWheel;
        }

        /// <summary>
        /// Проверка, является ли тип детали колесом
        /// </summary>
        private bool IsWheelType(PartType type)
        {
            return type == PartType.Wheel || type == PartType.SpikedWheel;
        }

        /// <summary>
        /// Поднять транспорт для установки колёс
        /// </summary>
        private void LiftVehicle()
        {
            if (vehicleRoot == null || vehicleLifted) return;

            Vector3 newPosition = originalVehiclePosition + Vector3.up * vehicleLiftHeight;
            vehicleRoot.position = newPosition;
            vehicleLifted = true;
        }

        /// <summary>
        /// Опустить транспорт обратно (если нужно)
        /// </summary>
        public void LowerVehicle()
        {
            if (vehicleRoot == null || !vehicleLifted) return;

            vehicleRoot.position = originalVehiclePosition;
            vehicleLifted = false;
        }
    }
}



