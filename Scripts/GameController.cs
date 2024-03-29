using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject pauseGameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseGameObject.activeSelf)
            {
                pauseGameObject.SetActive(false);
            }
            else
            {
                pauseGameObject.SetActive(true);
            }
        }

        if (pauseGameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.L))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}