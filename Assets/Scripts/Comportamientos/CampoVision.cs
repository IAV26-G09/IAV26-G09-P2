using UCM.IAV.Movimiento;
using UnityEngine;

public class CampoVision : MonoBehaviour
{
    private SeguirCamino camino;
    private Merodear merodear;
    private Llegada llegada;
    private SphereCollider sphereCollider;

    [SerializeField]
    float radioVision = 10.0f;
    [SerializeField]
    float angleVision = 30.0f;


    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.radius = radioVision;
        }

        llegada = GetComponentInParent<Llegada>();
        merodear = GetComponentInParent<Merodear>();
        camino = GetComponentInParent<SeguirCamino>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ENTRANDOOOOOOOOO");

        // ...

        //if (llegada != null)
        //{
        //    llegada.objetivo = other.gameObject;
        //    llegada.enabled = true;
        //}

        //if (merodear != null)
        //{
        //    merodear.enabled = false;
        //}
        //if (camino != null)
        //{
        //    camino.enabled = false;
        //}
    }

    private void OnTriggerStay(Collider other)
    {
        // si es colision con un obstaculo no hace nada
        if (other.GetComponent<Teseo>() == null) return;

        // teseo stay
        Debug.Log("STAYSTAYSTAY");

        // calculo del angulo desde delante
        Vector3 directionToPlayer = other.GetComponent<Transform>().position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // si estas dentro del campo de vision
        if (angleToPlayer <= angleVision)
        {
            // si no hay nada entre el minotauro y el avatar
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, radioVision))
            {
                // si con lo que choca en primera instancia es teseo
                if (hit.collider.GetComponent<Teseo>() != null)
                {
                    Debug.Log("PERSIGUIENDOOO");

                    // activamos persecucion
                    if (llegada != null)
                    { 
                        llegada.objetivo = other.gameObject;
                        llegada.enabled = true;
                    }

                    // desactivamos cualquier otro tipo de comportamiento
                    if (merodear != null)
                    {
                        merodear.enabled = false;
                    }
                    if (camino != null)
                    {
                        camino.enabled = false;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("SALIENDOOOO");

        if (llegada != null)
        {
            Debug.Log("DEJANDO DE PERSEGUIR");
            llegada.objetivo = null;
            llegada.enabled = false;
        }
        if (merodear != null)
        {
            merodear.enabled = true;
        }
        if (camino != null)
        {
            camino.enabled = true;
        }
    }

    private void Update()
    {
        Transform t = GetComponentInParent<Transform>();
        float r = radioVision / 2;
        float a = angleVision / 2;

        Vector3 v1 = Vector3.RotateTowards(t.forward, t.right * -1, a * Mathf.Deg2Rad, 0);
        Vector3 v2 = Vector3.RotateTowards(t.forward, t.right, a * Mathf.Deg2Rad, 0);

        Debug.DrawRay(t.position, t.forward * r, new Color(1,1,1), 0.2f);
        Debug.DrawRay(t.position, v1 * r, new Color(1,1,0), 0.2f);
        Debug.DrawRay(t.position, v2 * r, new Color(1,1,0), 0.2f);
    }
}
