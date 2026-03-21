/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using UCM.IAV.Movimiento;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace UCM.IAV.Navegacion
{
    // Posibles algoritmos para buscar caminos en grafos
    // REALMENTE PARA ESTA PRÁCTICA SÓLO SE NECESITA ASTAR, los otros no los usaremos...
    public enum TesterGraphAlgorithm
    {
        BFS, DFS, ASTAR
    }
    public enum Heuristic
    {
        Manhattan,
        Euclidea
    }

    public class TheseusGraph : MonoBehaviour
    {
        [SerializeField]
        protected Graph graph;

        [SerializeField]
        private TesterGraphAlgorithm algorithm;

        [SerializeField]
        private bool smoothPath;

        [SerializeField]
        private string vertexTag = "Vertex"; // Etiqueta de un nodo normal

        [SerializeField]
        private string obstacleTag = "Wall"; // Etiqueta de un obstáculo, tipo pared...

        [SerializeField]
        private Color pathColor;

        [SerializeField]
        [Range(0.1f, 1f)]
        private float pathNodeRadius = .3f;

        private bool ariadna;

        Camera mainCamera;
        protected GameObject srcObj;
        protected GameObject dstObj;
        protected List<Vertex> path; // La variable con el camino calculado

        protected LineRenderer hilo;
        protected float hiloOffset = 0.2f;

        protected ControlJugador control;
        protected SeguirCamino seguir;

        Heuristic _currHeuristic = Heuristic.Manhattan;

        // Despertar inicializando esto
        public virtual void Awake()
        {
            mainCamera = Camera.main;
            srcObj = GameManager.instance.GetPlayer();
            dstObj = null;
            path = new List<Vertex>();
            hilo = GetComponent<LineRenderer>();
            ariadna = false;

            hilo.startWidth = 0.15f;
            hilo.endWidth = 0.15f;
            hilo.positionCount = 0;

            if (srcObj != null)
            {
                control = srcObj.GetComponent<ControlJugador>();
                seguir = srcObj.GetComponent<SeguirCamino>();
            }
        }

        // Update is called once per frame
        public virtual void Update()
        {
            // clic derecho desactiva el hilo
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                updateAriadna(!ariadna);
            }
            
            if (Input.GetKeyDown(KeyCode.S))
                smoothPath = !smoothPath;

            if (ariadna) // si tenemos el hilo activado lo calculamos
            {
                //Source jugador y destino el nodo final
                if (srcObj == null) srcObj = GameManager.instance.GetPlayer();
                if (dstObj == null) dstObj = GameManager.instance.GetExitNode();

                //path = new List<Vertex>();

                switch (algorithm)
                {
                    case TesterGraphAlgorithm.ASTAR:
                        if (_currHeuristic == Heuristic.Manhattan) path = graph.GetPathAstar(srcObj, dstObj, Manhattan); 
                        else if (_currHeuristic == Heuristic.Euclidea) path = graph.GetPathAstar(srcObj, dstObj, Euclidean);
                        break;
                    default:
                    case TesterGraphAlgorithm.BFS:
                        path = graph.GetPathBFS(srcObj, dstObj);
                        break;
                    case TesterGraphAlgorithm.DFS:
                        path = graph.GetPathDFS(srcObj, dstObj);
                        break;
                }
                if (smoothPath)
                {
                    path = graph.Smooth(path); // Suavizar el camino, una vez calculado
                }

                if (path.Count > 0)
                {
                    //GameManager.instance.SetPlayerNode(path[path.Count - 1].transform);

                    DibujaHilo();
                }
                else
                {
                    hilo.positionCount = 0;
                }
            }
        }

        public virtual Transform GetNextNode()
        {
            if (path.Count > 0)
                return path[path.Count - 1].transform;

            return null;
        }

        // Dibujado de artilugios en el editor
        // OJO, ESTO SÓLO SE PUEDE VER EN LA PESTAÑA DE SCENE DE UNITY
        virtual public void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graph, null))
                return;

            Vertex v;
            if (!ReferenceEquals(srcObj, null))
            {
                Gizmos.color = Color.green; // Verde es el nodo inicial
                v = graph.GetNearestVertex(srcObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            if (!ReferenceEquals(dstObj, null))
            {
                Gizmos.color = Color.red; // Rojo es el color del nodo de destino
                v = graph.GetNearestVertex(dstObj.transform.position);
                Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
            }
            int i;
            Gizmos.color = pathColor;
            for (i = 0; i < path.Count; i++)
            {
                v = path[i];
                //Gizmos.DrawSphere(v.transform.position, pathNodeRadius);
                if (smoothPath && i != 0)
                {
                    Vertex prev = path[i - 1];
                    Gizmos.DrawLine(v.transform.position, prev.transform.position);
                }
            }
        }

        // Mostrar el camino calculado
        public void ShowPathVertices(List<Vertex> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Vertex v = path[i];
                Ovillo o = v.gameObject.GetComponentInChildren<Ovillo>();
                o.Show(true);

                //GameObject r = v.gameObject;
                //foreach (Transform target in v.transform)
                //{
                //    if (target.gameObject.CompareTag("Ovillo"))
                //    {
                //        target.gameObject.SetActive(true);
                //    }
                //}
            }
        }

        // Cuantificación, cómo traduce de posiciones del espacio (la pantalla) a nodos
        private GameObject GetNodeFromScreen(Vector3 screenPosition)
        {
            GameObject node = null;
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            foreach (RaycastHit h in hits)
            {
                if (!h.collider.CompareTag(vertexTag) && !h.collider.CompareTag(obstacleTag))
                    continue;
                node = h.collider.gameObject;
                break;
            }
            return node;
        }

        // Dibuja el hilo de Ariadna
        public virtual void DibujaHilo()
        {
            hilo.positionCount = path.Count + 1;
            hilo.SetPosition(0, new Vector3(srcObj.transform.position.x, srcObj.transform.position.y + hiloOffset, srcObj.transform.position.z));

            for (int i = path.Count - 1; i >= 0; i--)
            {
                Vector3 vertexPos = new Vector3(path[i].transform.position.x, path[i].transform.position.y + hiloOffset, path[i].transform.position.z);
                hilo.SetPosition(path.Count - i, vertexPos);
            }

            ShowPathVertices(path);
        }

        void updateAriadna(bool ar)
        {
            ariadna = ar;
            hilo.enabled = ariadna;
            control.enabled = !ariadna;
            seguir.enabled = ariadna;
            seguir.graph = this;
        }

        public void ChangeHeuristic(string heuristica)
        {
            if (heuristica == Heuristic.Manhattan.ToString())
            {
                _currHeuristic = Heuristic.Manhattan;
            }
            else if (heuristica == Heuristic.Euclidea.ToString())
            {
                _currHeuristic = Heuristic.Euclidea;
            }
        }

        float Manhattan(Vertex a, Vertex b)
        {
            return Mathf.Abs(a.transform.position.x - b.transform.position.x) +
                   Mathf.Abs(a.transform.position.z - b.transform.position.z);
        }
        float Euclidean(Vertex a, Vertex b)
        {
            return Vector3.Distance(a.transform.position, b.transform.position);
        }

        public virtual void ResetPath()
        {
            path = null;
        }

        public bool GetAriadna()
        {
            return ariadna;
        }

        public bool GetSmooth()
        {
            return smoothPath;
        }
    }
}
