/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Navegacion
{

    using System.Collections;
    using System.Collections.Generic;
    using UCM.IAV.Movimiento;
    using UnityEngine;

    /// <summary>
    /// Abstract class for graphs
    /// </summary>
    public abstract class Graph : MonoBehaviour
    {
        // Aquí el grafo entero es representado con estas listas, que luego puede aprovechar el algoritmo A*.
        // El pseudocódigo de Millington no asume que tengamos toda la información del grafo representada y
        // por eso va guardando registros de los nodos que visita... pero si nos es posible,
        // OPCIONALMENTE podemos usar estas variables como una CACHÉ donde tener toda la información
        public GameObject vertexPrefab;
        protected List<Vertex> vertices;
        protected List<List<Vertex>> neighbourVertex;
        protected List<List<float>> costs;
        protected bool[,] mapVertices;
        protected float[,] costsVertices; // Costes reales (g)... aunque también se podría crear una clase
                                          // para las conexiones y poner los costes ahí,
                                          // como en el pseudocódigo de Millington (NodeRecord).
                                          // Esto está 'optimizado' porque sabemos que trabajamos con una rejilla...
        protected int numCols, numRows;

        // Esto de la heurística es para algoritmos de búsqueda con estrategias informadas como A*, naturalmente.
        // Un delegado especifica la cabecera de una función, la que sea, que cumpla con esos parámetros y
        // devuelva ese tipo.
        // Cuidado al implementarlas, porque no puede ser que la distancia -por ejemplo-
        // entre dos casillas tenga una heurística más cara que el coste real de navegar de una a otra.
        public delegate float Heuristic(Vertex a, Vertex b);

        // Used for getting path in frames
        public List<Vertex> path;


        public virtual void Start()
        {
            Load();
        }

        public virtual void Load() { }

        public virtual int GetSize()
        {
            if (ReferenceEquals(vertices, null))
                return 0;
            return vertices.Count;
        }

        public virtual void UpdateVertexCost(Vector3 position, float costMultipliyer) { }

        public virtual Vertex GetNearestVertex(Vector3 position)
        {
            return null;
        }

        public virtual GameObject GetRandomPos()
        {
            return null;
        }

        public virtual Vector3 GetVertexPos(Vertex v)
        {
            return new Vector3();
        }

        public virtual Vertex[] GetNeighbours(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new Vertex[0];
            return neighbourVertex[v.id].ToArray();
        }

        public virtual float[] GetNeighboursCosts(Vertex v)
        {
            if (ReferenceEquals(neighbourVertex, null) || neighbourVertex.Count == 0 ||
                v.id < 0 || v.id >= neighbourVertex.Count)
                return new float[0];

            Vertex[] neighs = neighbourVertex[v.id].ToArray();
            float[] costsV = new float[neighs.Length];
            for (int neighbour = 0; neighbour < neighs.Length; neighbour++) {
                int j = (int)Mathf.Floor(neighs[neighbour].id / numCols);
                int i = (int)Mathf.Floor(neighs[neighbour].id % numCols);
                costsV[neighbour] = costsVertices[j, i];
            }

            return costsV;
        }

        // Encuentra caminos óptimos
        public List<Vertex> GetPathBFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO BFS
            return new List<Vertex>();
        }

        // No encuentra caminos óptimos
        public List<Vertex> GetPathDFS(GameObject srcO, GameObject dstO)
        {
            // IMPLEMENTAR ALGORITMO DFS
            return new List<Vertex>();
        }

        public List<Vertex> GetPathAstar(GameObject srcO, GameObject dstO, Heuristic h = null)
        {
            // creamos las listas de nodos abiertos y cerrados
            BinaryHeap<Vertex> open = new BinaryHeap<Vertex>(); // nodos que vamos conociendo y que seran expandidos (si no son el nodo dst)
            BinaryHeap<Vertex> closed = new BinaryHeap<Vertex>(); // vamos guardando los que vamos visitando

            // cogemos los vertices reales asociados a los objetos
            Vertex start = GetNearestVertex(srcO.transform.position); // vertice del que partimos
            Vertex goal = GetNearestVertex(dstO.transform.position); // vertice al que vamos

            float[] gCost = new float[vertices.Count]; // coste mas barato que conocemos desde el start hasta un nodo n
            float[] fCost = new float[vertices.Count]; // gCost + heuristica
            // array del nodo anterior a cada uno por el camino mas barato, usado para reconstruir el camino
            int[] prev = new int[vertices.Count];

            // inicializacion de listas a valores predeterminados infinitos
            for (int i = 0; i < vertices.Count; i++)
            {
                gCost[i] = Mathf.Infinity;
                fCost[i] = Mathf.Infinity;
                prev[i] = -1; // vacio
            }

            gCost[start.id] = 0;
            fCost[start.id] = h(start, goal);
            start.cost = fCost[start.id];

            // aniadimos el vertice origen a visitar
            open.Add(start);

            // iteramos por los vertices
            while (open.Count > 0) // mientras queden nodos abiertos
            {
                // miramos el primero en la lista: el elemento de menor coste (pq ya se ordenan por si solos por coste e id en principio)
                // y lo quitamos (ya lo hemos "expandido"
                Vertex act = open.Remove();

                // si hemos llegado
                if (act == goal) 
                    return BuildPath(start.id, goal.id, ref prev); // devuelve el camino reconstruido

                Vertex[] neighbours = GetNeighbours(act);
                float[] neighboursCosts = GetNeighboursCosts(act);

                // recorremos todos los vecinos del actual
                for (int i = 0; i < neighbours.Length; i++)
                {
                    Vertex neighbor = neighbours[i];

                    // coste g de start al vecino PASANDO por act
                    float gProbado = gCost[act.id] + neighboursCosts[i];

                    if (gProbado < gCost[neighbor.id]) // si la tentativa de coste es menor que [infinito] (en un principio) -> lo actualizas
                    {
                        // este camino a neighbor es mejor que el anterior asi que lo guardamos ->
                        prev[neighbor.id] = act.id; // el anterior al neighbor es el actual
                        gCost[neighbor.id] = gProbado; // actualizamos coste
                        fCost[neighbor.id] = gProbado + h(neighbor, goal);
                        neighbor.cost = fCost[neighbor.id];

                        if (!open.Contains(neighbor)) // si neighbor no esta en open lo metemos
                        {
                            open.Add(neighbor);
                        }
                    }
                }
            }

            // devolvemos una lista por defecto vacia, no se ha alcanzado el objetivo
            return new List<Vertex>();
        }

        public List<Vertex> Smooth(List<Vertex> inputPath)
        {
            List<Vertex> outputPath = new List<Vertex>();

            if (inputPath.Count <= 1) // si hay 2 o menos nodos no se puede suavizar
            {
                return inputPath;
            }

            // se asume que los dos primeros van a pasar el test del raycast
            outputPath.Add(inputPath[0]);

            int i = 0;
            int j = 1;
            
            while (i < inputPath.Count - 1 && j < inputPath.Count)
            {
                Vertex a = inputPath[i];
                Vertex b = inputPath[j];
                if (RayClear(a, b, 0.42f))
                {
                    j++;
                    if (j == inputPath.Count - 1)
                    {
                        i = inputPath.Count - 1;
                    }
                }
                else
                {
                    i = j - 1;
                    outputPath.Add(inputPath[i]);
                    j++;
                }
            }

            Vector3 playerPos = GameManager.instance.GetPlayer().transform.position;
            Vector3 lastPos = GetVertexPos(outputPath[outputPath.Count - 1]);

            int layerMask = 1 << 6;
            Vector3 dir = lastPos - playerPos;
           
            //if (Physics.Raycast(playerPos, dir.normalized, dir.magnitude, layerMask))
            //{
            //    outputPath.Add(inputPath[inputPath.Count - 1]);
            //}

            if (Physics.SphereCast(
                playerPos, 
                0.42f,
                dir.normalized, 
                out RaycastHit hitInfo,
                dir.magnitude, 
                layerMask
            ))
            {
                outputPath.Add(inputPath[inputPath.Count - 1]);
            }

            return outputPath; 
        }

        // true si no se ha chocado con nada, el raycast sale "limpio"
        //private bool RayClear(Vertex a, Vertex b)
        //{
        //    Vector3 posA = GetVertexPos(a) + gameObject.transform.up;
        //    Vector3 posB = GetVertexPos(b) + gameObject.transform.up;
        //    Vector3 dirVertex = posB - posA;

        //    int layerMask = 1 << 6;
        //    bool hit1 = (Physics.Raycast(posA, dirVertex.normalized, dirVertex.magnitude, layerMask));

        //    Color c1 = Color.green;
        //    if (hit1) c1 = Color.red;
        //    Debug.DrawLine(posA, posB, c1);

        //    return !(hit1);
        //}

        private bool RayClear(Vertex a, Vertex b, float radius)
        {
            Vector3 posA = GetVertexPos(a) + Vector3.up;
            Vector3 posB = GetVertexPos(b) + Vector3.up;

            Vector3 dir = posB - posA;
            int layerMask = 1 << 6;

            bool hit = Physics.SphereCast(
                posA,
                radius,
                dir.normalized,
                out RaycastHit hitInfo,
                dir.magnitude,
                layerMask
            );

            Color c1 = Color.green;
            if (hit) c1 = Color.red;
            Debug.DrawLine(posA, posB, c1);

            return !hit;
        }

        // Reconstruir el camino, dando la vuelta a la lista de nodos 'padres' /previos que hemos ido anotando
        private List<Vertex> BuildPath(int srcId, int dstId, ref int[] prevList)
        {
            List<Vertex> path = new List<Vertex>();

            if (dstId < 0 || dstId >= vertices.Count) 
                return path;

            int prev = dstId;
            do
            {
                path.Add(vertices[prev]);
                prev = prevList[prev];
            } while (prev != srcId);
            return path;
        }
    }
}
