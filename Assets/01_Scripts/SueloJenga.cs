using UnityEngine;

public class SueloJenga : MonoBehaviour
{
    [Header("Detección de caída")]
    public float caidaMinimaParaPerder = 0.5f;

    private void OnCollisionEnter(Collision choque)
    {
        BloqueJenga bloque =
            choque.gameObject.GetComponent<BloqueJenga>();

        // No es una pieza del Jenga
        if (bloque == null)
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.JuegoTerminado)
            return;

        // Si todavía lo tiene agarrado, no contamos derrota
        if (bloque.estaSiendoMovido)
            return;

        // Comprobamos si realmente cayó desde su posición inicial
        if (bloque.CuantoHaCaido() >= caidaMinimaParaPerder)
        {
            Debug.Log(
                "TORRE DERRIBADA - Jugador " +
                GameManager.Instance.JugadorActual +
                " perdió."
            );

            GameManager.Instance.TorreDerribada();
        }
    }
}