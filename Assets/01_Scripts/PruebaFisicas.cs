using UnityEngine;

public class PruebaFisicas : MonoBehaviour
{
    [Header("Fuerza del 'Dedo'")]
    public float fuerzaEmpuje = 300f; // Puedes subir o bajar este número en el Inspector

    void Update()
    {
        // Detecta el clic izquierdo del ratón
        if (Input.GetMouseButtonDown(0))
        {
            // Dispara un rayo invisible desde la cámara hacia el puntero del ratón
            Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit golpe;

            // Si el rayo choca con algún objeto en la escena...
            if (Physics.Raycast(rayo, out golpe))
            {
                // Buscamos si el objeto tocado tiene el componente Rigidbody (físicas)
                Rigidbody rb = golpe.collider.GetComponent<Rigidbody>();

                // Si tiene Rigidbody, significa que es un bloque (la base no lo tiene)
                if (rb != null)
                {
                    // Empujamos el bloque en la misma dirección hacia la que mira la cámara
                    Vector3 direccionEmpuje = rayo.direction;
                    rb.AddForce(direccionEmpuje * fuerzaEmpuje);

                    Debug.Log("¡Pum! Empujaste el bloque: " + golpe.collider.gameObject.name);
                }
                else
                {
                    Debug.Log("Tocaste algo que no tiene físicas (como la base o el aire).");
                }
            }
        }
    }
}