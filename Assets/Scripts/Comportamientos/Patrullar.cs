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

        private void Start()
        {
            srcObj = gameObject;
            sigNodo = graph.GetNearestVertex(srcObj.transform.position);
            antNodo = sigNodo;
            //sigNodoPosicion = graph.vertexObjs[sigNodo.id].transform.position;
            SetPositions();
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

            Vector3 dir = sigNodoPosicion - transform.position;
            dir.y = 0;

            if (dir.magnitude <= magnitudeRange)
            {
                ChooseNextNode();
            }

            direccion.lineal = dir;
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

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
                Gizmos.color = Color.magenta; // Magenta es el nodo actual
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }

            Gizmos.color = Color.cyan; // Cyan es el nodo siguiente
            Gizmos.DrawSphere(sigNodoPosicion, pathNodeRadius);

            Gizmos.color = Color.yellow; // Negro es el nodo anterior
            Gizmos.DrawSphere(antNodoPosicion, pathNodeRadius);
        }

        public void SetPositions()
        {
            sigNodoPosicion = graph.GetVertexPos(sigNodo);
            antNodoPosicion = graph.GetVertexPos(antNodo);
        }

        public void ResetPath()
        {
            Vertex temp = sigNodo;
            sigNodo = antNodo;
            antNodo = sigNodo;
            SetPositions();
        }
    }
}