using UnityEngine;

// Ova linija ti omogućava da desnim klikom u Projektu napraviš novi predmet
[CreateAssetMenu(fileName = "Novi Predmet", menuName = "Inventory/Predmet")]
public class ItemData : ScriptableObject
{
    public string nazivPredmeta;

    [TextArea]
    public string opis;

    public GameObject modelZaProstirku; // Model koji će se pojaviti na tvojoj prostirci
    public int maxStack = 10;           // Koliko najviše ovih predmeta možeš da nosiš
}