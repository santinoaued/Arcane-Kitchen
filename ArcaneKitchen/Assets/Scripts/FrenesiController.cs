using System;
using UnityEngine;

public class FrenesiController : MonoBehaviour
{
    public static event Action<bool> OnFrenesiChanged; 

    private bool frenesiActivo = false;

    [SerializeField] int danioperiodico = 1;
    private float temporizadorDanio = 0f;
    private float temporizadorCuracion = 0f;

    private playerSanityHealth cordura;

    void Start()
    {
        cordura = GetComponent<playerSanityHealth>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!frenesiActivo)
            {
                ActivarFrenesi();
            } else
            {
                DesactivarFrenesi();
            }
        }

        if (frenesiActivo)
        {
            temporizadorDanio += Time.deltaTime;
            if (temporizadorDanio >= 1.5f)
            {
                cordura.RecibirDanioCordura(danioperiodico);
                temporizadorDanio = 0f;
            }
        } else
        {
            temporizadorCuracion += Time.deltaTime;
            if (temporizadorCuracion >= 5f)
            {
                cordura.Tranquilizarse();
                temporizadorCuracion = 0f;
            }
        }
    }

    void ActivarFrenesi()
    {
        frenesiActivo = true;

        OnFrenesiChanged?.Invoke(true);

        temporizadorDanio = 0f;

        Debug.Log("Esta ON");
    }

    void DesactivarFrenesi()
    {
        frenesiActivo = false;

        OnFrenesiChanged?.Invoke(false);

        Debug.Log("Esta OFF");
    }
}
