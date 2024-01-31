using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeChanger : MonoBehaviour
{

    public Animator animator;

    private string levelToLoad;
    
    


    public void FadeToLevel(string levelName)
    {
        levelToLoad = levelName;
        animator.SetTrigger("FadeOut");
        
    }

    private void onFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}