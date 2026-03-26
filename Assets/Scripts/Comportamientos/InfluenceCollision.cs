using System.Collections;
using System.Collections.Generic;
using System.IO;
using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    /// <summary>
    /// Clase para actualizar el coste de los nodos en colision con agentes minotauros y su area de influencia
    /// </summary>
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
            if (vertex != null && RayClear(vertex))
            {
                if (vertex.fCost < costOnCollision)
                    graph.UpdateVertexCost(vertex.transform.position, costOnCollision);

                if (debugging)
                    affectedVertexes.Add(vertex);
            }
        }

        private void ExitVertex(Vertex vertex, float exitCost)
        {
            if (vertex != null && RayClear(vertex))
            {
                graph.UpdateVertexCost(vertex.transform.position, exitCost);

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

            foreach (Vertex vv in affectedVertexes)
            {
                GameManager.instance.DrawSphere(vv.transform.position, gizmoRadius, new Color(0.95f, 0.65f, 0f));
            }
        }

        // si el minotauro tiene campo de vision hasta el vertice...
        private bool RayClear(Vertex v)
        {
            Vector3 origin = transform.position;
            Vector3 target = v.transform.position;

            Vector3 dir = target - origin;
            float distance = Vector3.Distance(origin, target);

            int layerMask = 1 << 6;
            return !Physics.Raycast(origin, dir, out RaycastHit hitInfo, distance, layerMask);
        }
    }
}