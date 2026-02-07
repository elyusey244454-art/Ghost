using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Вращение колеса при движении транспорта
    /// SOLID: Single Responsibility - отвечает только за визуальное вращение колеса
    /// </summary>
    public class WheelRotation : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 360f; // Скорость вращения (градусы в секунду)
        [SerializeField] private Vector3 rotationAxis = new Vector3(0, 0, 1); // Ось вращения (локальная)

        [Header("Auto Detection")]
        [SerializeField] private bool autoDetectVehicle = true; // Автоматически искать транспорт
        
        private Rigidbody vehicleRigidbody; // Rigidbody транспорта для определения скорости
        private bool isRotating = false;

        void Start()
        {
            if (autoDetectVehicle)
            {
                // Ищем Rigidbody у родителя (транспорта)
                Transform current = transform;
                while (current != null)
                {
                    vehicleRigidbody = current.GetComponent<Rigidbody>();
                    if (vehicleRigidbody != null)
                    {
                        break;
                    }
                    current = current.parent;
                }
            }
        }

        void Update()
        {
            // Проверяем, движется ли транспорт
            if (vehicleRigidbody != null)
            {
                float speed = vehicleRigidbody.linearVelocity.magnitude;
                
                if (speed > 0.1f) // Если скорость больше порога
                {
                    // Вращаем колесо
                    float rotation = rotationSpeed * Time.deltaTime;
                    transform.Rotate(rotationAxis, rotation, Space.Self);
                }
            }
        }

        /// <summary>
        /// Установить Rigidbody транспорта вручную
        /// </summary>
        public void SetVehicleRigidbody(Rigidbody rb)
        {
            vehicleRigidbody = rb;
        }

        /// <summary>
        /// Начать вращение (для ручного управления)
        /// </summary>
        public void StartRotation()
        {
            isRotating = true;
        }

        /// <summary>
        /// Остановить вращение (для ручного управления)
        /// </summary>
        public void StopRotation()
        {
            isRotating = false;
        }
    }
}

