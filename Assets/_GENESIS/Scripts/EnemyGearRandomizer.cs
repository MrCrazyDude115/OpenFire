using UnityEngine;

public class EnemyGearRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] headgear;
    [SerializeField] private Material[] gearMaterials;
    [SerializeField] private Transform headgearParent;

    private void Start()
    {
        SpawnGear();
    }

    private int RandomizeGear(GameObject[] array)
    {
        int gearArraySize = array.Length;
        
        int chosen = Random.Range(0, gearArraySize);
        return chosen;
    }

    private int RandomizeMaterial(Material[] array)
    {
        int materialArraySize = array.Length;

        int chosen = Random.Range(0, materialArraySize);
        return chosen;
    }

    private void SpawnGear()
    {
        int chosenHead = RandomizeGear(headgear);
        int chosenMaterial = RandomizeMaterial(gearMaterials);

        GameObject head = Instantiate(headgear[chosenHead], headgearParent);
        MeshRenderer headRenderer = head.GetComponent<MeshRenderer>();
        headRenderer.material = gearMaterials[chosenMaterial];
    }
}
