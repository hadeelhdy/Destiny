using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ScaleManager : MonoBehaviour
{
    // Portal System Variables
    public GameObject portalEffect;
    private bool isPortalActive = false;

    private GameObject portalDestination;

    // Scale Manager Variables
    public RawImage[] scaleRawImages;
    public GameObject scaleBallPrefab, rightBallPrefab, leftBallPrefab;
    public Transform scaleRightDropPoint, scaleLeftDropPoint;
    public Transform rightCylinderDropPoint, leftCylinderDropPoint;

    private bool isRightChoice = false;
    private List<GameObject> scaleBalls = new List<GameObject>();
    private List<GameObject> cylinderBalls = new List<GameObject>();

    public TextMeshProUGUI expandableText;
    private Vector3 originalTextScale;
    private Vector3 centerOfScreenPosition;

    public Transform rightTarget;
    public Transform leftTarget;

    private GameObject portalPosition;
    void Start()
    {
        // Portal System Initialization
        if (portalEffect != null)
        {
            portalEffect.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Portal effect is not assigned!");
        }

        // Scale Manager Initialization
        SetScaleVisibility(false);
        if (expandableText != null)
        {
            originalTextScale = expandableText.rectTransform.localScale;
            expandableText.rectTransform.localScale = Vector3.zero;
            centerOfScreenPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        }
    }

    public void TogglePortal(bool? state = null)
    {
        if (portalEffect != null)
        {
            if (state.HasValue)
            {
                isPortalActive = state.Value;
            }
            else
            {
                isPortalActive = !isPortalActive;
            }

            // Move portal to the correct position
            if (isPortalActive && portalPosition != null)
            {
                portalEffect.transform.position = portalPosition.transform.position;
            }

            portalEffect.SetActive(isPortalActive);
        }
    }

    public Transform GetPortalDestination()
    {
        return portalDestination != null ? portalDestination.transform : null;
    }
    public void SetPortalDestination(GameObject destination)
    {
        portalDestination = destination;
    }

    public void HandleScaleBallDrop(bool isRight, float ballSize, Texture ballTexture, GameObject portalPos)
    {
        isRightChoice = isRight;
        ClearScale();
        SetScaleVisibility(true);
        this.portalPosition = portalPos;
        SpawnBall(scaleBallPrefab, isRight ? scaleRightDropPoint : scaleLeftDropPoint, ballSize, ballTexture, true);
        Invoke("StartScaleShrink", 2f);
    }

    void SpawnBall(GameObject ballPrefab, Transform dropPoint, float ballSize, Texture ballTexture, bool isScaleBall)
    {
        GameObject newBall = Instantiate(ballPrefab, dropPoint.position, Quaternion.identity);
        newBall.transform.localScale = Vector3.one * ballSize;

        Renderer ballRenderer = newBall.GetComponent<Renderer>();
        if (ballRenderer != null && ballTexture != null)
        {
            ballRenderer.material.mainTexture = ballTexture;
        }

        Rigidbody ballRb = newBall.GetComponent<Rigidbody>();
        ballRb.useGravity = true;

        if (isScaleBall)
        {
            scaleBalls.Add(newBall);
        }
        else
        {
            cylinderBalls.Add(newBall);
        }
    }

    void StartScaleShrink()
    {
        if (scaleRawImages == null || scaleRawImages.Length == 0)
        {
            Debug.LogError("Scale RawImages array is empty or not assigned!");
            return;
        }
        StartCoroutine(ShrinkScale());
    }

    IEnumerator ShrinkScale()
    {
        float shrinkDuration = 1f;
        float elapsedTime = 0f;
        Vector3 originalScale = Vector3.one;

        foreach (RawImage img in scaleRawImages)
        {
            if (img != null)
            {
                originalScale = img.rectTransform.localScale;
                break;
            }
        }

        while (elapsedTime < shrinkDuration)
        {
            float scaleFactor = 1 - (elapsedTime / shrinkDuration);
            foreach (RawImage img in scaleRawImages)
            {
                if (img != null)
                {
                    img.rectTransform.localScale = originalScale * scaleFactor;
                }
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetScaleVisibility(false);
        DropBallOnCylinder();

        if (expandableText != null)
        {
            StartCoroutine(ExpandText());
        }
    }

    IEnumerator ExpandText()
    {
        float expandDuration = 1f;
        float elapsedTime = 0f;

        expandableText.rectTransform.position = centerOfScreenPosition;
        expandableText.rectTransform.localScale = Vector3.zero;

        while (elapsedTime < expandDuration)
        {
            float scaleFactor = elapsedTime / expandDuration;
            expandableText.rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalTextScale, scaleFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        expandableText.rectTransform.localScale = originalTextScale;
        yield return new WaitForSeconds(2f);

        StartCoroutine(MoveTextToTargetPosition());
    }

    IEnumerator MoveTextToTargetPosition()
    {
        float moveDuration = 2f;
        float elapsedTime = 0f;
        Vector3 originalPosition = expandableText.rectTransform.position;
        Vector3 originalScale = expandableText.rectTransform.localScale;
        Transform targetPosition = isRightChoice ? rightTarget : leftTarget;

        while (elapsedTime < moveDuration)
        {
            float moveFactor = elapsedTime / moveDuration;
            float shrinkFactor = Mathf.Lerp(1f, 0.6f, moveFactor);

            expandableText.rectTransform.position = Vector3.Lerp(originalPosition, targetPosition.position, moveFactor);
            expandableText.rectTransform.localScale = originalScale * shrinkFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        expandableText.rectTransform.position = targetPosition.position;
        expandableText.rectTransform.localScale = Vector3.zero;
    }

    void SetScaleVisibility(bool visible)
    {
        if (scaleRawImages == null || scaleRawImages.Length == 0) return;

        foreach (RawImage img in scaleRawImages)
        {
            if (img != null)
            {
                img.gameObject.SetActive(visible);
                if (visible) img.rectTransform.localScale = Vector3.one;
            }
        }
    }

    void DropBallOnCylinder()
    {
        StartCoroutine(DelayedBallDrop());
    }

    IEnumerator DelayedBallDrop()
    {
        yield return new WaitForSeconds(5f);

        Transform cylinderDropPoint = isRightChoice ? rightCylinderDropPoint : leftCylinderDropPoint;
        GameObject ballPrefab = isRightChoice ? rightBallPrefab : leftBallPrefab;

        float scaleBallSize = scaleBalls.Count > 0 ? scaleBalls[0].transform.localScale.x : 1f;
        Texture scaleBallTexture = scaleBalls.Count > 0 ? scaleBalls[0].GetComponent<Renderer>().material.mainTexture : null;

        SpawnBall(ballPrefab, cylinderDropPoint, scaleBallSize, scaleBallTexture, false);

        // Automatically activate the portal when the ball drops on the cylinder
        TogglePortal(true);
    }

    void ClearScale()
    {
        foreach (GameObject ball in scaleBalls)
        {
            Destroy(ball);
        }
        scaleBalls.Clear();
    }
}