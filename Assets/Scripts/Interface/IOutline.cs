using UnityEngine;

public interface IOutline //реализуем интерфейс для линий
{
    void UpdatePosition(Vector3 postion);

    void SetColor (Color color);

    void Destroy();
}
