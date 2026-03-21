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

        private float accAnd;
        private float accSpr;

        private bool able = true;
        private bool sprinting = false;


        private void Start()
        {
            accAnd = agente.aceleracionMax / 2;
            accSpr = agente.aceleracionMax;
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            if (!able)
            {
                return direccion;
            }
                
            // Direccion actual
            // Control por teclado
            direccion.lineal.x = Input.GetAxis("Horizontal");
            direccion.lineal.z = Input.GetAxis("Vertical");

            // Control por raton
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 10;

            // Si apuntamos a un sitio valido
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            { // Cogemos la direccion y nos congelamos en altura
                Vector3 projectedPos1 = hit.point;
                Vector3 projectedPos2 = transform.position;

                projectedPos1.y = projectedPos2.y;

                direccion.lineal = projectedPos1 - projectedPos2;
                direccion.lineal.y = 0;
            }

            // Si la colision, aunque valida esta en un radio cercano al jugador
            if (direccion.lineal.magnitude < minimuRadius)
            {
                return new Direccion();
            }

            // Comprobamos si estamos corriendo
            sprinting = Input.GetKey(KeyCode.Mouse0);

            // Resto de calculo de movimiento
            direccion.lineal.Normalize();
            float acc = sprinting ? accSpr : accAnd;
            direccion.lineal *= acc;

            return direccion;
        }
    }
}