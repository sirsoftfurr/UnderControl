using UnityEngine;

public class Splat : MonoBehaviour
{
  public enum SplatLocation
  {
    Foreground,
    Background,
  }

  public Color backgroundTint;
  public float minSizeMod = 0.8f;
  public float maxSizeMod = 1.2f;

  public Sprite[] sprites;
  
  private SplatLocation splatLocation;
  private SpriteRenderer splatRenderer;

  private void Awake()
  {
    splatRenderer = GetComponent<SpriteRenderer>();
  }

  public void Initialize(SplatLocation splatLocation)
  {
    this.splatLocation = splatLocation;
    SetSprite();
    SetSize();
    SetRotation();

    SetLocationProperties();
  }

  private void SetSprite()
  {
    int randomIndex = Random.Range(0, sprites.Length);
    splatRenderer.sprite = sprites[randomIndex];
  }

  private void SetSize()
  {
    float sizeMod = Random.Range(minSizeMod, maxSizeMod);
    transform.localScale *= sizeMod;
  }

  private void SetRotation()
  {
    float randomRotation = Random.Range(-360f, 360f);
    transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
  }

  private void SetLocationProperties()
  {
    switch (splatLocation)
    {
      case SplatLocation.Background:
        splatRenderer.color = backgroundTint;
        splatRenderer.sortingOrder = 9;
        break;
      
      case SplatLocation.Foreground:
        splatRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        splatRenderer.sortingOrder = 10;
        break;
    }
  }
}
