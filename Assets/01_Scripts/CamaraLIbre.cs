using UnityEngine;

public class CamaraLibre : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public float velocidadMovimiento = 10f;
    public float sensibilidadRaton = 3f;

    private float rotacionX = 0f;
    private float rotacionY = 0f;

    void Start()
    {
        // Guardamos la rotación inicial de la cámara para que no salte bruscamente
        Vector3 rotacionInicial = transform.localRotation.eulerAngles;
        rotacionX = rotacionInicial.y;
        rotacionY = rotacionInicial.x;
    }

    void Update()
    {
        // 1. GIRAR LA CÁMARA (Solo si mantienes el clic derecho)
        if (Input.GetMouseButton(1))
        {
            rotacionX += Input.GetAxis("Mouse X") * sensibilidadRaton;
            rotacionY -= Input.GetAxis("Mouse Y") * sensibilidadRaton;

            // Limitamos la rotación arriba/abajo para no dar volteretas
            rotacionY = Mathf.Clamp(rotacionY, -90f, 90f);

            transform.localRotation = Quaternion.Euler(rotacionY, rotacionX, 0f);
        }

        // 2. MOVER LA CÁMARA (Con las teclas WASD)
        float movimientoX = Input.GetAxis("Horizontal"); // A, D
        float movimientoZ = Input.GetAxis("Vertical");   // W, S
        float movimientoY = 0f;

        // Subir con E, Bajar con Q
        if (Input.GetKey(KeyCode.E)) movimientoY = 1f;
        if (Input.GetKey(KeyCode.Q)) movimientoY = -1f;

        Vector3 direccion = (transform.right * movimientoX) + (transform.up * movimientoY) + (transform.forward * movimientoZ);
        transform.position += direccion * velocidadMovimiento * Time.deltaTime;
    }
}