using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // =========================
    // TURNOS
    // =========================

    [Header("Turnos")]
    [SerializeField] private int jugadorActual = 1;

    // Guarda quién fue el último jugador que comenzó
    // a manipular una pieza.
    private int jugadorResponsableMovimiento = 1;

    // Sirve para saber si ya hubo alguna manipulación.
    private bool hayMovimientoRegistrado = false;

    // =========================
    // REALIDAD AUMENTADA
    // =========================

    [Header("Realidad Aumentada")]
    [SerializeField] private bool seguimientoAREstable = false;

    // =========================
    // ESTADO DEL JUEGO
    // =========================

    public bool JuegoTerminado { get; private set; } = false;

    // =========================
    // PROPIEDADES
    // =========================

    public int JugadorActual
    {
        get { return jugadorActual; }
    }

    public int JugadorResponsableMovimiento
    {
        get
        {
            if (hayMovimientoRegistrado)
                return jugadorResponsableMovimiento;

            return jugadorActual;
        }
    }

    public bool SeguimientoAREstable
    {
        get { return seguimientoAREstable; }
    }

    // =========================
    // INICIAR
    // =========================

    void Awake()
    {
        // Evita tener dos GameManager activos por accidente.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        jugadorActual = 1;
        jugadorResponsableMovimiento = 1;
        hayMovimientoRegistrado = false;
        JuegoTerminado = false;

        Debug.Log("Partida iniciada. Turno del Jugador 1");
    }

    // =========================
    // REGISTRAR QUIÉN MUEVE
    // =========================

    public void RegistrarInicioMovimiento()
    {
        if (JuegoTerminado)
            return;

        jugadorResponsableMovimiento = jugadorActual;
        hayMovimientoRegistrado = true;

        Debug.Log(
            "Jugador " +
            jugadorResponsableMovimiento +
            " está realizando su movimiento."
        );
    }

    // =========================
    // CAMBIAR TURNO
    // =========================

    public void CambiarTurno()
    {
        if (JuegoTerminado)
            return;

        jugadorActual++;

        if (jugadorActual > 3)
        {
            jugadorActual = 1;
        }

        /*
         * IMPORTANTE:
         * No cambiamos jugadorResponsableMovimiento aquí.
         *
         * Si el Jugador 1 coloca una pieza correctamente,
         * pasa el turno al Jugador 2.
         *
         * Pero si inmediatamente después la torre cae,
         * todavía debe perder el Jugador 1.
         *
         * El responsable recién cambia cuando el siguiente
         * jugador empieza realmente a mover una pieza.
         */

        Debug.Log("Turno del Jugador " + jugadorActual);
    }

    // =========================
    // TORRE DERRIBADA
    // =========================

    public void TorreDerribada()
    {
        if (JuegoTerminado)
            return;

        JuegoTerminado = true;

        int jugadorQuePierde;

        if (hayMovimientoRegistrado)
        {
            jugadorQuePierde =
                jugadorResponsableMovimiento;
        }
        else
        {
            jugadorQuePierde =
                jugadorActual;
        }

        Debug.Log(
            "¡Jugador " +
            jugadorQuePierde +
            " PERDIÓ!"
        );
    }

    // =========================
    // ESTADO DEL AR
    // =========================

    public void SetSeguimientoAR(bool estable)
    {
        seguimientoAREstable = estable;

        if (seguimientoAREstable)
        {
            Debug.Log(
                "Seguimiento AR estable. " +
                "Manipulación habilitada."
            );
        }
        else
        {
            Debug.Log(
                "Seguimiento AR perdido. " +
                "Manipulación bloqueada."
            );
        }
    }
}