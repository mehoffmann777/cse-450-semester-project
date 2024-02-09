using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{

    private AssetBundle bundle;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadMap()
    {
        SceneManager.LoadScene("TestBattle");
    }

    public void loadCombat()
    {
        SceneManager.LoadScene("TestingCharacter");
    }
}

