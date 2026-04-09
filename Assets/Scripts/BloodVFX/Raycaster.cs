using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public ParticleSystem splatParticles;
    public GameObject splatPrefab;
    public Transform splatHolder;
    
    
    public void SpawnBlood(Vector2 position, bool isBackground)
    {
        GameObject splat = Instantiate(splatPrefab, position, Quaternion.identity);
        splat.transform.SetParent(splatHolder, true);

        Splat splatScript = splat.GetComponent<Splat>();

        if (isBackground)
            splatScript.Initialize(Splat.SplatLocation.Background);
        else
            splatScript.Initialize(Splat.SplatLocation.Foreground);

        // 🔥 NEW: separate particle system per hit
        ParticleSystem ps = Instantiate(splatParticles, position, Quaternion.identity);
        ps.Play();

        Destroy(ps.gameObject, 2f); // cleanup
    }
}
