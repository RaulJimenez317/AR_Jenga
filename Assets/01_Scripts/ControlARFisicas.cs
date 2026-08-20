using UnityEngine;
using System.Collections;

public class ControlARFisicas : MonoBehaviour
{
    [Header("Configuración")]
    public float tiempoParaEstabilizar = 0.75f;

    private Rigidbody[] bloques;
    private Coroutine rutinaActivacion;

    void Awake()
    {
        // Busca todos los Rigidbody que están dentro del ImageTarget.
        bloques = GetComponentsInChildren<Rigidbody>(true);

        // Al iniciar, la torre NO debe tener físicas.
        CongelarTorre();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSeguimientoAR(false);
        }
    }

    // Se llamará cuando Vuforia encuentre la imagen.
    public void TargetEncontrado()
    {
        if (rutinaActivacion != null)
        {
            StopCoroutine(rutinaActivacion);
        }

        rutinaActivacion =
            StartCoroutine(ActivarDespuesDeEstabilizar());
    }

    private IEnumerator ActivarDespuesDeEstabilizar()
    {
        // Esperamos un momento para que la pose AR se estabilice.
        yield return new WaitForSeconds(tiempoParaEstabilizar);

        foreach (Rigidbody rb in bloques)
        {
            if (rb == null)
                continue;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = false;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSeguimientoAR(true);
        }

        Debug.Log("AR estable. Físicas de Jenga activadas.");

        rutinaActivacion = null;
    }

    // Se llamará cuando Vuforia pierda la imagen.
    public void TargetPerdido()
    {
        if (rutinaActivacion != null)
        {
            StopCoroutine(rutinaActivacion);
            rutinaActivacion = null;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetSeguimientoAR(false);
        }

        CongelarTorre();

        Debug.Log("Target perdido. Torre congelada.");
    }

    private void CongelarTorre()
    {
        foreach (Rigidbody rb in bloques)
        {
            if (rb == null)
                continue;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.isKinematic = true;
        }
    }
}