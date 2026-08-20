using UnityEngine;

public class PruebaFisicas : MonoBehaviour
{
    [Header("Reglas y Colores")]
    public float alturaMinimaPermitida = -1f;
    public Color colorIluminado = Color.green;

    private Color colorOriginal;

    private GameObject bloqueMirado;
    private GameObject bloqueSeleccionado;

    [Header("Movimiento del bloque")]
    private float distanciaArrastre;
    private Rigidbody rigidbodySeleccionado;

    [Header("Sonido de selección")]
    public AudioSource audioSeleccion;
    public float duracionSonido = 2f;
    public float tiempoEntreSonidos = 2f;

    private float ultimoSonido = -Mathf.Infinity;

    void Update()
    {
        // ========================================
        // SI EL JUEGO TERMINÓ, BLOQUEAMOS TODO
        // ========================================

        if (GameManager.Instance != null &&
            GameManager.Instance.JuegoTerminado)
        {
            if (bloqueSeleccionado != null)
            {
                BloqueJenga bloqueJenga =
                    bloqueSeleccionado.GetComponent<BloqueJenga>();

                if (bloqueJenga != null)
                {
                    bloqueJenga.estaSiendoMovido = false;
                }

                Rigidbody rb =
                    bloqueSeleccionado.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                Renderer renderer =
                    bloqueSeleccionado.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color = colorOriginal;
                }

                bloqueSeleccionado = null;
                rigidbodySeleccionado = null;
            }

            LimpiarBloqueMirado();

            return;
        }

         // ========================================
    // BLOQUEAR SI EL AR NO ESTÁ ESTABLE
    // ========================================

    if (GameManager.Instance != null &&
        !GameManager.Instance.SeguimientoAREstable)
    {
        LimpiarBloqueMirado();
        return;
    }

        // ========================================
        // INPUT
        // ========================================

        bool hayInteraccion = false;
        bool empezoAInteractuar = false;
        bool terminoDeInteractuar = false;

        Vector3 posicionPuntero = Input.mousePosition;

        // ========================================
        // TOUCH
        // ========================================

        if (Input.touchCount > 0)
        {
            Touch toque = Input.GetTouch(0);

            posicionPuntero = toque.position;

            if (toque.phase == TouchPhase.Began)
            {
                hayInteraccion = true;
                empezoAInteractuar = true;
            }

            if (toque.phase == TouchPhase.Moved ||
                toque.phase == TouchPhase.Stationary)
            {
                hayInteraccion = true;
            }

            if (toque.phase == TouchPhase.Ended ||
                toque.phase == TouchPhase.Canceled)
            {
                terminoDeInteractuar = true;
            }
        }

        // ========================================
        // PC
        // ========================================

        else
        {
            if (Input.GetMouseButton(0))
            {
                hayInteraccion = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                empezoAInteractuar = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                terminoDeInteractuar = true;
            }
        }

        // ========================================
        // CÁMARA
        // ========================================

        if (Camera.main == null)
            return;

        Ray rayo =
            Camera.main.ScreenPointToRay(posicionPuntero);

        RaycastHit golpe;

        // ========================================
        // DETECTAR BLOQUE
        // ========================================

        if (bloqueSeleccionado == null)
        {
            if (Physics.Raycast(rayo, out golpe))
            {
                Rigidbody rb =
                    golpe.collider.GetComponent<Rigidbody>();

                BloqueJenga bloqueJenga =
                    golpe.collider.GetComponent<BloqueJenga>();

                // Solamente aceptamos piezas del Jenga.
                if (rb != null && bloqueJenga != null)
                {
                    GameObject bloqueTocado =
                        golpe.collider.gameObject;

                    bool alturaPermitida =
                        bloqueTocado.transform.position.y >=
                        alturaMinimaPermitida;

                    bool esNivelSuperior =
                        EsNivelSuperior(bloqueTocado);

                    // El nivel superior NO puede retirarse.
                    if (alturaPermitida && !esNivelSuperior)
                    {
                        MarcarBloque(bloqueTocado);
                    }
                    else
                    {
                        LimpiarBloqueMirado();
                    }
                }
                else
                {
                    LimpiarBloqueMirado();
                }
            }
            else
            {
                LimpiarBloqueMirado();
            }
        }

        // ========================================
        // AGARRAR BLOQUE
        // ========================================

        if (empezoAInteractuar)
        {
            if (bloqueMirado != null)
            {
                bloqueSeleccionado = bloqueMirado;

                // Registramos qué jugador empezó a mover.
                // Así, si después la torre cae,
                // pierde el jugador correcto.
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.RegistrarInicioMovimiento();
                }

                BloqueJenga bloqueJenga =
                    bloqueSeleccionado.GetComponent<BloqueJenga>();

                if (bloqueJenga != null)
                {
                    // Recordamos dónde estaba.
                    bloqueJenga.GuardarPosicion();

                    bloqueJenga.estaSiendoMovido = true;
                }

                rigidbodySeleccionado =
                    bloqueSeleccionado.GetComponent<Rigidbody>();

                distanciaArrastre =
                    Vector3.Distance(
                        Camera.main.transform.position,
                        bloqueSeleccionado.transform.position
                    );

                // Mientras está agarrado, desactivamos
                // temporalmente las físicas.
                if (rigidbodySeleccionado != null)
                {
                    rigidbodySeleccionado.linearVelocity =
                        Vector3.zero;

                    rigidbodySeleccionado.angularVelocity =
                        Vector3.zero;

                    rigidbodySeleccionado.isKinematic = true;
                }

                // ========================================
                // SONIDO
                // ========================================

                if (audioSeleccion != null &&
                    Time.time - ultimoSonido >= tiempoEntreSonidos)
                {
                    audioSeleccion.Play();

                    ultimoSonido = Time.time;

                    CancelInvoke(nameof(DetenerSonido));

                    Invoke(
                        nameof(DetenerSonido),
                        duracionSonido
                    );
                }
            }
        }

        // ========================================
        // ARRASTRAR BLOQUE
        // ========================================

        if (hayInteraccion &&
            bloqueSeleccionado != null)
        {
            Vector3 nuevaPosicion =
                rayo.GetPoint(distanciaArrastre);

            bloqueSeleccionado.transform.position =
                nuevaPosicion;
        }

        // ========================================
        // SOLTAR BLOQUE
        // ========================================

        if (terminoDeInteractuar)
        {
            if (bloqueSeleccionado != null)
            {
                BloqueJenga bloqueJenga =
                    bloqueSeleccionado.GetComponent<BloqueJenga>();

                if (bloqueJenga != null)
                {
                    bloqueJenga.estaSiendoMovido = false;

                    // ========================================
                    // MOVIMIENTO INVÁLIDO
                    // ========================================

                    if (!EstaColocadoArriba(bloqueSeleccionado))
                    {
                        bloqueJenga.VolverAPosicionOriginal();

                        Debug.Log(
                            "Movimiento inválido. " +
                            "El bloque vuelve a su lugar."
                        );
                    }

                    // ========================================
                    // MOVIMIENTO VÁLIDO
                    // ========================================

                    else
                    {
                        Debug.Log(
                            "Movimiento válido. " +
                            "Bloque colocado arriba."
                        );

                        // Solamente un movimiento válido
                        // cambia el turno.
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.CambiarTurno();
                        }
                    }
                }

                // ========================================
                // DEVOLVER FÍSICA
                // ========================================

                if (rigidbodySeleccionado != null)
                {
                    rigidbodySeleccionado.isKinematic = false;

                    rigidbodySeleccionado.linearVelocity =
                        Vector3.zero;

                    rigidbodySeleccionado.angularVelocity =
                        Vector3.zero;

                    rigidbodySeleccionado = null;
                }

                // Restauramos el color de la pieza.
                Renderer renderer =
                    bloqueSeleccionado.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material.color =
                        colorOriginal;
                }

                bloqueSeleccionado = null;
                bloqueMirado = null;
            }
        }
    }

    // ========================================
    // ¿ES DEL NIVEL SUPERIOR?
    // ========================================

    private bool EsNivelSuperior(GameObject bloque)
    {
        BloqueJenga[] bloques =
            FindObjectsByType<BloqueJenga>(
                FindObjectsSortMode.None
            );

        if (bloques.Length == 0)
            return false;

        float alturaMaxima = float.MinValue;

        foreach (BloqueJenga b in bloques)
        {
            if (b.transform.position.y > alturaMaxima)
            {
                alturaMaxima =
                    b.transform.position.y;
            }
        }

        // Tolerancia por pequeñas diferencias
        // causadas por las físicas.
        float tolerancia = 0.15f;

        return bloque.transform.position.y >=
               alturaMaxima - tolerancia;
    }

    // ========================================
    // ¿ESTÁ COLOCADO ARRIBA?
    // ========================================

    private bool EstaColocadoArriba(GameObject bloque)
    {
        BloqueJenga[] bloques =
            FindObjectsByType<BloqueJenga>(
                FindObjectsSortMode.None
            );

        float alturaMaxima = float.MinValue;

        float centroX = 0f;
        float centroZ = 0f;

        int cantidad = 0;

        foreach (BloqueJenga b in bloques)
        {
            // Ignoramos la pieza que estamos comprobando.
            if (b.gameObject == bloque)
                continue;

            if (b.transform.position.y > alturaMaxima)
            {
                alturaMaxima =
                    b.transform.position.y;
            }

            centroX += b.transform.position.x;
            centroZ += b.transform.position.z;

            cantidad++;
        }

        if (cantidad == 0)
            return false;

        centroX /= cantidad;
        centroZ /= cantidad;

        // Debe estar por encima del bloque
        // más alto de la torre.
        bool estaArriba =
            bloque.transform.position.y >=
            alturaMaxima + 0.15f;

        // Además debe estar cerca del centro
        // de la torre.
        float distanciaHorizontal =
            Vector2.Distance(
                new Vector2(
                    bloque.transform.position.x,
                    bloque.transform.position.z
                ),
                new Vector2(
                    centroX,
                    centroZ
                )
            );

        bool estaSobreLaTorre =
            distanciaHorizontal <= 1.5f;

        return estaArriba && estaSobreLaTorre;
    }

    // ========================================
    // MARCAR BLOQUE EN VERDE
    // ========================================

    private void MarcarBloque(GameObject nuevoBloque)
    {
        if (bloqueMirado == nuevoBloque)
            return;

        LimpiarBloqueMirado();

        bloqueMirado = nuevoBloque;

        Renderer renderer =
            bloqueMirado.GetComponent<Renderer>();

        if (renderer != null)
        {
            colorOriginal =
                renderer.material.color;

            renderer.material.color =
                colorIluminado;
        }
    }

    // ========================================
    // QUITAR COLOR VERDE
    // ========================================

    private void LimpiarBloqueMirado()
    {
        if (bloqueMirado != null)
        {
            Renderer renderer =
                bloqueMirado.GetComponent<Renderer>();

            if (renderer != null)
            {
                renderer.material.color =
                    colorOriginal;
            }

            bloqueMirado = null;
        }
    }

    // ========================================
    // DETENER SONIDO
    // ========================================

    private void DetenerSonido()
    {
        if (audioSeleccion != null)
        {
            audioSeleccion.Stop();
        }
    }
}