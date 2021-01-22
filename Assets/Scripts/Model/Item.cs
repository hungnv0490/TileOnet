using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [HideInInspector]
    public int index;

    [HideInInspector]
    public Point point;

    public List<Transform> angles;

    public int rectIndex;

    [HideInInspector]
    public List<Vector3> startAnglePos;

    private float offsetMove;
    public bool moving = false;

    private IEnumerator scaleIE, moveIE;

    public int instanceId;

    void Awake()
    {
        moving = false;
        instanceId = gameObject.GetInstanceID();
        startAnglePos = new List<Vector3>();
    }

    public void Reset4AnglePos()
    {
        startAnglePos.Clear();
        startAnglePos.Add(WorldPosition(0));
        startAnglePos.Add(WorldPosition(1));
        startAnglePos.Add(WorldPosition(2));
        startAnglePos.Add(WorldPosition(3));
    }

    public Vector3 WorldPosition(int type)
    {
        return angles[type].position;
    }

    public void SetTargetPosition(Vector3 first, Vector3 target, float ratio = 30)
    {
        if (first != target)
        {
            // offsetMove = Vector3.Distance(target, first) / ratio;
            // targetPos = target;
            // moving = true;
            if (moveIE != null) StopCoroutine(moveIE);
            moveIE = MoveToTarget(target, ratio);
            StartCoroutine(moveIE);
        }
    }

    public void OnClick()
    {
        // Debug.Log("Touch:" + point);
        rectIndex = 0;
        GameManager.instance.CheckMatch(this);
    }

    void Update()
    {
        // if (!moving) return;

        // if (transform.position == targetPos) moving = false;

        // transform.position = Vector3.MoveTowards(transform.position, targetPos, offsetMove);
    }

    public void StopScale()
    {
        if (scaleIE != null)
        {
            StopCoroutine(scaleIE);
            scaleIE = null;
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void Scale(bool onlyDec, float offset, float timePerOnce)
    {
        scaleIE = ScaleIE(onlyDec, offset, timePerOnce);
        StartCoroutine(scaleIE);
    }

    private IEnumerator ScaleIE(bool onlyDec, float offset, float timePerOnce)
    {
        var localScale = gameObject.transform.localScale;
        bool inc = false;
        while (true)
        {
            var newLS = gameObject.transform.localScale;
            if (!inc)
            {
                newLS.x -= offset;
                newLS.y -= offset;
                if (newLS.x <= 0.5f)
                {
                    if (!onlyDec) inc = true;
                }
                gameObject.transform.localScale = newLS;
            }
            else
            {
                newLS.x += offset;
                newLS.y += offset;
                gameObject.transform.localScale = newLS;

                if (newLS.x >= 1f)
                {
                    gameObject.transform.localScale = new Vector3(1, 1, 1);
                    yield break;
                }
            }

            if (onlyDec) EffectController.instance.SetupRectAroundItem(rectIndex, this, true);

            if (newLS.x <= 0.1f)
            {
                EffectController.instance.HideRect(rectIndex);
                EffectController.instance.HideLine();
                ObjectPool.Instance.ReleaseObject(gameObject);
                yield break;
            }
            yield return new WaitForSeconds(timePerOnce);
        }
    }

    public void MoveWhenWrong()
    {
        StartCoroutine(MoveWhenWrongIE());
    }

    private IEnumerator MoveWhenWrongIE()
    {
        moving = true;
        var curPos = gameObject.transform.position;
        var tmpPos = curPos;
        float offset = 0;
        int type = 1;//to right
        while (true)
        {
            if (type == 1)
            {
                offset += 0.1f;
                tmpPos.x = curPos.x - offset;
                if (offset >= 0.3)
                {
                    curPos.x = tmpPos.x;
                    type = 2;
                    offset = 0;
                }
                // Debug.Log("type 1:" + type + " tmp:" + tmp);
            }
            else if (type == 2) // to left
            {
                offset += 0.1f;
                tmpPos.x = curPos.x + offset;
                if (offset >= 0.6)
                {
                    curPos.x = tmpPos.x;
                    offset = 0;
                    type = 3; // back center
                }
                // Debug.Log("type 2:" + type + " tmp:" + tmp);
            }
            else if (type == 3)
            {
                offset += 0.1f;
                tmpPos.x = curPos.x - offset;
                // Debug.Log("type 3:" + type + " tmp:" + tmp + " tmpPos:" + tmpPos.x + " position.x: " + position.x);

                if (offset == 0.3f)
                {
                    gameObject.transform.position = tmpPos;
                    break;
                }
            }
            gameObject.transform.position = tmpPos;

            yield return new WaitForSeconds(0.01f);
        }
        moving = false;
    }

    private IEnumerator MoveToTarget(Vector3 targetPos, float ratio = 30)
    {
        moving = true;
        var maxMove = Vector3.Distance(targetPos, transform.position) / ratio;
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        Reset4AnglePos();
        moving = false;
    }
}
