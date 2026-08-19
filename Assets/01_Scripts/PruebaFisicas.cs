using UnityEngine;

public class PruebaFisicas : MonoBehaviour
{
    [Header("Fuerza Progresiva")]
    public float fuerzaInicial = 2000f;
    public float fuerzaMaxima = 30000f;
    public float velocidadDeCarga = 20000f;
    private float fuerzaActual;

    [Header("Reglas y Colores")]
    public float alturaMinimaPermitida = -1f;
    public Color colorIluminado = Color.green;
    private Color colorOriginal;

    private GameObject bloqueMirado;
    private GameObject bloqueSeleccionado;

    void Update()
    {
        Ray rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit golpe;

        // 1. ILUMINAR (HOVER)
        if (bloqueSeleccionado == null)
        {
            if (Physics.Raycast(rayo, out golpe))
            {
                Rigidbody rb = golpe.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    GameObject bloqueTocado = golpe.collider.gameObject;
                    if (bloqueTocado.transform.position.y >= alturaMinimaPermitida)
                    {
                        if (bloqueMirado != null && bloqueMirado != bloqueTocado)
                        {
                            bloqueMirado.GetComponent<Renderer>().material.color = colorOriginal;
                        }

                        if (bloqueMirado != bloqueTocado)
                        {
                            bloqueMirado = bloqueTocado;
                            colorOriginal = bloqueMirado.GetComponent<Renderer>().material.color;
                            bloqueMirado.GetComponent<Renderer>().material.color = colorIluminado;
                        }
                    }
                }
            }
            else
            {
                if (bloqueMirado != null)
                {
                    bloqueMirado.GetComponent<Renderer>().material.color = colorOriginal;
                    bloqueMirado = null;
                }
            }
        }

        // 2. CLIC IZQUIERDO: SELECCIONAR Y EMPEZAR A EMPUJAR
        if (Input.GetMouseButtonDown(0))
        {
            if (bloqueMirado != null)
            {
                bloqueSeleccionado = bloqueMirado;
                fuerzaActual = fuerzaInicial;
            }
        }

        // 3. MANTENER CLIC: Acelerar fuerza para sacar la pieza
        if (Input.GetMouseButton(0) && bloqueSeleccionado != null)
        {
            Rigidbody rbSeleccionado = bloqueSeleccionado.GetComponent<Rigidbody>();
            if (rbSeleccionado != null)
            {
                fuerzaActual += velocidadDeCarga * Time.deltaTime;
                if (fuerzaActual > fuerzaMaxima) fuerzaActual = fuerzaMaxima;

                Vector3 direccionEmpuje = rayo.direction;
                rbSeleccionado.AddForce(direccionEmpuje * fuerzaActual * Time.deltaTime);
            }
        }

        // 4. SOLTAR CLIC: LIBERAR LA PIEZA SIN TELETRANSPORTES
        if (Input.GetMouseButtonUp(0))
        {
            if (bloqueSeleccionado != null)
            {
                // Restauramos su color
                if (bloqueSeleccionado != bloqueMirado)
                {
                    bloqueSeleccionado.GetComponent<Renderer>().material.color = colorOriginal;
                }

                // Simplemente soltamos la pieza de nuestra mano virtual
                bloqueSeleccionado = null;
            }
        }
    }
}