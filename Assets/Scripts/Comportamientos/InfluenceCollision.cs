using UCM.IAV.Movimiento;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    public class InfluenceCollision : MonoBehaviour
    {
        private float lastVertexCost;
        [SerializeField]
        private float costOnCollision = 5.0f;

        private void OnTriggerEnter(Collider collision)
        {
            var vertex = collision.gameObject.GetComponent<Vertex>();
            Debug.Log(collision.gameObject.name);
            if (vertex != null)
            {
                lastVertexCost = vertex.gCost;

                //Debug.Log("last " + lastVertexCost);

                vertex.gCost = costOnCollision;

                Debug.Log("VERTICE " + vertex.id + " AHORA ES " + vertex.gCost);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            var vertex = collision.gameObject.GetComponent<Vertex>();
            if (vertex != null)
            {
                vertex.gCost = lastVertexCost;
            }
        }
    }
}