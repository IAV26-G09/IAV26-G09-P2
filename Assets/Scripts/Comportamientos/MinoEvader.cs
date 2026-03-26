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
    /// <summary>
    /// Clara para gestionar la colision entre dos minotauros
    /// </summary>
    public class MinoEvader : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            MinoCollision collision = other.gameObject.GetComponent<MinoCollision>();
            if (!ReferenceEquals(collision, null))
            {
                Patrullar own = gameObject.GetComponent<Patrullar>();
                if (own != null)
                {
                    own.ResetPath(); // al chocar con otro minotauro deshace su camino
                }
            }
        }
    }
}
