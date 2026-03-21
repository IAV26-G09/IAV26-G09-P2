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
        private float lastVertexCost;
        [SerializeField]
        private float costOnCollision = 5.0f;

        public Graph graph;

        //private void OnTriggerEnter(Collider collision)
        private void OnCollisionEnter(Collision collision)
        {
            if (this.enabled)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                if (vertex != null)
                {
                    Debug.Log(collision.gameObject.name);

                    //lastVertexCost = vertex.gCost;
                    //Debug.Log("last " + lastVertexCost);

                    //vertex.gCost = costOnCollision;
                    graph.UpdateVertexCost(vertex.gameObject.transform.position, costOnCollision);

                    Debug.Log("VERTICE " + vertex.id + " AHORA ES " + costOnCollision);
                }
            }
        }

        //private void OnTriggerExit(Collider collision)
        private void OnCollisionExit(Collision collision)
        {
            if (this.enabled)
            {
                var vertex = collision.gameObject.GetComponent<Vertex>();
                if (vertex != null)
                {
                    //vertex.gCost = lastVertexCost;

                    graph.UpdateVertexCost(vertex.gameObject.transform.position, 1);
                }
            }
        }

        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            Gizmos.color = Color.red; // Verde es el nodo inicial
            v = graph.GetNearestVertex(transform.position);
            Gizmos.DrawSphere(v.transform.position, 0.25f);
        }
    }
}