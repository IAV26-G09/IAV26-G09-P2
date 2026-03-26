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
    /// Clase para el comportamiento de los minotauros vigias, rotacion aleatoria
    /// </summary>
    public class Vigilar : ComportamientoAgente
    {
        [SerializeField]
        float maxTime = 2.0f;

        [SerializeField]
        float minTime = 1.0f;

        [SerializeField]
        float minRan = -0.05f;
        [SerializeField]
        float maxRan = 0.05f;

        float t = 3.0f;
        float actualT = 2.0f;

        Direccion lastDir = new Direccion();

        public override Direccion GetDireccion()
        {
            if (t >= actualT)
            {
                Direccion direccion = new Direccion();

                float wanderOrientation = Random.Range(minRan, maxRan);

                direccion.angular = wanderOrientation;

                lastDir = direccion;

                actualT = Random.Range(minTime, maxTime);

                t = 0.0f;
            }
            else
            {
                t += Time.deltaTime;
            }

            return lastDir;
        }
    }
}
