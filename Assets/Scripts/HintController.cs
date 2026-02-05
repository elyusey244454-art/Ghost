using UnityEngine;

namespace VehicleConstructor
{
    /// <summary>
    /// Контроллер подсказки - управляет визуальной подсказкой для точки крепления
    /// SOLID: Single Responsibility - отвечает только за показ/скрытие подсказки
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class HintController : MonoBehaviour
    {
        [Header("Hint Info")]
        [SerializeField] private PartType compatiblePartType;
        
        private SpriteRenderer spriteRenderer;
        private bool isVisible = true;

        /// <summary>
        /// Совместимый тип детали
        /// </summary>
        public PartType CompatiblePartType => compatiblePartType;
        
        /// <summary>
        /// Видима ли подсказка
        /// </summary>
        public bool IsVisible => isVisible;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Show(); // По умолчанию все подсказки видны
        }

        /// <summary>
        /// Показать подсказку
        /// </summary>
        public void Show()
        {
            isVisible = true;
            spriteRenderer.enabled = true;
        }

        /// <summary>
        /// Скрыть подсказку
        /// </summary>
        public void Hide()
        {
            isVisible = false;
            spriteRenderer.enabled = false;
        }

        /// <summary>
        /// Проверка совместимости с типом детали
        /// </summary>
        public bool IsCompatibleWith(PartType type)
        {
            return compatiblePartType == type;
        }
    }
}

