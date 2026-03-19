using UCM.IAV.Movimiento;
using UnityEngine;

public class ExitCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            GameManager.instance.goToScene("Menu");
        }
    }
}
