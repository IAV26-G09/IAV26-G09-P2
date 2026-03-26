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

            // si tienes mas de una opcion
            if (neighbours.Length > 1)
            {
                sigNodo = GetNewNode(ref neighbours);
            }
            // si tienes un vecino o menos significa que estas en una encrucijada
            // eliges el disponible
            else
            {
                antNodo = sigNodo; // antes de cambiarlo guardas el anterior
                sigNodo = neighbours[0];
                if (!justFinishedIdle)
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

            if (newNode.id != antNodo.id) // para no poder volver hacia atras
            {
                if (IsTurn(antNodo, sigNodo, newNode) && !justFinishedIdle)
                {
                    idling = true;
                    initialYRotation = transform.eulerAngles.y;
                }
                justFinishedIdle = false;
                antNodo = sigNodo;
                return newNode;
            }
            else
            {
                return GetNewNode(ref neighbours);
            }
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            if (idling)
            {
                counter += Time.deltaTime;

                direccion.lineal = Vector3.zero;

                float angle = Mathf.Sin(counter * idleFrequency) * maxIdleAngle;

                transform.rotation = Quaternion.Euler(0, initialYRotation + angle, 0);

                if (counter >= idleTime)
                {
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
                {
                    ChooseNextNode();
                }

                direccion.lineal = dir;
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }

            return direccion;
        }

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

        public void SetPositions()  
        {
            sigNodoPosicion = graph.GetVertexPos(sigNodo);
            antNodoPosicion = graph.GetVertexPos(antNodo);
        }

        public void ResetPath()
        {
            if (graph == null || sigNodo == null || antNodo == null) return;
            Vertex temp = sigNodo;
            sigNodo = antNodo;
            antNodo = temp;
            SetPositions();
        }

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