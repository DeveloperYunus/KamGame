using UnityEngine;

public class EnemySword : MonoBehaviour
{
    public Transform enemyObj;                  //parent enemy objesi
    public float damage;
    [Tooltip("4 = Mage Boss push")] public int dmgKind;
    public float pushStrong;
}