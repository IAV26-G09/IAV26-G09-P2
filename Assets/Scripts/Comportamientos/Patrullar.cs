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
    using System;
    using UCM.IAV.Navegacion;
    using UnityEngine;

    public class Patrullar : ComportamientoAgente
    {
        Transform sigNodo;

        public Graph graph;
        public GameObject srcObj;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        private void Start()
        {
            srcObj = gameObject;
        }

        override public void Update()
        {
            //sigNodo = graph.GetNextNode();
            //base.Update();
        }

        public override Direccion GetDireccion()
        {
            Direccion direccion = new Direccion();

            //if (sigNodo != null)
            //{
            //    //Direccion actual
            //    direccion.lineal = sigNodo.position - transform.position;
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