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
using UnityEngine.UI;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public bool mino = false;

    enum DropdownType
    {
        SIZE,
        PATRULLEROS,
        ESTATICOS
    };

    [SerializeField]
    DropdownType type = 0;

    void Start()
    {
       // Establece changeSize al OnValueChanged del Dropdown
        if(type == DropdownType.SIZE)
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.ChangeSize(); });
       else if(type == DropdownType.PATRULLEROS)
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.setNumMinosPatrulleros(); });
       else
            gameObject.GetComponent<Dropdown>().onValueChanged.AddListener(delegate { UCM.IAV.Movimiento.GameManager.instance.setNumMinosEstaticos(); });
    }
}
