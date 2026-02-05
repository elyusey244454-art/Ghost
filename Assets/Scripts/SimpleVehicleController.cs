using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Упрощённый контроллер транспорта
    /// SOLID: Single Responsibility - отвечает только за движение
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SimpleVehicleController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float baseSpeed = 10f;
        [SerializeField] private float jumpForce = 15f;
        
        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Движение (вызывается из Input системы)
        /// </summary>
        public void Move(float horizontal)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.x = horizontal * baseSpeed;
            rb.linearVelocity = velocity;
        }

        /// <summary>
        /// Прыжок (вызывается из Input системы)
        /// </summary>
        public void Jump()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}


