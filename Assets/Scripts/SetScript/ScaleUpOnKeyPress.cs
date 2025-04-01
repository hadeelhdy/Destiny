using UnityEngine;
using System.Collections;  // Add this namespace for IEnumerator support

public class ScaleUpOnKeyPress : MonoBehaviour
{
    // Reference to the character (with bones)
    public GameObject characterReference;
    
    // Starting scale
    private Vector3 startScale = new Vector3(1.7f, 1.7f, 1.7f);
    // Target scale
    private Vector3 targetScale = new Vector3(1.75f, 1.75f, 1.75f);
    
    // Duration of scaling transition
    public float scaleDuration = 1f;

    // Variable to track the scaling progress
    private float scaleProgress = 0f;

    // Update is called once per frame
    void Update()
    {
        // Check for Q key press
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(ScaleUp());
        }
    }

    // Coroutine to smoothly scale up
    private IEnumerator ScaleUp()
    {
        // Make sure the characterReference is set
        if (characterReference == null)
        {
            Debug.LogError("Character Reference not assigned!");
            yield break; // Exit if no reference is assigned
        }

        // Set the starting scale
        characterReference.transform.localScale = startScale;

        while (scaleProgress < 1f)
        {
            // Interpolate between the start scale and the target scale based on progress
            scaleProgress += Time.deltaTime / scaleDuration;
            characterReference.transform.localScale = Vector3.Lerp(startScale, targetScale, scaleProgress);
            
            // Wait until the next frame
            yield return null;
        }

        // Ensure the final scale is exactly the target scale
        characterReference.transform.localScale = targetScale;
    }
}
