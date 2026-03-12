using UCM.IAV.Movimiento;
using UnityEngine;

public class CampoVision : MonoBehaviour
{
    private Patrullar patrullar;
    private Vigilar vigilar;
    private Llegada llegada;
    private SphereCollider sphereCollider;

    [SerializeField]
    private float radioVision = 10.0f;
    [SerializeField]
    [Range(0.0f, 180.0f)] // para evitar que los minotauros puedan ver mas alla de un angulo de vision de 180 grados
    private float angleVision = 30.0f;
    [SerializeField] 
    private bool debug = true;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null) sphereCollider.radius = radioVision;

        llegada = GetComponentInParent<Llegada>();
        vigilar = GetComponentInParent<Vigilar>();
        patrullar = GetComponentInParent<Patrullar>();
    }

    private void OnTriggerStay(Collider other)
    {
        // si es colision con un obstaculo no hace nada
        if (other.GetComponent<Teseo>() == null) return;

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
                    // activamos seguimiento
                    if (llegada != null)
                    { 
                        llegada.objetivo = other.gameObject;
                        llegada.enabled = true;
                    }

                    // desactivamos cualquier otro tipo de comportamiento
                    if (vigilar != null)
                    {
                        vigilar.enabled = false;
                    }
                    if (patrullar != null)
                    {
                        patrullar.enabled = false;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // activamos seguimiento
        if (llegada != null)
        {
            llegada.objetivo = null;
            llegada.enabled = false;
        }
        // activamos cualquier otro tipo de comportamiento
        if (vigilar != null)
        {
            vigilar.enabled = true;
        }
        if (patrullar != null)
        {
            patrullar.enabled = true;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // Debug
        if (debug)
        {
            Transform t = GetComponentInParent<Transform>();
            float r = radioVision / 2;
            float a = angleVision / 2;

            Vector3 v1 = Vector3.RotateTowards(t.forward, t.right * -1, a * Mathf.Deg2Rad, 0);
            Vector3 v2 = Vector3.RotateTowards(t.forward, t.right, a * Mathf.Deg2Rad, 0);

            Debug.DrawRay(t.position, t.forward * r, new Color(1, 1, 1), 0.2f);
            Debug.DrawRay(t.position, v1 * r, new Color(1, 1, 0), 0.2f);
            Debug.DrawRay(t.position, v2 * r, new Color(1, 1, 0), 0.2f);
        }
#endif
    }
}