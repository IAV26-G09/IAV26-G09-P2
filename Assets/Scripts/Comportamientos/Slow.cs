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
using UnityEngine;

namespace UCM.IAV.Navegacion
{
    using UCM.IAV.Movimiento;

    /// <summary>
    /// Clase para gestionar el area de influencia de los minotauros
    /// </summary>
    public class Slow : MonoBehaviour
    {
        /*
         *  Cuando el jugador (identificado con el ControlJugador) se acerca al trigger del
         *  minotauro, su velocidad maxima en el componente Agente se ve reducida enormemente
         *  si no tiene un obstaculo entre medias.
         *  Si logra abandonar el campo de vision, se restaura su velocidad.
         */

        private Agente playerAgent;
        private bool playerInside = false;
        private bool slowed = false;
        private float originalSpeed;
        [SerializeField]
        private float slowSpeed = 1.0f;

        private void OnTriggerEnter(Collider other)
        {
            ControlJugador cj = other.GetComponent<ControlJugador>();
            if (cj != null)
            {
                playerInside = true;
                playerAgent = other.GetComponent<Agente>();
                originalSpeed = playerAgent.velocidadMax;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!playerInside || playerAgent == null)
                return;

            bool canSee = RayClear(playerAgent.transform.position);

            if (canSee && !slowed)
            {
                playerAgent.velocidadMax = slowSpeed;
                slowed = true;
            }
            else if (!canSee && slowed)
            {
                RestoreSpeed();
            }
        }

        private void RestoreSpeed()
        {
            if (playerAgent != null)
            {
                playerAgent.velocidadMax = originalSpeed;
            }
            slowed = false;
        }

        private void OnTriggerExit(Collider other)
        {
            ControlJugador cj = other.GetComponent<ControlJugador>();
            if (cj != null)
            {
                RestoreSpeed();
                playerInside = false;
                playerAgent = null;
            }
        }

        // si el minotauro tiene campo de vision hasta el objetivo...
        private bool RayClear(Vector3 target)
        {
            Vector3 origin = transform.position;
            Vector3 dir = target - origin;
            float distance = Vector3.Distance(origin, target);
            int layerMask = 1 << 6;
            return !Physics.Raycast(origin, dir, out RaycastHit hitInfo, distance, layerMask);
        }
    }
}
