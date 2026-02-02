using UnityEngine;

namespace Ghost.CarSystem
{
    /// <summary>
    /// Скрипт для управления движением транспорта.
    /// Работает только после завершения сборки.
    /// </summary>
    public class CarMovement : MonoBehaviour
    {
        [Header("Настройки движения")]
        [Tooltip("Скорость движения транспорта вперед")]
        [Range(1f, 50f)]
        public float moveSpeed = 10f;

        [Tooltip("Сила поворота транспорта")]
        [Range(1f, 20f)]
        public float turnSpeed = 5f;

        [Tooltip("Максимальная скорость транспорта")]
        [Range(5f, 100f)]
        public float maxSpeed = 30f;

        [Header("Ссылки на компоненты")]
        [Tooltip("Тело транспорта (должно иметь Rigidbody)")]
        public GameObject carBody;

        [Tooltip("Переднее колесо (для поворота)")]
        public AttachmentPoint frontWheelPoint;

        [Tooltip("Заднее колесо")]
        public AttachmentPoint backWheelPoint;

        // Приватные переменные
        private Rigidbody carRigidbody;
        private bool isInitialized = false;

        /// <summary>
        /// Инициализирует скрипт движения
        /// </summary>
        /// <param name="body">Тело транспорта</param>
        public void Initialize(GameObject body)
        {
            carBody = body;
            
            // Получаем Rigidbody из тела
            carRigidbody = carBody.GetComponent<Rigidbody>();
            if (carRigidbody == null)
            {
                Debug.LogError("У тела транспорта нет компонента Rigidbody!");
                return;
            }

            // Находим точки крепления колес
            if (frontWheelPoint == null)
            {
                Transform frontPoint = carBody.transform.Find("FrontWheelPoint");
                if (frontPoint != null)
                {
                    frontWheelPoint = frontPoint.GetComponent<AttachmentPoint>();
                }
            }

            if (backWheelPoint == null)
            {
                Transform backPoint = carBody.transform.Find("BackWheelPoint");
                if (backPoint != null)
                {
                    backWheelPoint = backPoint.GetComponent<AttachmentPoint>();
                }
            }

            isInitialized = true;
            Debug.Log("Движение транспорта инициализировано!");
        }

        private void Update()
        {
            // Движение работает только после инициализации
            if (!isInitialized || carRigidbody == null)
                return;

            // Получаем ввод от игрока
            float horizontal = Input.GetAxis("Horizontal"); // A/D или стрелки влево/вправо
            float vertical = Input.GetAxis("Vertical");     // W/S или стрелки вверх/вниз

            // Движение вперед/назад - применяем силу только при нажатии клавиш
            if (Mathf.Abs(vertical) > 0.1f)
            {
                // Применяем силу в направлении движения транспорта
                Vector3 forwardDirection = carRigidbody.transform.forward;
                Vector3 force = forwardDirection * moveSpeed * vertical;
                carRigidbody.AddForce(force);

                // Ограничиваем максимальную скорость
                if (carRigidbody.linearVelocity.magnitude > maxSpeed)
                {
                    carRigidbody.linearVelocity = carRigidbody.linearVelocity.normalized * maxSpeed;
                }
            }
            else
            {
                // Если не нажата клавиша движения, применяем сопротивление для остановки
                carRigidbody.linearVelocity = Vector3.Lerp(carRigidbody.linearVelocity, Vector3.zero, Time.deltaTime * 2f);
            }

            // Поворот - только при нажатии клавиш
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                // Поворачиваем транспорт вокруг оси Y
                float rotation = horizontal * turnSpeed;
                carRigidbody.angularVelocity = new Vector3(0, rotation, 0);
            }
            else
            {
                // Плавно останавливаем вращение
                carRigidbody.angularVelocity = Vector3.Lerp(carRigidbody.angularVelocity, Vector3.zero, Time.deltaTime * 2f);
            }
        }

        /// <summary>
        /// Останавливает движение транспорта
        /// </summary>
        public void StopMovement()
        {
            if (carRigidbody != null)
            {
                carRigidbody.linearVelocity = Vector3.zero;
                carRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
