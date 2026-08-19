using UnityEngine;

public class SueloJenga : MonoBehaviour
{
    void OnCollisionEnter(Collision choque)
    {
        // Revisamos si lo que choc� con el suelo es un bloque (tiene Rigidbody)
        Rigidbody rbBloque = choque.gameObject.GetComponent<Rigidbody>();

        if (rbBloque != null)
        {
            // 1. Frenamos el bloque en seco para que no rebote
            rbBloque.linearVelocity = Vector3.zero;
            rbBloque.angularVelocity = Vector3.zero;

            // 2. Escaneamos la torre para buscar la pieza m�s alta
            float alturaMaxima = -100f;
            GameObject bloqueMasAlto = null;

            // Buscamos todos los bloques de la escena
            Rigidbody[] todosLosBloques = FindObjectsOfType<Rigidbody>();
            foreach (Rigidbody rb in todosLosBloques)
            {
                // Ignoramos el bloque que se acaba de caer
                if (rb.gameObject != choque.gameObject)
                {
                    if (rb.transform.position.y > alturaMaxima)
                    {
                        alturaMaxima = rb.transform.position.y;
                        bloqueMasAlto = rb.gameObject;
                    }
                }
            }

            // 3. Magia: Teletransportamos el bloque arriba del todo
            if (bloqueMasAlto != null)
            {
                // Lo subimos 0.4 unidades (la altura exacta de un piso)
                float nuevaAltura = alturaMaxima + 0.4f;

                // Leemos c�mo est� rotado el bloque de abajo para cruzar este a 90 grados
                float rotacionAbajo = bloqueMasAlto.transform.eulerAngles.y;
                float nuevaRotacionY = (rotacionAbajo < 45f || rotacionAbajo > 315f) ? 90f : 0f;

                // Lo colocamos exactamente en el centro (X: 0, Z: 0)
                choque.gameObject.transform.position = new Vector3(0, nuevaAltura, 0);
                choque.gameObject.transform.rotation = Quaternion.Euler(0, nuevaRotacionY, 0);
            }
        }
    }
}