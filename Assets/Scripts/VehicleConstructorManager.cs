using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace VehicleConstructor
{
    /// <summary>
    /// Главный менеджер конструктора транспорта
    /// SOLID: 
    /// - Open/Closed: легко расширяется добавлением новых PartActivator и HintController
    /// - Dependency Inversion: зависит от абстракций (компонентов), а не конкретных классов
    /// </summary>
    public class VehicleConstructorManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform vehicleRoot; // Корень транспорта
        
        [Header("Settings")]
        [SerializeField] private float activationRadius = 2f; // Радиус для активации детали
        
        // Кэш всех активаторов и подсказок
        private Dictionary<PartType, List<PartActivator>> partActivators = new Dictionary<PartType, List<PartActivator>>();
        private Dictionary<PartType, List<HintController>> hintsByType = new Dictionary<PartType, List<HintController>>();
        private List<HintController> allHints = new List<HintController>();
        
        private PartType? currentDraggingType = null;

        void Awake()
        {
            InitializeComponents();
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

            Debug.Log($"[VehicleConstructor] Инициализация завершена. Активаторов: {activators.Length}, Подсказок: {allHints.Count}");
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
            
            Debug.Log($"[VehicleConstructor] Drag начат для {type}");
        }

        /// <summary>
        /// Конец перетаскивания детали
        /// </summary>
        public void OnPartDragEnd(PartType type, Vector3 dropPosition)
        {
            currentDraggingType = null;
            
            // Пытаемся активировать ближайшую неактивную деталь этого типа
            bool activated = TryActivateNearestPart(type, dropPosition);
            
            if (activated)
            {
                Debug.Log($"[VehicleConstructor] Деталь {type} активирована!");
            }
            else
            {
                Debug.Log($"[VehicleConstructor] Не удалось активировать {type}");
            }
            
            // Показываем все подсказки снова
            ShowAllHints();
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
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Скрыть подсказку для активированной детали
        /// </summary>
        private void HideHintForActivator(PartActivator activator)
        {
            // Находим подсказку, которая является дочерним объектом активатора
            var hint = activator.GetComponentInChildren<HintController>();
            if (hint != null)
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
                // Показываем только подсказки для неактивированных деталей
                var parentActivator = hint.GetComponentInParent<PartActivator>();
                if (parentActivator == null || !parentActivator.IsActivated)
                {
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
                    // Показываем только если деталь ещё не активирована
                    var parentActivator = hint.GetComponentInParent<PartActivator>();
                    if (parentActivator == null || !parentActivator.IsActivated)
                    {
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

        #if UNITY_EDITOR
        /// <summary>
        /// Визуализация радиуса активации в редакторе
        /// </summary>
        void OnDrawGizmosSelected()
        {
            if (partActivators == null) return;

            Gizmos.color = Color.yellow;
            foreach (var activatorList in partActivators.Values)
            {
                foreach (var activator in activatorList)
                {
                    if (!activator.IsActivated)
                    {
                        Gizmos.DrawWireSphere(activator.transform.position, activationRadius);
                    }
                }
            }
        }
        #endif
    }
}


