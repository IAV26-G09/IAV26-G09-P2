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

        private Agente playerAgent; // componente agente del avatar
        private bool playerInside = false; // si el jugador se encuentra dentro de la esfera de influencia
        private bool slowed = false; // si el jugador ha sido ralentizado
        private float originalSpeed; // cada vez que el jugador entre en el trigger aqui se guardara su velocidadi nicial
        [SerializeField]
        private float slowSpeed = 1.0f; // velocidad maxima a aplicar cuando ralentizamos al jugador

        private void OnTriggerEnter(Collider other)
        {
            ControlJugador cj = other.GetComponent<ControlJugador>();
            if (cj != null)
            { // cuando entra el jugador en el radio se actualizan sus datos
                playerInside = true;
                playerAgent = other.GetComponent<Agente>();
                originalSpeed = playerAgent.velocidadMax;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // si no esta el jugador o no hay referencia a su agente no se hace nada
            if (!playerInside || playerAgent == null)
                return;

            // raycast para ver si el minotauro esta viendo al jugador
            bool canSee = RayClear(playerAgent.transform.position);

            // sii puede ver al jugador y este todavia no esta ralentizado
            if (canSee && !slowed)
            { // se le ralentiza y se actualiza el booleano
                playerAgent.velocidadMax = slowSpeed;
                slowed = true;
            }
            else if (!canSee && slowed)
            { // si ha dejado de poder ver y estaba siendo ralentizado se le devuelve su velocidad original
                RestoreSpeed();
            }
        }

        // metodo para devolver la velocidad original al agente del jugador
        private void RestoreSpeed()
        {
            if (playerAgent != null)
            {
                playerAgent.velocidadMax = originalSpeed;
            }
            slowed = false;
        }

        // si sale del trigger se actualizan los datos y se le devuelve su velocidad original
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
