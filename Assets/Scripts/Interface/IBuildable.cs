using UnityEngine;

public interface IBuildable // Реализуе интерфейс для построек
{
    GameObject Build {  get; }

    Vector3 Size { get; }

    void Place();
}
