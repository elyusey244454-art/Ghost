using UnityEngine;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Простой скрипт для тестирования сборки транспорта.
    /// Можно использовать для быстрой проверки работы системы.
    /// </summary>
    public class CarBuilderTester : MonoBehaviour
    {
        [Header("Ссылка на CarBuilder")]
        [Tooltip("Перетащите сюда объект с компонентом CarBuilder")]
        public CarBuilder carBuilder;

        [Header("Настройки тестовой сборки")]
        [Tooltip("Индекс тела транспорта (0 = Milk Rover, 1 = Can Rover, 2 = Banana Rover)")]
        public int bodyIndex = 0;

        [Tooltip("Индекс переднего колеса (0 = Simple Wheel, 1 = Spiked Wheel)")]
        public int frontWheelIndex = 0;

        [Tooltip("Индекс заднего колеса (0 = Simple Wheel, 1 = Spiked Wheel)")]
        public int backWheelIndex = 0;

        [Tooltip("Прикрепить пропеллер?")]
        public bool attachPropeller = false;

        [Tooltip("Прикрепить крылья? (если false и attachRocket = false, ничего не прикрепится)")]
        public bool attachWings = false;

        [Tooltip("Прикрепить ракету? (если false и attachWings = false, ничего не прикрепится)")]
        public bool attachRocket = false;

        [Tooltip("Автоматически завершить сборку после создания?")]
        public bool autoFinish = true;

        private void Start()
        {
            // Автоматически находим CarBuilder, если не назначен
            if (carBuilder == null)
            {
                carBuilder = FindObjectOfType<CarBuilder>();
                if (carBuilder == null)
                {
                    Debug.LogError("CarBuilder не найден в сцене!");
                    return;
                }
            }

            // Запускаем тестовую сборку
            TestBuild();
        }

        /// <summary>
        /// Выполняет тестовую сборку транспорта
        /// </summary>
        [ContextMenu("Test Build")]
        public void TestBuild()
        {
            if (carBuilder == null)
            {
                Debug.LogError("CarBuilder не назначен!");
                return;
            }

            Debug.Log("=== Начало тестовой сборки ===");

            // 1. Создаем тело
            carBuilder.CreateBody(bodyIndex);
            
            // Небольшая задержка для инициализации
            Invoke(nameof(AttachWheels), 0.1f);
        }

        private void AttachWheels()
        {
            // 2. Прикрепляем колеса
            carBuilder.AttachFrontWheel(frontWheelIndex);
            carBuilder.AttachBackWheel(backWheelIndex);

            // Небольшая задержка
            Invoke(nameof(AttachTopParts), 0.1f);
        }

        private void AttachTopParts()
        {
            // 3. Прикрепляем верхние детали
            if (attachPropeller)
            {
                carBuilder.AttachPropeller();
            }

            if (attachWings)
            {
                carBuilder.AttachWings();
            }
            else if (attachRocket)
            {
                carBuilder.AttachRocket();
            }

            // 4. Завершаем сборку
            if (autoFinish)
            {
                Invoke(nameof(FinishBuild), 0.1f);
            }
        }

        private void FinishBuild()
        {
            carBuilder.FinishBuilding();
            Debug.Log("=== Тестовая сборка завершена ===");
        }
    }
}


