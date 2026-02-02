using UnityEngine;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Скрипт для сборки транспорта из частей.
    /// Позволяет выбирать тело транспорта и прикреплять к нему различные детали.
    /// </summary>
    public class CarBuilder : MonoBehaviour
    {
        [Header("Префабы тел транспорта")]
        [Tooltip("Список доступных тел транспорта (Milk Rover, Can Rover, Banana Rover)")]
        public GameObject[] bodyPrefabs;

        [Header("Префабы деталей")]
        [Tooltip("Префабы колес (Simple Wheel, Spiked Wheel)")]
        public GameObject[] wheelPrefabs;
        
        [Tooltip("Префаб пропеллера (Fan Booster)")]
        public GameObject propellerPrefab;
        
        [Tooltip("Префаб крыльев (Wings)")]
        public GameObject wingPrefab;
        
        [Tooltip("Префаб ракеты (Rocket Booster)")]
        public GameObject rocketPrefab;

        [Header("Точки крепления на теле")]
        [Tooltip("Точка крепления переднего колеса (снизу спереди)")]
        public AttachmentPoint frontWheelPoint;
        
        [Tooltip("Точка крепления заднего колеса (снизу сзади)")]
        public AttachmentPoint backWheelPoint;
        
        [Tooltip("Точка крепления пропеллера (наверху)")]
        public AttachmentPoint propellerPoint;
        
        [Tooltip("Точка крепления крыльев или ракеты (сверху)")]
        public AttachmentPoint topAttachmentPoint;

        [Header("Текущее состояние")]
        [Tooltip("Текущее тело транспорта в сцене")]
        public GameObject currentBody;

        [Header("Скрипт движения")]
        [Tooltip("Скрипт, который будет управлять движением транспорта после сборки")]
        public CarMovement carMovement;

        /// <summary>
        /// Создает тело транспорта из выбранного префаба
        /// </summary>
        /// <param name="bodyIndex">Индекс тела в массиве bodyPrefabs (0, 1 или 2)</param>
        public void CreateBody(int bodyIndex)
        {
            // Проверяем, что индекс правильный
            if (bodyIndex < 0 || bodyIndex >= bodyPrefabs.Length)
            {
                Debug.LogError($"Неверный индекс тела: {bodyIndex}. Доступно тел: {bodyPrefabs.Length}");
                return;
            }

            // Удаляем старое тело, если оно есть
            if (currentBody != null)
            {
                Destroy(currentBody);
            }

            // Создаем новое тело
            GameObject bodyPrefab = bodyPrefabs[bodyIndex];
            if (bodyPrefab == null)
            {
                Debug.LogError($"Префаб тела с индексом {bodyIndex} не назначен!");
                return;
            }

            currentBody = Instantiate(bodyPrefab, transform);
            currentBody.name = bodyPrefab.name;

            // Находим точки крепления на новом теле
            FindAttachmentPoints();

            Debug.Log($"Создано тело: {bodyPrefab.name}");
        }

        /// <summary>
        /// Автоматически находит точки крепления на теле транспорта
        /// </summary>
        private void FindAttachmentPoints()
        {
            if (currentBody == null) return;

            // Ищем точки крепления по имени
            Transform frontPoint = currentBody.transform.Find("FrontWheelPoint");
            Transform backPoint = currentBody.transform.Find("BackWheelPoint");
            Transform propPoint = currentBody.transform.Find("PropellerPoint");
            Transform topPoint = currentBody.transform.Find("TopAttachmentPoint");

            // Назначаем найденные точки
            if (frontPoint != null)
            {
                frontWheelPoint = frontPoint.GetComponent<AttachmentPoint>();
                if (frontWheelPoint == null)
                {
                    frontWheelPoint = frontPoint.gameObject.AddComponent<AttachmentPoint>();
                    frontWheelPoint.partType = AttachmentPoint.PartType.Wheel;
                }
            }

            if (backPoint != null)
            {
                backWheelPoint = backPoint.GetComponent<AttachmentPoint>();
                if (backWheelPoint == null)
                {
                    backWheelPoint = backPoint.gameObject.AddComponent<AttachmentPoint>();
                    backWheelPoint.partType = AttachmentPoint.PartType.Wheel;
                }
            }

            if (propPoint != null)
            {
                propellerPoint = propPoint.GetComponent<AttachmentPoint>();
                if (propellerPoint == null)
                {
                    propellerPoint = propPoint.gameObject.AddComponent<AttachmentPoint>();
                    propellerPoint.partType = AttachmentPoint.PartType.Propeller;
                }
            }

            if (topPoint != null)
            {
                topAttachmentPoint = topPoint.GetComponent<AttachmentPoint>();
                if (topAttachmentPoint == null)
                {
                    topAttachmentPoint = topPoint.gameObject.AddComponent<AttachmentPoint>();
                    topAttachmentPoint.partType = AttachmentPoint.PartType.Wing; // По умолчанию для крыльев
                }
            }
        }

        /// <summary>
        /// Прикрепляет колесо к передней точке крепления
        /// </summary>
        /// <param name="wheelIndex">Индекс колеса в массиве wheelPrefabs</param>
        public void AttachFrontWheel(int wheelIndex)
        {
            if (frontWheelPoint == null)
            {
                Debug.LogError("Передняя точка крепления не найдена!");
                return;
            }

            if (!frontWheelPoint.IsEmpty())
            {
                frontWheelPoint.DetachPart();
            }

            if (wheelIndex < 0 || wheelIndex >= wheelPrefabs.Length)
            {
                Debug.LogError($"Неверный индекс колеса: {wheelIndex}");
                return;
            }

            GameObject wheel = Instantiate(wheelPrefabs[wheelIndex]);
            frontWheelPoint.AttachPart(wheel);
            
            Debug.Log($"Прикреплено переднее колесо: {wheelPrefabs[wheelIndex].name}");
        }

        /// <summary>
        /// Прикрепляет колесо к задней точке крепления
        /// </summary>
        /// <param name="wheelIndex">Индекс колеса в массиве wheelPrefabs</param>
        public void AttachBackWheel(int wheelIndex)
        {
            if (backWheelPoint == null)
            {
                Debug.LogError("Задняя точка крепления не найдена!");
                return;
            }

            if (!backWheelPoint.IsEmpty())
            {
                backWheelPoint.DetachPart();
            }

            if (wheelIndex < 0 || wheelIndex >= wheelPrefabs.Length)
            {
                Debug.LogError($"Неверный индекс колеса: {wheelIndex}");
                return;
            }

            GameObject wheel = Instantiate(wheelPrefabs[wheelIndex]);
            backWheelPoint.AttachPart(wheel);
            
            Debug.Log($"Прикреплено заднее колесо: {wheelPrefabs[wheelIndex].name}");
        }

        /// <summary>
        /// Прикрепляет пропеллер
        /// </summary>
        public void AttachPropeller()
        {
            if (propellerPoint == null)
            {
                Debug.LogError("Точка крепления пропеллера не найдена!");
                return;
            }

            if (propellerPrefab == null)
            {
                Debug.LogError("Префаб пропеллера не назначен!");
                return;
            }

            if (!propellerPoint.IsEmpty())
            {
                propellerPoint.DetachPart();
            }

            GameObject propeller = Instantiate(propellerPrefab);
            propellerPoint.AttachPart(propeller);
            Debug.Log("Прикреплен пропеллер");
        }

        /// <summary>
        /// Прикрепляет крылья
        /// </summary>
        public void AttachWings()
        {
            if (topAttachmentPoint == null)
            {
                Debug.LogError("Верхняя точка крепления не найдена!");
                return;
            }

            if (wingPrefab == null)
            {
                Debug.LogError("Префаб крыльев не назначен!");
                return;
            }

            if (!topAttachmentPoint.IsEmpty())
            {
                topAttachmentPoint.DetachPart();
            }

            GameObject wings = Instantiate(wingPrefab);
            topAttachmentPoint.AttachPart(wings);
            topAttachmentPoint.partType = AttachmentPoint.PartType.Wing;
            Debug.Log("Прикреплены крылья");
        }

        /// <summary>
        /// Прикрепляет ракету
        /// </summary>
        public void AttachRocket()
        {
            if (topAttachmentPoint == null)
            {
                Debug.LogError("Верхняя точка крепления не найдена!");
                return;
            }

            if (rocketPrefab == null)
            {
                Debug.LogError("Префаб ракеты не назначен!");
                return;
            }

            if (!topAttachmentPoint.IsEmpty())
            {
                topAttachmentPoint.DetachPart();
            }

            GameObject rocket = Instantiate(rocketPrefab);
            topAttachmentPoint.AttachPart(rocket);
            topAttachmentPoint.partType = AttachmentPoint.PartType.Rocket;
            Debug.Log("Прикреплена ракета");
        }

        /// <summary>
        /// Завершает сборку и запускает движение транспорта
        /// </summary>
        public void FinishBuilding()
        {
            // Проверяем, что есть тело и хотя бы одно колесо
            if (currentBody == null)
            {
                Debug.LogError("Нельзя завершить сборку: нет тела транспорта!");
                return;
            }

            if (frontWheelPoint == null || frontWheelPoint.IsEmpty())
            {
                Debug.LogError("Нельзя завершить сборку: нет переднего колеса!");
                return;
            }

            if (backWheelPoint == null || backWheelPoint.IsEmpty())
            {
                Debug.LogError("Нельзя завершить сборку: нет заднего колеса!");
                return;
            }

            // Включаем скрипт движения
            if (carMovement != null)
            {
                carMovement.enabled = true;
                carMovement.Initialize(currentBody);
                Debug.Log("Сборка завершена! Транспорт готов к движению.");
            }
            else
            {
                Debug.LogWarning("Скрипт движения не назначен! Транспорт не будет двигаться.");
            }
        }
    }
}
