using UnityEngine;

public class Outline : MonoBehaviour, IOutline // Реализуем линию для того чтобы когда ставим здание показывала область цветом можно ставить или нет 
{
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void UpdatePosition(Vector3 position)
    {
        transform.position = position + Vector3.up * 0.01f;
    }

    public void SetColor(Color color)
    {
        if(_renderer != null)
        {
            _renderer.material.color = color;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
