using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBase : MonoBehaviour {

    [SerializeField]
    [Range(0f, 10f)]
    private float timeBetweenFields;
    public Vector3 movementOffset;
    public FieldBase targetField;

    public void Setup()
    {
        movementOffset = Random.insideUnitCircle * 0.75f;
        transform.position = TowerDef.instance.pathStart.transform.position + movementOffset;
        targetField = TowerDef.instance.pathStart.nextPath;
        MoveTo(targetField);
    }

    public void MoveTo(FieldBase field)
    {
        Vector3 targetPos = field.transform.position + movementOffset;
        transform.DOMove(targetPos, timeBetweenFields).SetEase(Ease.Linear).OnComplete(OnTargetReached);
    }

    public void OnTargetReached()
    {
        targetField = targetField.nextPath;
        if(targetField != null)
        {
            MoveTo(targetField);
        }
    }

    public void OnDestroy()
    {
        transform.DOKill();
        TowerDef.RemoveEnemy(this);
    }

}
