using UnityEngine;

public class BloqueJenga : MonoBehaviour
{
    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;

    // Posición con la que el bloque comenzó la partida
    private Vector3 posicionInicialPartida;

    public bool estaSiendoMovido = false;

    void Start()
    {
        posicionInicialPartida = transform.position;
    }

    // Guarda dónde estaba antes del movimiento actual
    public void GuardarPosicion()
    {
        posicionOriginal = transform.position;
        rotacionOriginal = transform.rotation;
    }

    // Devuelve el bloque si el movimiento fue inválido
    public void VolverAPosicionOriginal()
    {
        transform.position = posicionOriginal;
        transform.rotation = rotacionOriginal;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        estaSiendoMovido = false;
    }

    // Indica cuánto cayó respecto a su posición inicial
    public float CuantoHaCaido()
    {
        return posicionInicialPartida.y - transform.position.y;
    }
}