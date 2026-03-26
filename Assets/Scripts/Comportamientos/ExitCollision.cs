using UCM.IAV.Movimiento;
using UnityEngine;

/// <summary>
/// Clase para gestionar la colision con la casilla de salida
/// </summary>
public class ExitCollision : MonoBehaviour
{
    // Controla el ciclo de juego, al llegar a la casilla de salida se vuelve al menu
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(3))
        {
            GameManager.instance.goToScene("Menu");
        }
    }
}
