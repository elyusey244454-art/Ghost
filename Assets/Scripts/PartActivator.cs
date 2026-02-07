using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Активатор детали - управляет состоянием детали на транспорте
    /// SOLID: Single Responsibility - отвечает только за активацию/деактивацию
    /// </summary>
    public class PartActivator : MonoBehaviour
    {
        [Header("Part Info")]
        [SerializeField] private PartType partType;
        
        [Header("References")]
        [SerializeField] private GameObject visualObject; // 3D модель детали
        
        private bool isActivated = false;

        /// <summary>
        /// Тип детали (только для чтения)
        /// </summary>
        public PartType PartType => partType;
        
        /// <summary>
        /// Активирована ли деталь
        /// </summary>
        public bool IsActivated => isActivated;

        void Awake()
        {
            // Если visualObject не указан, используем весь GameObject
            if (visualObject == null)
            {
                visualObject = gameObject;
            }
            
            // По умолчанию деталь неактивна
            Deactivate();
        }

        /// <summary>
        /// Активировать деталь (включить визуал)
        /// </summary>
        public void Activate()
        {
            if (!isActivated)
            {
                isActivated = true;
                visualObject.SetActive(true);
                Debug.Log($"[PartActivator] {partType} активирована");
            }
        }

        /// <summary>
        /// Деактивировать деталь (выключить визуал)
        /// </summary>
        public void Deactivate()
        {
            isActivated = false;
            visualObject.SetActive(false);
        }
    }
}



