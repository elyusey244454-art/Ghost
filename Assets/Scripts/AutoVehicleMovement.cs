using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Автоматическое движение транспорта вперед
    /// SOLID: Single Responsibility - отвечает только за движение
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class AutoVehicleMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f; // Скорость движения вперед
        [SerializeField] private bool isMoving = false; // Движется ли транспорт

        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (isMoving && rb != null)
            {
                // Движение вперед по локальной оси X (вправо для транспорта)
                Vector3 forwardForce = transform.right * moveSpeed;
                rb.linearVelocity = new Vector3(forwardForce.x, rb.linearVelocity.y, forwardForce.z);
            }
        }

        /// <summary>
        /// Начать движение
        /// </summary>
        public void StartMoving()
        {
            isMoving = true;
            Debug.Log("[AutoVehicleMovement] Транспорт начал движение!");
        }

        /// <summary>
        /// Остановить движение
        /// </summary>
        public void StopMoving()
        {
            isMoving = false;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }
            Debug.Log("[AutoVehicleMovement] Транспорт остановлен!");
        }

        /// <summary>
        /// Проверка, движется ли транспорт
        /// </summary>
        public bool IsMoving => isMoving;
    }
}


