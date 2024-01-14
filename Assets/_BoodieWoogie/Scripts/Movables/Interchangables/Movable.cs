using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Movable : MonoBehaviour, IInterchangable
{
    [SerializeField]
    private AnimationCurve speedCurve = new AnimationCurve();
    [SerializeField]
    private GameObject SelectedSpriteR;
    [SerializeField]
    private GameObject SelectedSpriteL;
    [SerializeField]
    float speed = 1.0f;
    [SerializeField]
    private UnityEvent onTranslationBegin = new UnityEvent();
    [SerializeField]
    private UnityEvent onTranslationEnd = new UnityEvent();
    private bool isSelected = false;

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Interchange(Vector2 position)
    {
        StartCoroutine(MoveToInterchangePoint(position));
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void Select(EInterchangableSide side)
    {
        isSelected = true;
        if (!SelectedSpriteR || !SelectedSpriteL)
        {
            return;
        }
        if (side == EInterchangableSide.R)
        {
            SelectedSpriteR.SetActive(true);
            return;
        }
        SelectedSpriteL.SetActive(true);
    }

    public void Unselect()
    {
        isSelected = false;
        if (!SelectedSpriteR || !SelectedSpriteL)
        {
            return;
        }
        SelectedSpriteR.SetActive(false);
        SelectedSpriteL.SetActive(false);
    }
    public IEnumerator MoveToInterchangePoint(Vector3 targetPos)
    {
        Vector3 difference = targetPos - transform.position;
        float distance = difference.magnitude;
        float originalDistance = distance;
        while (distance > 0.0f)
        {
            transform.position = targetPos - difference.normalized * distance;
            distance -= Time.deltaTime * speed * speedCurve.Evaluate(distance/originalDistance);
            yield return null;
        }
        transform.position = targetPos;
    }
}
