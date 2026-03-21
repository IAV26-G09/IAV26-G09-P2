/*    
   Copyright (C) 2020-2023 Federico Peinado
   http://www.federicopeinado.com
   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).
   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Movimiento;
using UnityEngine;

namespace UCM.IAV.Navegacion
{
   
    public class MinoManager : MonoBehaviour
    {
        public GameObject minotaur;

        private Graph graph;

        public int numMinosPatrulleros = 0;
        public int numMinosEstaticos = 0;

        enum MinoType
        {
            PATRULLEROS,
            ESTATICOS
        };

        private void Start()
        {
            numMinosPatrulleros = GameManager.instance.getNumMinosPatrulleros();
            numMinosEstaticos = GameManager.instance.getNumMinosEstaticos();
            StartUp();
        }

        void StartUp()
        {
            GameObject graphGO = GameObject.Find("GraphGrid");

            if (graphGO != null)
                graph = graphGO.GetComponent<GraphGrid>();

            for (int i = 0; i < numMinosPatrulleros; i++)
                GenerateMino(MinoType.PATRULLEROS);

            for (int i = 0; i < numMinosEstaticos; i++)
                GenerateMino(MinoType.ESTATICOS);
        }

        void GenerateMino(MinoType type)
        {
            GameObject minoGO = Instantiate(minotaur, graph.GetRandomPos().transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
            if (type == MinoType.PATRULLEROS)
            {
                // patrullero con llegada
                //SeguirCamino cam = minoGO.AddComponent<SeguirCamino>();
                //cam.enabled = false;
                Patrullar patrulla = minoGO.AddComponent<Patrullar>();
                patrulla.graph = graph;
            }
            else
            {
                // estatico con merodeo
                minoGO.AddComponent<Vigilar>();

            }
            InfluenceCollision col = minoGO.GetComponent<InfluenceCollision>();
            if (col != null)
            {
                col.graph = graph;
            }
            GameManager.instance.AddCameraTarget(minoGO.transform);
        }
    }
}
