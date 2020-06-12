using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject m_Instructions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeScene(int _SceneID)
    {
        SceneManager.LoadScene(_SceneID);
    }

    public void ShowInstructions()
    {
        if (m_Instructions)
        {
            m_Instructions.SetActive(true);
        }
    }

    public void HideInstructions()
    {
        if (m_Instructions)
        {
            m_Instructions.SetActive(false);
        }
    }
}
