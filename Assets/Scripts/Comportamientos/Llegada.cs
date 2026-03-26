/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class Llegada : ComportamientoAgente
    {
        // El radio para llegar al objetivo
        public float radioObjetivo;

        // El radio en el que se empieza a ralentizarse
        public float radioRalentizado;

        public float fuerzaRalentizado;

        public float avoidQuantity = 5;
        public int distance = 7;

        // El tiempo en el que conseguir la aceleracion objetivo
        float timeToTarget = 0.1f;
        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // Distancia de objeto al agente
            float distance = (objetivo.transform.position - transform.position).magnitude;

            // Si ha alcanzado el radio objetivo se para
            if (distance < radioObjetivo)
            {
                direccion.lineal = new Vector3(0, 0, 0);
                return direccion;
            }

            float targetAccel;

            // Maxima aceleracion desde fuera del radio de frenado
            if (distance > radioRalentizado)
                targetAccel = agente.aceleracionMax;
            // Aceleracion escalada
            else
                targetAccel = agente.aceleracionMax * distance / (radioRalentizado * fuerzaRalentizado);

            // Velocity combina aceleracion y direccion
            Vector3 targetVelocity = objetivo.transform.position - transform.position;
            targetVelocity.Normalize();
            targetVelocity *= targetAccel;

            // La aceleracion se posiciona al nivel de la del objetivo
            direccion.lineal = targetVelocity - agente.velocidad;
            direccion.lineal /= timeToTarget;

            // Comprobamos que no se pase de aceleración
            if (direccion.lineal.magnitude > agente.aceleracionMax)
            {
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }

            return direccion;
        }

        Vector3 RayCastCollision(Vector3 pos, Vector3 dir, LayerMask lMask)
        {
            RaycastHit hit;
            if (Physics.Raycast(pos, dir, out hit, distance, lMask))
            {
                Vector3 incomingVec = hit.point - pos;

                if (incomingVec.magnitude > distance) return Vector3.zero;

                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                return hit.point + hit.normal * avoidQuantity;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}
