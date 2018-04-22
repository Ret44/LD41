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
    public bool isDying = false;
 

    public int HP = 3;

    [SerializeField]
    private SpriteRenderer renderer;

    [SerializeField]
    private Animator _animator;

    public void Setup()
    {
        movementOffset = Random.insideUnitCircle * 0.45f;
        transform.position = TowerDef.instance.pathStart.transform.position + movementOffset;
        targetField = TowerDef.instance.pathStart.nextPath;
        MoveTo(targetField);
    }

    public void MoveTo(FieldBase field)
    {
        Vector3 targetPos = field.transform.position + movementOffset;
        if (field.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = Vector3.one;
        transform.DOMove(targetPos, timeBetweenFields).SetEase(Ease.Linear).OnComplete(OnTargetReached);
    }
    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void Hit(int dmg)
    {
        HP -= dmg;
        if(HP<0)
        {
            transform.DOKill();
            isDying = true;
            _animator.SetTrigger("dead");
        }
        else
        {
            _animator.SetTrigger("hit");
        }
    }

    public void OnTargetReached()
    {
        targetField = targetField.nextPath;
        if (targetField != null)
        {
            MoveTo(targetField);
        }
        else
            Destroy(this.gameObject);
    }

    public void OnDestroy()
    {
        transform.DOKill();
        TowerDef.RemoveEnemy(this);
    }

    public void Update()
    {
        renderer.sortingOrder = (int)(transform.position.y * -1000);
    }
}
