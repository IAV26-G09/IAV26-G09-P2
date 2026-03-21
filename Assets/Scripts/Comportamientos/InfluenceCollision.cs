using System.Collections;
using System.Collections.Generic;
using System.IO;
using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    public class InfluenceCollision : MonoBehaviour
    {
        [SerializeField]
        private float costOnCollision = 5.0f;

        public Graph graph;

        [SerializeField]
        bool useTrigger = true;

        [SerializeField]
        bool debugging = true;
        List<Vertex> affectedVertexes = new List<Vertex>();
        float gizmoRadius = 0.25f;

        private void EnterVertex(Vertex vertex)
        {
            if (vertex != null)
            {
                //Debug.Log(collision.gameObject.name);
                //Debug.Log("last " + lastVertexCost);

                if (vertex.fCost < costOnCollision)
                    graph.UpdateVertexCost(vertex.gameObject.transform.position, costOnCollision);

                if (debugging)
                    affectedVertexes.Add(vertex);

                //Debug.Log("VERTICE " + vertex.id + " AHORA ES " + costOnCollision);
            }
        }

        private void ExitVertex(Vertex vertex, float exitCost)
        {
            if (vertex != null)
            {
                graph.UpdateVertexCost(vertex.gameObject.transform.position, exitCost);

                if (debugging)
                    affectedVertexes.Remove(vertex);
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (this.enabled && useTrigger)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                EnterVertex(vertex);
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (this.enabled && !useTrigger)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                EnterVertex(vertex);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (this.enabled && !useTrigger)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                ExitVertex(vertex, 5);
            }
        }
        private void OnTriggerExit(Collider collision)
        {
            if (this.enabled && useTrigger)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                ExitVertex(vertex, 1);  
            }
        }

        virtual public void OnDrawGizmos()
        {
            if (!debugging)
                return;

            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            Gizmos.color = Color.red; // Verde es el nodo inicial
            v = graph.GetNearestVertex(transform.position);
            Gizmos.DrawSphere(v.transform.position, gizmoRadius);

            foreach (Vertex vv in affectedVertexes)
            {
                Gizmos.color = Color.yellow; // Amarillo es el nodo afectado
                Gizmos.DrawSphere(vv.transform.position, gizmoRadius);
            }
        }
    }
}