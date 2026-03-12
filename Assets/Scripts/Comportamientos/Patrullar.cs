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

        public GraphGrid graph;
        public GameObject srcObj;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        private void Start()
        {
            srcObj = gameObject;
            sigNodo = graph.GetNearestVertex(srcObj.transform.position);
            sigNodoPosicion = graph.vertexObjs[sigNodo.id].transform.position;
            Debug.Log(sigNodoPosicion);
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
                int rnd = Random.Range(0, neighbours.Length);
                antNodo = sigNodo; // antes de cambiarlo guardas el anterior
                sigNodo = neighbours[0];
            }

            sigNodoPosicion = graph.vertexObjs[sigNodo.id].transform.position;
        }

        Vertex GetNewNode(ref Vertex[] neighbours)
        {
            int rnd = Random.Range(0, neighbours.Length);
            Vertex newNode = neighbours[rnd];

            if (newNode != antNodo) // para no poder volver hacia atras
            {
                antNodo = newNode;
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
            Debug.Log(direccion.lineal);

            Vector3 dir = sigNodoPosicion - transform.position;
            dir.y = 0;

            if (dir.magnitude <= 0.01f)
            {
                ChooseNextNode();
            }

            direccion.lineal = dir;
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;
            Debug.Log(direccion.lineal);

            /*
            //if (sigNodo != null)
            //{
            //    //Direccion actual
            //}
            //else
            //{
            //    direccion.lineal = new Vector3(0, 0, 0);
            //}

            ////Resto de calculo de movimiento
            //direccion.lineal.Normalize();
            //direccion.lineal *= agente.aceleracionMax;

            //// Podriamos meter una rotacion automatica en la direccion del movimiento, si quisieramos
            ///*/

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
                Gizmos.color = Color.magenta; // Verde es el nodo inicial
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
        }

        public void ResetPath()
        {
            //graph.ResetPath();
        }
    }
}