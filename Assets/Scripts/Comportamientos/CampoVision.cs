using UCM.IAV.Movimiento;
using UnityEngine;

public class CampoVision : MonoBehaviour
{
    private SeguirCamino camino;
    private Merodear merodear;
    private Llegada llegada;
    private SphereCollider sphereCollider;

    [SerializeField]
    float radioVision = 5.0f;
    [SerializeField]
    float angleVision = 5.0f;


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
        Debug.Log("ENTRANDOOOOOOOOO");

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
        Vector3 directionToPlayer = other.GetComponent<Transform>().position - transform.position;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer <= angleVision)
        {
            Debug.Log("TRUETRUETRUETREU");

            if (llegada != null)
            {
                llegada.objetivo = other.gameObject;
                llegada.enabled = true;
            }

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

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("SALIENDOOOO");

        if (llegada != null)
        {
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
        Vector3 ori = OriToVec(angleVision);
        Debug.DrawLine(transform.position, transform.forward);
        Debug.DrawLine(transform.position, transform.forward - ori, new Color(0, 0, 1), 0.2f);
        Debug.DrawLine(transform.position, transform.forward + ori, new Color(0, 0, 1), 0.2f);
    }

    public Vector3 OriToVec(float orientacion)
    {
        Vector3 vector = Vector3.zero;
        vector.x = Mathf.Sin(orientacion * Mathf.Deg2Rad) * 1.0f; //  * 1.0f se a�ade para asegurar que el tipo es float
        vector.z = Mathf.Cos(orientacion * Mathf.Deg2Rad) * 1.0f; //  * 1.0f se a�ade para asegurar que el tipo es float
        return vector.normalized;
    }

}
