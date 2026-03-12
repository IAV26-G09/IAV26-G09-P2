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
    using UCM.IAV.Navegacion;
    using UnityEngine;

    public class Patrullar : ComportamientoAgente
    {
        Vertex sigNodo;
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
            int rnd = Random.Range(0, neighbours.Length);
            sigNodo = neighbours[rnd];
            sigNodoPosicion = graph.vertexObjs[sigNodo.id].transform.position;
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

            //if (sigNodo != null)
            //{
            //    //Direccion actual
            //}
            //else
            //{
            //    direccion.lineal = new Vector3(0, 0, 0);
            //}

            ////Resto de cálculo de movimiento
            //direccion.lineal.Normalize();
            //direccion.lineal *= agente.aceleracionMax;

            //// Podríamos meter una rotación automática en la dirección del movimiento, si quisiéramos

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