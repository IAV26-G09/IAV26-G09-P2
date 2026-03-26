/*
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).
   Autor: Federico Peinado
   Contacto: email@federicopeinado.com
*/


namespace UCM.IAV.Movimiento
{
    using UCM.IAV.Navegacion;
    using UnityEngine;

    /// <summary>
    /// Clase para el comportamiento de los minotauros patrulla
    /// </summary>
    public class Patrullar : ComportamientoAgente
    {
        Vertex sigNodo;
        Vertex antNodo;
        private Vector3 sigNodoPosicion;
        private Vector3 antNodoPosicion;

        public Graph graph;
        public GameObject srcObj;

        [SerializeField]
        [Range(0f, 1f)]
        float magnitudeRange = 0.25f;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        bool idling = false;
        float counter = 0.0f;
        [SerializeField]
        float idleTime = 2f;

        [SerializeField]
        float idleFrequency = 2f; // velocidad de oscilación
        [SerializeField]
        float maxIdleAngle = 45f;
        private float initialYRotation;
        bool justFinishedIdle = false;

        private void Start()
        {
            srcObj = gameObject;
            sigNodo = graph.GetNearestVertex(srcObj.transform.position);
            antNodo = sigNodo;
            SetPositions();
            initialYRotation = transform.eulerAngles.y;
        }

        void ChooseNextNode()
        {
            Vertex[] neighbours = graph.GetNeighbours(sigNodo);

            // si tienes mas de una opcion (mas de un vecino)
            if (neighbours.Length > 1)
            {
                sigNodo = GetNewNode(ref neighbours);
            }
            // si tienes un vecino o menos significa que estas en un callejon sin salida
            // eliges el disponible
            else if (neighbours.Length == 1) 
            {
                antNodo = sigNodo; // antes de cambiarlo guardas el anterior
                sigNodo = neighbours[0]; // escoges el unico disponible
                if (!justFinishedIdle) // reiniciamos idle
                {
                    idling = true;
                    initialYRotation = transform.eulerAngles.y;
                }
            }
            SetPositions();
        }

        // metodo recursivo para obtener un nodo nuevo al que ir
        Vertex GetNewNode(ref Vertex[] neighbours)
        {
            int rnd = Random.Range(0, neighbours.Length);
            Vertex newNode = neighbours[rnd];

            if (newNode.id != antNodo.id)
            { // para no poder volver hacia atras
                if (IsTurn(antNodo, sigNodo, newNode) && !justFinishedIdle) 
                { // si va a girar y no acaba de estar en idle reiniciamos el idle
                    idling = true;
                    initialYRotation = transform.eulerAngles.y;
                }
                // actualizamos valores
                justFinishedIdle = false;
                antNodo = sigNodo;
                return newNode;
            }
            else
            { // volvemos a buscar
                return GetNewNode(ref neighbours);
            }
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            // el idle dura un cierto tiempo idleTime y se lleva la cuenta con counter
            if (idling)
            {
                counter += Time.deltaTime;

                // rotacion manual con el seno del contador y una frecuencia dentro de un angulo maximo
                float angle = Mathf.Sin(counter * idleFrequency) * maxIdleAngle;
                // la aplicamos sobre la rotacion inicial cuando se quedo quieto
                transform.rotation = Quaternion.Euler(0, initialYRotation + angle, 0);

                if (counter >= idleTime)
                {
                    // si hemos acabado el idle reiniciamos valores y reescogemos nodo al que ir
                    counter = 0.0f;
                    idling = false;
                    justFinishedIdle = true;
                    ChooseNextNode();
                }

                return direccion;
            }
            else
            {
                Vector3 dir = sigNodoPosicion - transform.position;
                dir.y = 0;

                if (dir.magnitude <= magnitudeRange)
                { // si ha llegado a un nodo escogemos el siguiente al que ir
                    ChooseNextNode();
                }

                direccion.lineal = dir;
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }

            return direccion;
        }

        // muestra con bolitas los nodos anterior, actual y siguiente
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            if (!ReferenceEquals(srcObj, null))
            {
                // Magenta es el nodo actual
                v = graph.GetNearestVertex(srcObj.transform.position);
                GameManager.instance.DrawSphere(v.transform.position, pathNodeRadius, Color.magenta);
            }
            GameManager.instance.DrawSphere(sigNodoPosicion, pathNodeRadius, Color.cyan);
            GameManager.instance.DrawSphere(antNodoPosicion, pathNodeRadius, Color.yellow);
        }

        // metodo para actualizar las referencias a las posiciones de los vertices que tiene marcados
        public void SetPositions()  
        {
            sigNodoPosicion = graph.GetVertexPos(sigNodo);
            antNodoPosicion = graph.GetVertexPos(antNodo);
        }

        // supone un giro de 180 grados porque hace que el agente vuelva al nodo anterior
        public void ResetPath()
        {
            if (graph == null || sigNodo == null || antNodo == null) return;
            Vertex temp = sigNodo;
            sigNodo = antNodo;
            antNodo = temp;
            SetPositions();
        }

        // hace el dot product de los vectores desde la posicion actual (current) a la anterior (from) y a la proxima (to) para ver el angulo que suponen y por tanto si es un giro
        bool IsTurn(Vertex from, Vertex current, Vertex to)
        { 
            Vector3 dir1 = graph.GetVertexPos(current) - graph.GetVertexPos(from);
            Vector3 dir2 = graph.GetVertexPos(to) - graph.GetVertexPos(current);

            dir1.y = 0;
            dir2.y = 0;

            dir1.Normalize();
            dir2.Normalize();

            float dot = Vector3.Dot(dir1, dir2);

            //1 -> 0º
            //-1 -> 180º
            //0 -> 90º

            return dot < 0.9f;
        }
    }
}