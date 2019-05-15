using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{

    private bool loadScene;
    private float fadeCounter = 0;

    [SerializeField] private Image loadImg;
    [SerializeField] private Animator fadeAnim;
    [SerializeField] [Range (0, 0.1f)] private float logoFadeSpeed;
    [SerializeField] [Range (0, 3)] private float loadTime;

    private void Update()
    {
        //Loading screen
        if (loadScene)
        {
            fadeCounter += logoFadeSpeed;
            loadImg.color = new Color(loadImg.color.r, loadImg.color.g, loadImg.color.b, Mathf.PingPong(fadeCounter, 1));
        }
    }

    //src https://blog.teamtreehouse.com/make-loading-screen-unity
    public IEnumerator LoadScene(string scene)
    {
        loadScene = true;
        loadImg.enabled = true;
        fadeAnim.SetBool("end", true);

        fadeAnim.transform.SetAsLastSibling();
        loadImg.transform.SetAsLastSibling();

        //Uncomment this line to see loading screen better; our game loads TOO FAST
        yield return new WaitForSeconds(loadTime);

        // Start an asynchronous operation to load the scene that was passed to the LoadScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
