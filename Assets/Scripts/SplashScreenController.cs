using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenController : MonoBehaviour
{
    // Escena  a la que iremos trás mostrar el splash.
    public string SceneAfterSplash = "MainMenu";
    // Tiempo de duración de la pantalla de splash.
    [Range(1,5)]
    public float splashDuration = 3;
    // Referencia a la imagen de fade
    public Image fadeImage;
    // Gradiente para configurar las duraciones del fade
    //public Gradient fadeColorGradient;
    public AnimationCurve fadeCurve;

    private float timeCounter = 0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Color color = fadeColorGradient.Evaluate(0);
        //fadeImage.color = color;

        float alpha = fadeCurve.Evaluate(0);
        Color imageColor = fadeImage.color;
        imageColor.a = alpha;
        fadeImage.color = imageColor;
    }

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;
        // Opción A
        //Color color = fadeColorGradient.Evaluate(timeCounter / splashDuration);
        //fadeImage.color = color;
        // Opción B
        float alpha = fadeCurve.Evaluate(timeCounter / splashDuration);
        Color imageColor = fadeImage.color;
        imageColor.a = alpha;
        fadeImage.color = imageColor;

        if(timeCounter >= splashDuration)
        {
            SceneManager.LoadScene(SceneAfterSplash);
        }
    }
}

