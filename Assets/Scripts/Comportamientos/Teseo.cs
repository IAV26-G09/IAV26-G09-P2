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

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para controlar el comportamiento de Teseo (seguir/no seguir camino)
    /// </summary>
    public class Teseo : MonoBehaviour
    {
        bool ariadna = false;

        SeguirCamino segCam;
        ControlJugador contJug;

        void Start()
        {
            segCam = GetComponent<SeguirCamino>();
            contJug = GetComponent<ControlJugador>();
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!ariadna)
                {
                    updateAriadna(true);
                }
            }
            else
            {
                if (ariadna)
                {
                    updateAriadna(false);
                }
            }
        }

        public void updateAriadna(bool ar)
        {
            ariadna = ar;
            segCam.enabled = ariadna;
            contJug.enabled = !ariadna;
        }
    }
}
