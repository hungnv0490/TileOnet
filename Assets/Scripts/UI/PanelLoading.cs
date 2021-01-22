using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLoading : MonoBehaviour
{
    public Image image;

    private IEnumerator startIE;
    private float rotatePerMin = 145;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        gameObject.SetActive(true);
        startIE = StartLoadingIE();
        StartCoroutine(startIE);
    }

    private IEnumerator StartLoadingIE()
    {
        while (true)
        {
            yield return null;
            image.transform.Rotate(0, 0, rotatePerMin * Time.deltaTime);
        }
    }

    public void StopLoading()
    {
        gameObject.SetActive(false);

        if (startIE != null)
        {
            StopCoroutine(startIE);
            startIE = null;
        }
    }
}
