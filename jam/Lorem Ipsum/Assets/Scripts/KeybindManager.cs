using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class KeybindManager : MonoBehaviour
{
    public PlayerControler player; 
    
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    public TextMeshProUGUI leftKeyText;
    public TextMeshProUGUI rightKeyText;

    private bool isWaitingForKey = false;

    void Start()
    {
        UpdateUIStrings();
    }

    public void OpenSettings()
    {
        if(mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if(settingsPanel != null) settingsPanel.SetActive(true);
        UpdateUIStrings();
    }

    public void CloseSettings()
    {
        if(settingsPanel != null) settingsPanel.SetActive(false);
        if(mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void StartRebindLeft() { StartCoroutine(ListenForKey(true)); }
    public void StartRebindRight() { StartCoroutine(ListenForKey(false)); }

    IEnumerator ListenForKey(bool rebindingLeft)
    {
        isWaitingForKey = true;
        if (rebindingLeft) leftKeyText.text = "...";
        else rightKeyText.text = "...";

        // Wait one frame so the click used to press the button doesnt trigger a rebind
        yield return null; 

        while (isWaitingForKey)
        {
            // Scan all possible keys
            foreach (Key k in System.Enum.GetValues(typeof(Key)))
            {
                
                if (k == Key.None) continue;

                // Check if the current keyboard supports this key and if it was pressed
                try 
                {
                    if (Keyboard.current[k].wasPressedThisFrame)
                    {
                        if (rebindingLeft)
                        {
                            player.leftKey = k;
                            PlayerPrefs.SetInt("LeftKey", (int)k);
                        }
                        else
                        {
                            player.rightKey = k;
                            PlayerPrefs.SetInt("RightKey", (int)k);
                        }
                        isWaitingForKey = false;
                        break;
                    }
                }
                catch 
                {
                    // Skip keys that don't exist on this specific keyboard
                    continue; 
                }
            }
            yield return null;
        }
        UpdateUIStrings();
    }

    void UpdateUIStrings()
    {
        if(player == null) return;
        leftKeyText.text = "Left: " + player.leftKey.ToString();
        rightKeyText.text = "Right: " + player.rightKey.ToString();
    }
}