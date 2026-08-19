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

    [Header("Sonido de selección")]
    public AudioSource audioSeleccion;
    public float duracionSonido = 2f;
    public float tiempoEntreSonidos = 2f;

    private float ultimoSonido = -Mathf.Infinity;

    void Update()
    {


        bool hayInteraccion = false;
        bool empezoAInteractuar = false;
        bool terminoDeInteractuar = false;

        Vector3 posicionPuntero = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);

            posicionPuntero = toque.position;
            hayInteraccion = true;

            if (toque.phase == TouchPhase.Began)
                empezoAInteractuar = true;

            if (toque.phase == TouchPhase.Ended ||
                toque.phase == TouchPhase.Canceled)
                terminoDeInteractuar = true;
        }
        // PC
        else
        {
            if (Input.GetMouseButton(0))
                hayInteraccion = true;

            if (Input.GetMouseButtonDown(0))
                empezoAInteractuar = true;

            if (Input.GetMouseButtonUp(0))
                terminoDeInteractuar = true;
        }



        if (Camera.main == null)
            return;

        Ray rayo = Camera.main.ScreenPointToRay(posicionPuntero);
        RaycastHit golpe;

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
                        if (bloqueMirado != null &&
                            bloqueMirado != bloqueTocado)
                        {
                            bloqueMirado.GetComponent<Renderer>().material.color =
                                colorOriginal;
                        }

                        if (bloqueMirado != bloqueTocado)
                        {
                            bloqueMirado = bloqueTocado;

                            colorOriginal =
                                bloqueMirado.GetComponent<Renderer>().material.color;

                            bloqueMirado.GetComponent<Renderer>().material.color =
                                colorIluminado;
                        }
                    }
                }
            }
            else
            {
                if (bloqueMirado != null)
                {
                    bloqueMirado.GetComponent<Renderer>().material.color =
                        colorOriginal;

                    bloqueMirado = null;
                }
            }
        }



        if (empezoAInteractuar)
        {
            if (bloqueMirado != null)
            {
                bloqueSeleccionado = bloqueMirado;
                fuerzaActual = fuerzaInicial;

                // SONIDO
                if (audioSeleccion != null &&
                    Time.time - ultimoSonido >= tiempoEntreSonidos)
                {
                    audioSeleccion.Play();

                    ultimoSonido = Time.time;

                    CancelInvoke(nameof(DetenerSonido));
                    Invoke(nameof(DetenerSonido), duracionSonido);
                }
            }
        }



        if (hayInteraccion && bloqueSeleccionado != null)
        {
            Rigidbody rbSeleccionado =
                bloqueSeleccionado.GetComponent<Rigidbody>();

            if (rbSeleccionado != null)
            {
                fuerzaActual += velocidadDeCarga * Time.deltaTime;

                if (fuerzaActual > fuerzaMaxima)
                    fuerzaActual = fuerzaMaxima;

                Vector3 direccionEmpuje = rayo.direction;

                rbSeleccionado.AddForce(
                    direccionEmpuje *
                    fuerzaActual *
                    Time.deltaTime
                );
            }
        }



        if (terminoDeInteractuar)
        {
            if (bloqueSeleccionado != null)
            {
                if (bloqueSeleccionado != bloqueMirado)
                {
                    bloqueSeleccionado.GetComponent<Renderer>().material.color =
                        colorOriginal;
                }

                bloqueSeleccionado = null;
            }
        }
    }



    private void DetenerSonido()
    {
        if (audioSeleccion != null)
        {
            audioSeleccion.Stop();
        }
    }
}