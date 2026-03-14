using UnityEngine;

public class Ovillo : MonoBehaviour
{
    MeshRenderer mr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    private void LateUpdate()
    {
        
    }
}
