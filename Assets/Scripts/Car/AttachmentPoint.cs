using UnityEngine;

namespace Ghost.CarSystem
{
    public class AttachmentPoint : MonoBehaviour
    {
        [Header("Настройки точки крепления")]
        public PartType partType;
        
        [Tooltip("Уже прикрепленная деталь (если есть)")]
        public GameObject attachedPart;

        public enum PartType
        {
            Wheel,      // Колесо (спереди или сзади)
            Propeller,  // Пропеллер (Fan Booster)
            Wing,       // Крылья (Wings)
            Rocket      // Ракета (Rocket Booster)
        }

        /// <summary>
        /// Проверяет, свободна ли точка крепления
        /// </summary>
        public bool IsEmpty()
        {
            return attachedPart == null;
        }

        /// <summary>
        /// Прикрепляет деталь к этой точке
        /// </summary>
        public void AttachPart(GameObject part)
        {
            if (!IsEmpty())
            {
                Debug.LogWarning($"Точка крепления {gameObject.name} уже занята!");
                return;
            }

            attachedPart = part;
            part.transform.SetParent(transform);
            part.transform.localPosition = Vector3.zero;
            part.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Удаляет прикрепленную деталь
        /// </summary>
        public void DetachPart()
        {
            if (attachedPart != null)
            {
                Destroy(attachedPart);
                attachedPart = null;
            }
        }
    }
}

