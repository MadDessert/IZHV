using UnityEngine;
using TMPro;

public class CarSelector : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public Transform modelContainer;
    public TextMeshProUGUI highscoreText;
    
    private int currentCarIndex = 0;
    private GameObject currentModel;
    private PlayerControler moveScript;
    private Rigidbody rb;

    void Start()
    {
        moveScript = GetComponent<PlayerControler>(); 
        rb = GetComponent<Rigidbody>();
        
        if(moveScript != null) moveScript.enabled = false;
        if(rb != null) rb.isKinematic = true;

        UpdateCarModel();
    }

    public void NextCar() { currentCarIndex = (currentCarIndex + 1) % carPrefabs.Length; UpdateCarModel(); }
    public void PrevCar() { currentCarIndex--; if (currentCarIndex < 0) currentCarIndex = carPrefabs.Length - 1; UpdateCarModel(); }

    void UpdateCarModel()
    {
        if (currentModel != null) Destroy(currentModel);
        currentModel = Instantiate(carPrefabs[currentCarIndex], modelContainer.position, modelContainer.rotation);
        currentModel.transform.SetParent(modelContainer);
    }

    public void StartGame(GameObject menuUI)
    {
        if(menuUI != null) menuUI.SetActive(false);
        if(moveScript != null) 
        {
            moveScript.enabled = true;
            moveScript.BeginRun();
        }
        if(rb != null) rb.isKinematic = false;
    }
}