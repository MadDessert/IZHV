using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Helper behavior which manages the debug UI. </summary>
public class DebugMenuUI : MonoBehaviour
{
#region Editor

    [ Header("Global") ]
    [Tooltip("Display the debug UI?")]
    public bool displayUI;
    
#endregion // Editor

#region Internal

    /// <summary> Dimensions of the main window. </summary>
    private static Vector2 WINDOW_DIMENSION = new Vector2(256.0f, 192.0f);
    /// <summary> Base padding used within the UI. </summary>
    private static float BASE_PADDING = 8.0f;

    /// <summary> Rectangle representing the screen drawing area. </summary>
    private Rect mScreenRect;
    /// <summary> Rectangle representing the main window. </summary>
    private Rect mMainWindowRect;

    /// <summary> Dummy value used for demonstration. </summary>
    private float mDummyValue = 0.0f;
    
#endregion // Internal

#region Interface

#endregion // Interface

    /// <summary> Called when the script instance is first loaded. </summary>
    private void Awake()
    { }

    /// <summary> Called before the first frame update. </summary>
    void Start()
    {
        // Deduce the drawing screen area from the main camera.
        var mainCamera = GameSettings.Instance.mainCamera;
        mScreenRect = new Rect(
            mainCamera.rect.x * Screen.width, 
            mainCamera.rect.y * Screen.height, 
            mainCamera.rect.width * Screen.width, 
            mainCamera.rect.height * Screen.height
        );
        // Initially place the debug window into the top right corner.
        mMainWindowRect = new Rect(
            mScreenRect.x + mScreenRect.width - WINDOW_DIMENSION.x, mScreenRect.y, 
            WINDOW_DIMENSION.x, WINDOW_DIMENSION.y
        );
    }

    /// <summary> Update called once per frame. </summary>
    void Update()
    { }

    /// <summary> Called when GUI drawing should be happening. </summary>
    private void OnGUI()
    {
        if (displayUI)
        { // Only draw the window if we are currently displaying it.
            mMainWindowRect = GUI.Window(0, mMainWindowRect, MainWindowUI, "Cheat Console");
            // Limit the window position to the screen area.
            mMainWindowRect.x = Mathf.Clamp(
                mMainWindowRect.x, mScreenRect.x, 
                mScreenRect.x + mScreenRect.width - WINDOW_DIMENSION.x
            );
            mMainWindowRect.y = Mathf.Clamp(
                mMainWindowRect.y, mScreenRect.y, 
                mScreenRect.y + mScreenRect.height - WINDOW_DIMENSION.y
            );
        }
    }

    /// <summary> Function used for drawing the main window. </summary>
    private void MainWindowUI(int windowId)
    {
        // Start the window drawing area, starting with some padding
        GUILayout.BeginArea(new Rect(
            BASE_PADDING, 2.0f * BASE_PADDING, 
            WINDOW_DIMENSION.x - 2.0f * BASE_PADDING, 
            WINDOW_DIMENSION.y - 3.0f * BASE_PADDING
        ));
        { // Main Window Area
            
            
            // GUILayout allows us to automatically place UI elements after each other.
            // BeginVertical starts a vertical group, while BeginHorizontal a horizontal one.
            GUILayout.BeginVertical();
            {
                
                /*
                 * Task 3b: The Cheat
                 */
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Currency: ", GUILayout.Width(WINDOW_DIMENSION.x / 4.0f));
                    
                    var currency = (float)InventoryManager.Instance.availableCurrency;
                    currency = GUILayout.HorizontalSlider(currency, 0.0f, 1000.0f, 
                        GUILayout.ExpandWidth(true));
                    
                    if (GUI.changed)
                    { InventoryManager.Instance.availableCurrency = (int)currency; }
                }
                GUILayout.EndHorizontal();
                
                /*
                 * Task 3c: The Tool
                 */
                
                // Interactive Mode Toggle
                GameManager.Instance.interactiveMode = GUILayout.Toggle(
                    GameManager.Instance.interactiveMode, 
                    "Interactive Mode"
                );

                // Master Volume Slider
                GUILayout.Label($"Master Volume: {SoundManager.Instance.masterVolume:F1} dB");
                SoundManager.Instance.masterVolume = GUILayout.HorizontalSlider(
                    SoundManager.Instance.masterVolume, 
                    -80.0f, 
                    20.0f
                );

                // Master Mute Toggle
                SoundManager.Instance.masterMuted = GUILayout.Toggle(
                    SoundManager.Instance.masterMuted, 
                    "Mute Sound"
                );
                
                
                // Placing the elements next to each other.
                GUILayout.BeginHorizontal();
                {
                    for (var iii = 1; iii <= 10; ++iii)
                    { // Create a set of 10 sliders all sharing the same value.
                        mDummyValue = GUILayout.VerticalSlider(
                            mDummyValue, 0.0f, 10.0f * iii, 
                            GUILayout.ExpandHeight(true)
                        );
                    }

                    /*
                     * Task 3a: The Dummy
                     */
                    if (GUILayout.Button("Enable\nDummy\nCharacter", 
                        GUILayout.ExpandWidth(true), 
                        GUILayout.ExpandHeight(true)))
                    { 
                        GameManager.Instance.TogglePlayerCharacter();
                    }
                }
                GUILayout.EndHorizontal();
                // Do not forget to end each group in the correct order!
            }
            GUILayout.EndVertical();
            // End the group!
            
            
        } // End of Main Window Area
        GUILayout.EndArea();
        
        // Allow dragging of the window by grabbing any part of it.
        GUI.DragWindow(new Rect(
            2.0f * BASE_PADDING,0.0f,
            WINDOW_DIMENSION.x - 4.0f * BASE_PADDING, 
            WINDOW_DIMENSION.y
        ));
    }
}
