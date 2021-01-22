
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public static EffectController instance = null;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform rectItem1;
    [SerializeField] private Transform rectItem2;
    [SerializeField] private GameObject dollarPref;
    [SerializeField] private Transform dollarContainer;
    [SerializeField] private Transform dollarTarget;
    private List<GameObject> dollars;
    public float widtLineInRect;

    private int dollarMove;

    void Awake()
    {
        if (instance != null && instance.gameObject.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        dollars = new List<GameObject>();
    }

    public void SetupLine(List<Vector3> listVector3, List<Vector3> listVector32)
    {
        if (listVector3 != null && listVector3.Count > 0)
        {
            lineRenderer.gameObject.SetActive(true);

            lineRenderer.positionCount = listVector3.Count;

            for (int i = 0; i < listVector3.Count; i++)
            {
                lineRenderer.SetPosition(i, listVector3[i]);
            }
        }

        dollars.Clear();
        for (int i = 0; i < listVector32.Count; i++)
        {
            var itemGO = ObjectPool.Instance.GetGameObject(dollarPref, listVector32[i], Quaternion.identity);

            itemGO.transform.parent = dollarContainer;

            itemGO.transform.localScale = new Vector3(1, 1, 1);

            dollars.Add(itemGO);
        }
    }

    public void HideDollars(float timeWait)
    {
        StartCoroutine(HideDollarsIE(timeWait));
    }

    private IEnumerator HideDollarsIE(float timeWait)
    {
        yield return new WaitForSeconds(timeWait);

        StartCoroutine(PlaySound(dollars.Count));

        dollarMove = 0;
        for (int i = 0; i < dollars.Count; i++)
        {
            var dollar = dollars[i];

            dollarMove++;

            StartCoroutine(DollarMove(dollar.transform, dollarTarget));
        }
    }

    public void CheckMovingDollars(Action action)
    {
        StartCoroutine(IsMovingDollarsIE(action));
    }

    private IEnumerator IsMovingDollarsIE(Action action)
    {
        while (dollarMove > 0)
        {
            yield return null;
        }

        action();
    }

    private IEnumerator DollarMove(Transform tf, Transform target)
    {
        while (tf.position != target.position)
        {
            tf.position = Vector3.MoveTowards(tf.position, target.position, 0.15f);
            yield return null;
        }
        dollarMove--;

        InGameUIManager.instance.CoinChange(1);
        ObjectPool.Instance.ReleaseObject(tf.gameObject);
    }

    IEnumerator PlaySound(int length)
    {
        for (int i = 0; i < length; i++)
        {
            AudioManager.instance.TileMove();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SetupRectAroundItem(int rectItemIndex, Item item, bool fill)
    {
        var rectItem = rectItemIndex == 1 ? rectItem1 : rectItem2;
        item.rectIndex = rectItemIndex;

        rectItem.gameObject.SetActive(true);

        var angle1 = rectItem.GetChild(0);
        var angle2 = rectItem.GetChild(1);
        var angle3 = rectItem.GetChild(2);
        var angle4 = rectItem.GetChild(3);

        if (fill)
        {
            var w = (item.angles[1].position - item.angles[0].position).x + 0.03f;
            var h = (item.angles[0].position - item.angles[3].position).y + 0.03f;

            SetupOneAngle(angle1, item, 1, w, h);
            SetupOneAngle(angle3, item, 3, w, h);
            angle2.GetComponent<LineRenderer>().positionCount = 0;
            angle4.GetComponent<LineRenderer>().positionCount = 0;
        }
        else
        {
            SetupOneAngle(angle1, item, 1);
            SetupOneAngle(angle2, item, 2);
            SetupOneAngle(angle3, item, 3);
            SetupOneAngle(angle4, item, 4);
        }
    }

    private void SetupOneAngle(Transform angle, Item item, int type, float w = 0, float h = 0)
    {
        var angleLine = angle.GetComponent<LineRenderer>();

        angleLine.positionCount = 3;

        var tmpX = w == 0 ? widtLineInRect : w;
        var tmpY = h == 0 ? widtLineInRect : h;

        float x1 = 0, y1 = 0;
        float x2 = 0, y2 = 0;
        if (type == 1) // top left
        {
            x1 = tmpX;
            y2 = -tmpY;
        }
        else if (type == 2) // top right
        {
            x1 = -tmpX;
            y2 = -tmpY;
        }
        else if (type == 3) // bottom right
        {
            y1 = tmpY;
            x2 = -tmpX;
        }
        else if (type == 4) // bottom left
        {
            y1 = tmpY;
            x2 = tmpX;
        }

        var position = item.WorldPosition(type - 1);
        if (w == 0 && h == 0) position = item.startAnglePos[type - 1];

        angleLine.SetPosition(0, position + new Vector3(x1, y1, 0));
        angleLine.SetPosition(1, position);
        angleLine.SetPosition(2, position + new Vector3(x2, y2, 0));
    }

    public void HideLine()
    {
        lineRenderer.gameObject.SetActive(false);
        Gradient lineRendererGradient = new Gradient();
        lineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f) }
            );
        lineRenderer.colorGradient = lineRendererGradient;
    }

    public IEnumerator FadeLineRenderer(float fadeSpeed)
    {
        Gradient lineRendererGradient = new Gradient();
        float timeElapsed = 0f;
        float alpha = 1f;

        while (timeElapsed < fadeSpeed)
        {
            alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeSpeed);

            lineRendererGradient.SetKeys
            (
                lineRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f) }
            );
            lineRenderer.colorGradient = lineRendererGradient;

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void HideRect(int rectIndex)
    {
        if (rectIndex == -1)
        {
            rectItem1.gameObject.SetActive(false);
            rectItem2.gameObject.SetActive(false);
        }
        else
        {
            var rectItem = rectIndex == 1 ? rectItem1 : rectItem2;
            rectItem.gameObject.SetActive(false);
        }
    }
}