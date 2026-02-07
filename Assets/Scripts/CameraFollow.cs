using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Режим обновления камеры
    /// </summary>
    public enum UpdateMode
    {
        LateUpdate,     // Обновление после всех Update (по умолчанию для камер)
        FixedUpdate     // Обновление синхронно с физикой (лучше для следования за Rigidbody)
    }

    /// <summary>
    /// Камера следует за транспортом
    /// SOLID: Single Responsibility - отвечает только за следование камеры
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target; // Цель (транспорт)
        
        [Header("Follow Settings")]
        [SerializeField] private Vector3 offset = new Vector3(2, 2, -5); // Смещение камеры относительно транспорта
        [SerializeField] private bool smoothFollow = true; // Плавное следование
        [SerializeField] private float smoothSpeed = 10f; // Скорость плавного следования (увеличена для уменьшения дерганья)
        [SerializeField] private UpdateMode updateMode = UpdateMode.FixedUpdate; // Режим обновления
        
        [Header("Auto Start")]
        [SerializeField] private bool followOnStart = false; // Следовать сразу при старте
        [SerializeField] private bool autoDetectVehicle = true; // Автоматически искать транспорт
        
        private bool isFollowing = false;
        private Vector3 initialOffset; // Начальное смещение (сохраняем при старте)
        private Vector3 velocity = Vector3.zero; // Для SmoothDamp

        void Start()
        {
            // Автоматически ищем транспорт
            if (autoDetectVehicle && target == null)
            {
                AutoVehicleMovement vehicleMovement = FindObjectOfType<AutoVehicleMovement>();
                if (vehicleMovement != null)
                {
                    target = vehicleMovement.transform;
                }
            }

            // Сохраняем начальное смещение
            if (target != null)
            {
                initialOffset = transform.position - target.position;
            }

            // Начинаем следовать сразу, если нужно
            if (followOnStart)
            {
                StartFollowing();
            }
        }

        void LateUpdate()
        {
            // Используем LateUpdate только если выбран режим LateUpdate
            if (updateMode == UpdateMode.LateUpdate)
            {
                UpdateCameraPosition();
            }
        }

        void FixedUpdate()
        {
            // Используем FixedUpdate если выбран режим FixedUpdate (синхронно с физикой)
            if (updateMode == UpdateMode.FixedUpdate)
            {
                UpdateCameraPosition();
            }
        }

        /// <summary>
        /// Обновление позиции камеры
        /// </summary>
        private void UpdateCameraPosition()
        {
            if (!isFollowing || target == null) return;

            // Целевая позиция камеры
            Vector3 targetPosition = target.position + offset;

            if (smoothFollow)
            {
                // Используем SmoothDamp вместо Lerp для более плавного движения без дерганья
                transform.position = Vector3.SmoothDamp(
                    transform.position, 
                    targetPosition, 
                    ref velocity, 
                    1f / smoothSpeed
                );
            }
            else
            {
                // Жёсткое следование
                transform.position = targetPosition;
            }
        }

        /// <summary>
        /// Начать следовать за целью
        /// </summary>
        public void StartFollowing()
        {
            isFollowing = true;
        }

        /// <summary>
        /// Остановить следование
        /// </summary>
        public void StopFollowing()
        {
            isFollowing = false;
        }

        /// <summary>
        /// Установить цель вручную
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                initialOffset = transform.position - target.position;
            }
        }

        /// <summary>
        /// Использовать текущее смещение как offset
        /// </summary>
        public void UseCurrentOffset()
        {
            if (target != null)
            {
                offset = transform.position - target.position;
            }
        }
    }
}

