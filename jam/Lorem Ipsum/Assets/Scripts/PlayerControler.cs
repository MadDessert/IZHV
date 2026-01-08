using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerControler : MonoBehaviour
{
    private float[] lanes = { -20.5f, -12.5f, -3.5f, 4.5f };
    private int currentLane = 2;

    [Header("Movement Settings")]
    public float laneChangeSpeed = 10f;
    public float forwardSpeed = 20f; 
    public float speedIncreaseRate = 0.1f; 
    public float maxSpeed = 60f;           

    [Header("UI References")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI highScoreDisplay;
    public GameObject pauseMenuPanel; 

    [Header("Keybindings")]
    public Key leftKey = Key.A;
    public Key rightKey = Key.D;
    
    private float startZ;
    private bool isGameRunning = false;
    private bool isPaused = false;

    void Start()
    {
        leftKey = (Key)PlayerPrefs.GetInt("LeftKey", (int)Key.A);
        rightKey = (Key)PlayerPrefs.GetInt("RightKey", (int)Key.D);

        float savedHigh = PlayerPrefs.GetFloat("HighScore", 0);
        if (highScoreDisplay != null) highScoreDisplay.text = "Best: " + savedHigh.ToString("F0");
        
        if(pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
    }

    public void BeginRun()
    {
        startZ = transform.position.z;
        isGameRunning = true;
        Time.timeScale = 1; // Ensure game is unpaused when starting
    }

    void Update()
    {
        // ESC to Pause 
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        if (!isGameRunning || isPaused) return;

        // Gradually Increase Speed
        if (forwardSpeed < maxSpeed)
        {
            forwardSpeed += speedIncreaseRate * Time.deltaTime;
        }

        // Movement Logic
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if (Keyboard.current == null) return;
        if (Keyboard.current[leftKey].wasPressedThisFrame && currentLane > 0) currentLane--;
        if (Keyboard.current[rightKey].wasPressedThisFrame && currentLane < lanes.Length - 1) currentLane++;

        float targetX = lanes[currentLane];
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * laneChangeSpeed);
    
        // Score Logic
        float distance = transform.position.z - startZ;
        if (scoreText != null) scoreText.text = "Score: " + distance.ToString("F0");

        if (distance > PlayerPrefs.GetFloat("HighScore", 0))
        {
            if (highScoreDisplay != null) highScoreDisplay.text = "Best: " + distance.ToString("F0");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0; // Freezes everything
            if(pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1; // Unfreezes everything
            if(pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("QUIT PRESSED");
        Application.Quit(); // Only works in the actual .exe build
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            float distance = transform.position.z - startZ;
            if (distance > PlayerPrefs.GetFloat("HighScore", 0))
            {
                PlayerPrefs.SetFloat("HighScore", distance);
                PlayerPrefs.Save();
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}