/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{

    using UnityEngine;

    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador: ComportamientoAgente
    {
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        /// 

        //float tiempoGiroSuave = 0.1f;
        //float velocidadGiroSuave;

        [SerializeField]
        float minimuRadius = 3.0f; // radio alrededor del jugador en el que no moverse

        private float velocidadMaxNormal;
        private float velocidadMaxRapida;
        private float aceleracionNormal;
        private float aceleracionRapida;

        private bool able = true;
        private bool sprinting = false;

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            Debug.Log("0");

            if (!able)
            {
                Debug.Log("1");
                return direccion;
            }
                
            // Direccion actual
            // Control por teclado
            direccion.lineal.x = Input.GetAxis("Horizontal");
            direccion.lineal.z = Input.GetAxis("Vertical");

            // Control por raton
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 8;

            // Si apuntamos a un sitio valido
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            { // Cogemos la direccion y nos congelamos en altura

                Debug.Log("eing");
                direccion.lineal = hit.point - transform.position;
                direccion.lineal.y = 0;
            }

            // Si la colision, aunque valida esta en un radio cercano al jugador
            if (direccion.lineal.magnitude < minimuRadius)
            {
                Debug.Log("2");
                return new Direccion();
            }

            // Comprobamos si estamos corriendo
            sprinting = Input.GetKey(KeyCode.Mouse0);

            if (sprinting)
            {
                agente.aceleracionMax = aceleracionRapida;
                agente.velocidadMax = velocidadMaxRapida;
                Debug.Log("3");
            }
            else
            {
                agente.aceleracionMax = aceleracionNormal;
                agente.velocidadMax = velocidadMaxNormal;
                Debug.Log("4");
            }

            // Resto de calculo de movimiento
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            Debug.Log("5");

            return direccion;
        }

        // MOVIMIENTO CON WASD
        /*
        public override Direccion GetDireccion()
        {

            Direccion direccion = new Direccion();
            
            //Direccion actual
            direccion.lineal.x = Input.GetAxis("Horizontal");
            direccion.lineal.z = Input.GetAxis("Vertical");

            //Resto de calculo de movimiento
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            return direccion;
        }
        */
    }
}