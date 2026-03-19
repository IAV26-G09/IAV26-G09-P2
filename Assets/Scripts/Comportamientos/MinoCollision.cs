/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    public class MinoCollision : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Teseo>() != null)
                GameManager.instance.RestartScene();
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.GetComponent<Teseo>() != null)
                GameManager.instance.RestartScene();
        }
    }
}