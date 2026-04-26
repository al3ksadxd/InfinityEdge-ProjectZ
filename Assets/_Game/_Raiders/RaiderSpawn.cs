using UnityEngine;
using System.Collections.Generic;

public class RaiderSpawn : MonoBehaviour
{
    public GameObject kockaPrefab;
    public Transform[] spawnPoints;

    [Header("Tajming")]
    public float pocetnoOdlaganje = 100f; // Koliko sekundi čekamo nakon klika na Play
    public float vremeIzmedjuTalasa = 100f; // Razmak između svakog sledećeg talasa

    void Start()
    {
        // Prvi broj (pocetnoOdlaganje) je ČEKANJE pre prvog starta
        // Drugi broj (vremeIzmedjuTalasa) je RAZMAK između ponavljanja
        InvokeRepeating("SpawnTalas", pocetnoOdlaganje, vremeIzmedjuTalasa);
    }

    void SpawnTalas()
    {
        if (spawnPoints.Length == 0) return;

        int brojLokacija = Random.Range(1, spawnPoints.Length + 1);
        List<Transform> slobodnaMesta = new List<Transform>(spawnPoints);

        for (int i = 0; i < brojLokacija; i++)
        {
            if (slobodnaMesta.Count == 0) break;

            int randomIndex = Random.Range(0, slobodnaMesta.Count);
            Transform izabranoMesto = slobodnaMesta[randomIndex];

            Instantiate(kockaPrefab, izabranoMesto.position, Quaternion.identity);

            slobodnaMesta.RemoveAt(randomIndex);
        }

        Debug.Log("Talas pokrenut nakon " + Time.time + "s. Stvoreno: " + brojLokacija);
    }
}