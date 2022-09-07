using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject StartMenu;
    public TMP_InputField UsernameField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        StartMenu.SetActive(false);
        UsernameField.interactable = false;
        Client.Instance.ConnectToServer();
    }
}