using UnityEngine;

/// <summary>
/// Clase para mostrar los ovillos en los nodos del camino
/// </summary>
public class Ovillo : MonoBehaviour
{
    MeshRenderer mr;

    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        if (mr != null) 
            mr.enabled = false;
    }

    public void Show(bool show)
    {
        mr.enabled = show;
    }
}
