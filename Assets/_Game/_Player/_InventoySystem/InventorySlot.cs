using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public ItemData tipPredmeta; // Tvoj ScriptableObject (npr. Kamen)
    public int trenutnaKolicina = 1; // Stavi na 1 za test

    private GameObject trenutniModel; // Ovde čuvamo stvoreni model

    void Start()
    {
        // Pozivamo ovo odmah da bi videli predmet čim pokreneš igru
        OsveziPrikaz();
    }

    public void OsveziPrikaz()
    {
        // 1. Ako već postoji neki model, obriši ga da ne pravimo duplikate
        if (trenutniModel != null)
        {
            Destroy(trenutniModel);
        }

        // 2. Proveri da li imamo predmet i da li je količina veća od 0
        if (tipPredmeta != null && trenutnaKolicina > 0)
        {
            // 3. Stvori (Instantiate) model iz ScriptableObject-a
            if (tipPredmeta.modelZaProstirku != null)
            {
                // Stvaramo model kao "dete" ovog slota (da bi se pomerali zajedno)
                trenutniModel = Instantiate(tipPredmeta.modelZaProstirku, transform.position, transform.rotation, transform);

                // VRLO BITNO: Postavi stvoreni model na Inventory layer
                PostaviLayerRekurzivno(trenutniModel, LayerMask.NameToLayer("Inventory"));
            }
        }
    }

    // Pomoćna funkcija koja osigurava da i model i svi njegovi delovi budu na Inventory layeru
    void PostaviLayerRekurzivno(GameObject obj, int noviLayer)
    {
        obj.layer = noviLayer;
        foreach (Transform child in obj.transform)
        {
            PostaviLayerRekurzivno(child.gameObject, noviLayer);
        }
    }
}