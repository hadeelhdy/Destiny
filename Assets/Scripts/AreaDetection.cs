using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AreaDetection : MonoBehaviour
{
    private string areaName;

    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI questionText;
    public GameObject optionsGroup;

    public Button optionButton1;
    public Button optionButton2;

    private ScaleManager scaleManager;

    public float ballSize = 1.0f;  
    public Texture ballTexture;    

    public GameObject portalPosition;
    public GameObject defaultPortalDestination;

    private void Start()
    {
        areaName = gameObject.name;

        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        if (questionText != null)
            questionText.gameObject.SetActive(false);

        if (optionsGroup != null)
            optionsGroup.SetActive(false);

        if (optionButton1 != null)
            optionButton1.gameObject.SetActive(false);

        if (optionButton2 != null)
            optionButton2.gameObject.SetActive(false);

        scaleManager = Object.FindFirstObjectByType<ScaleManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the area: " + areaName);

            // Set portal destination based on area name
            GameObject portalDestination = GetPortalDestination(areaName);
            scaleManager.SetPortalDestination(portalDestination);

            // Wait for 2 seconds before showing instruction text
            Invoke("ShowInstructionText", 2f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the area: " + areaName);

            // Close the portal when the player leaves the area
            if (scaleManager != null)
            {
                scaleManager.TogglePortal(false); // Ensure the portal is closed
            }

            HideAllTexts();
        }
    }

    private void ShowInstructionText()
    {
        if (instructionText != null)
            instructionText.gameObject.SetActive(true);

        // Wait for 2 seconds before showing the question text
        Invoke("ShowQuestionText", 2f);
    }

    private void ShowQuestionText()
    {
        if (instructionText != null)
            instructionText.gameObject.SetActive(false); // Hide instruction text before showing question

        if (questionText != null)
            questionText.gameObject.SetActive(true);

        // Wait for 2 seconds before showing the options
        Invoke("ShowOptions", 2f);
    }

    private void ShowOptions()
    {
        if (questionText != null)
            questionText.gameObject.SetActive(false); // Hide question text before showing options

        if (optionsGroup != null)
            optionsGroup.SetActive(true);

        if (optionButton1 != null)
            optionButton1.gameObject.SetActive(true);

        if (optionButton2 != null)
            optionButton2.gameObject.SetActive(true);

        // Set up button listeners
        if (optionButton1 != null)
            optionButton1.onClick.AddListener(() => HandleOptionSelected(true));

        if (optionButton2 != null)
            optionButton2.onClick.AddListener(() => HandleOptionSelected(false));
    }

    private void HandleOptionSelected(bool isRightChoice)
    {
        if (scaleManager != null)
        {
            // Pass the choice, ball size, and ball texture to ScaleManager
            scaleManager.HandleScaleBallDrop(isRightChoice, ballSize, ballTexture, portalPosition);
        }

        // Hide options after selection
        if (optionsGroup != null)
            optionsGroup.SetActive(false);
    }

    private void HideAllTexts()
    {
        if (instructionText != null)
            instructionText.gameObject.SetActive(false);

        if (questionText != null)
            questionText.gameObject.SetActive(false);

        if (optionsGroup != null)
            optionsGroup.SetActive(false);

        if (optionButton1 != null)
            optionButton1.gameObject.SetActive(false);

        if (optionButton2 != null)
            optionButton2.gameObject.SetActive(false);
    }

    private GameObject GetPortalDestination(string areaName)
    {
        // Implement logic to determine the portal destination based on areaName
        // For now, return defaultPortalDestination
        return defaultPortalDestination;
    }
}