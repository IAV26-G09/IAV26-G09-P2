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
using UCM.IAV.Movimiento;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    public class MinoEvader : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            MinoCollision collision = other.gameObject.GetComponent<MinoCollision>();
            if (!ReferenceEquals(collision, null))
            {
                Patrullar follow = other.gameObject.GetComponent<Patrullar>();
                if (follow != null) follow.ResetPath();
            }
        }
    }
}
