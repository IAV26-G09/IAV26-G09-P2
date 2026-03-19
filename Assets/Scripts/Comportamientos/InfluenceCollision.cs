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

        private void OnCollisionEnter(Collision collision)
        {
            var vertex = collision.gameObject.GetComponent<Vertex>();
            Debug.Log(collision.gameObject.name);
            if (vertex != null)
            {
                lastVertexCost = vertex.gCost;
                vertex.gCost = costOnCollision;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            var vertex = collision.gameObject.GetComponent<Vertex>();
            if (vertex != null)
            {
                vertex.gCost = lastVertexCost;
            }
        }
    }
}