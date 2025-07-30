using UnityEngine;

public class InputTester : MonoBehaviour
{
    void Start()
    {
        Debug.Log("InputTester script gestart!");
    }

    void Update()
    {
        Debug.Log("InputTester draait...");

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESCAPE WERKT!");
        }
    }
}
