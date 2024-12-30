using UnityEngine;

public class RepeatBackground : MonoBehaviour
{
    [SerializeField] private Transform player; 
    private float backgroundWidth; 
    private Vector3 lastPlayerPosition; 

    private void Start()
    {
        backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        Debug.Log($"Background Width: {backgroundWidth}");
        lastPlayerPosition = player.position;
    }

    private void Update()
    {
        if (player.position.x >= lastPlayerPosition.x + backgroundWidth)
        {
            transform.position += Vector3.right * backgroundWidth;
            lastPlayerPosition += Vector3.right * backgroundWidth;
        }
    }
}