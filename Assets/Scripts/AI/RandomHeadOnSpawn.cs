using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RandomHeadOnSpawn : MonoBehaviour
{
    public SpriteResolver headResolver;

    void Start()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        // wait 1 frame so SpriteLibrary is ready
        yield return null;

        SetRandomHead();
    }

    void SetRandomHead()
    {
        if (headResolver == null)
            headResolver = GetComponentInChildren<SpriteResolver>();

        string[] heads =
        {
            "Face11",
            "Face12",
            "Face21",
            "Face22",
            "Face31",
            "Face32"
        };

        string pick = heads[Random.Range(0, heads.Length)];

        headResolver.SetCategoryAndLabel("Head", pick);
        headResolver.ResolveSpriteToSpriteRenderer();
    }
}