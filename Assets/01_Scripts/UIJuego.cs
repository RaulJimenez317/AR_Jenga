using UnityEngine;
using TMPro;
using System.Collections;

public class UIJuego : MonoBehaviour
{
    public static UIJuego Instance { get; private set; }

    [Header("Interfaz")]
    [SerializeField] private TMP_Text textoTurno;

    [Header("Mensajes")]
    [SerializeField] private float duracionMensaje = 2f;

    private int ultimoJugador = -1;
    private bool juegoTerminadoMostrado = false;
    private Coroutine rutinaMensaje;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ActualizarTurno();
    }

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        // =========================
        // JUEGO TERMINADO
        // =========================
        if (GameManager.Instance.JuegoTerminado)
        {
            if (!juegoTerminadoMostrado)
            {
                MostrarPerdedor();
                juegoTerminadoMostrado = true;
            }

            return;
        }

        // =========================
        // ACTUALIZAR TURNO
        // =========================
        if (ultimoJugador != GameManager.Instance.JugadorActual)
        {
            ActualizarTurno();
        }
    }

    // =========================
    // MOSTRAR TURNO
    // =========================
    public void ActualizarTurno()
    {
        if (GameManager.Instance == null)
            return;

        ultimoJugador =
            GameManager.Instance.JugadorActual;

        if (textoTurno != null)
        {
            textoTurno.text =
                "Turno: Jugador " +
                ultimoJugador;
        }
    }

    // =========================
    // MOSTRAR MENSAJE TEMPORAL
    // =========================
    public void MostrarMensaje(string mensaje)
    {
        if (textoTurno == null)
            return;

        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
        }

        rutinaMensaje =
            StartCoroutine(
                MostrarMensajeTemporal(mensaje)
            );
    }

    private IEnumerator MostrarMensajeTemporal(
        string mensaje
    )
    {
        textoTurno.text = mensaje;

        yield return new WaitForSeconds(
            duracionMensaje
        );

        if (GameManager.Instance != null &&
            !GameManager.Instance.JuegoTerminado)
        {
            ActualizarTurno();
        }

        rutinaMensaje = null;
    }

    // =========================
    // MOSTRAR PERDEDOR
    // =========================
    private void MostrarPerdedor()
    {
        if (textoTurno == null ||
            GameManager.Instance == null)
        {
            return;
        }

        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
            rutinaMensaje = null;
        }

        int jugadorPerdedor =
            GameManager.Instance
                .JugadorResponsableMovimiento;

        textoTurno.text =
            "¡Jugador " +
            jugadorPerdedor +
            " PERDIÓ!";
    }
}