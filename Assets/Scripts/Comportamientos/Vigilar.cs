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
        float maxTime = 2.0f; // tiempo maximo de espera antes de girar
        [SerializeField]
        float minTime = 1.0f; // tiempo minimo de espera antes de girar

        [SerializeField]
        float minRan = -0.05f; // rango minimo de velocidad angular para girar
        [SerializeField]
        float maxRan = 0.05f; // rango maximo de velocidad angular para girar

        float t = 3.0f; // tiempo de espera, valores iniciales para asegurar giro al empezar
        float actualT = 2.0f; // contador de tiempo de espera, valores iniciales para asegurar giro al empezar

        Direccion lastDir = new Direccion();

        // cada tiempo actualT aleatorizada entre un rango minTime y maxTime se le da una velocidad angular aleatorizada entre minRan y maxRan al minotauro para hacerle girar
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
