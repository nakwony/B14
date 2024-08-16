using UnityEngine;
using System.Collections;
using _10._Externals.HeroEditor4D.Common.Scripts.CharacterScripts;
using Assets.HeroEditor4D.Common.Scripts.Enums;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [FormerlySerializedAs("playerData")]
    public PlayerDataSO PlayerData;

    [SerializeField]
    private HpBar _hpBar;

    [FormerlySerializedAs("scanner")]
    public Scanner Scanner;

    [FormerlySerializedAs("fireMuzzle")]
    public Transform FireMuzzle;

    [FormerlySerializedAs("damage")]
    public float Damage;

    [FormerlySerializedAs("attackSpeed")]
    public float AttackSpeed;

    [FormerlySerializedAs("currentHp")]
    public int CurrentHp;

    public int CurrentDamage { get; private set; }

    [FormerlySerializedAs("criticalPer")]
    public float CriticalPer;

    [FormerlySerializedAs("criticalMultiplier")]
    public float CriticalMultiplier;

    [FormerlySerializedAs("damageTextPool")]
    private bool _isUsingSkill;

    private Coroutine _attackCoroutine;
    private Animator _animator;
    private AnimationManager _animationManager;

    private void Awake()
    {
        PlayerData = DataManager.Instance.PlayerDataSo;
        Scanner = GetComponent<Scanner>();
        _animator = GetComponent<Animator>();
        _animationManager = GetComponent<AnimationManager>();

        AttackSpeed = PlayerData.AttackSpeed;
        CurrentHp = PlayerData.MaxHp;
        _hpBar.SetMaxHp(PlayerData.MaxHp);
        _hpBar.SetCurrentHp(CurrentHp);

        CriticalPer = PlayerData.CriticalPer;
        CriticalMultiplier = PlayerData.CriticalMultiplier;

        UpdateDamage();
    }

    private void Start()
    {
        StartAttacking();
        StartCoroutine(RecoverHp());
    }

    // ���ݱ��
    private IEnumerator Attack()
    {
        while (true)
        {
            if (!_isUsingSkill && Scanner.NearestTarget != null)
            {
                _animator.Play("Fire1H");
                SoundManager.Instance.Play("Fire",pitch: 0.8f);
                GameObject projectile = ProjectilePool.Instance.GetProjectile();
                projectile.transform.position = FireMuzzle.position;
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.Target = Scanner.NearestTarget;
                projectileScript.SetDirection(Scanner.NearestTarget.transform.position);

                Damage = CurrentDamage;


                projectileScript.Damage = Mathf.RoundToInt(Damage);
                projectileScript.ShooterTag = "Player";
                projectileScript.SetColor(Color.blue);
            }

            yield return new WaitForSeconds(1 / AttackSpeed);
        }
    }


    // ü�� ȸ�� ���
    private IEnumerator RecoverHp()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (CurrentHp < PlayerData.MaxHp && CurrentHp > 0)
            {
                CurrentHp += PlayerData.HpRecovery;
                _hpBar.SetCurrentHp(CurrentHp);
                if (CurrentHp > PlayerData.MaxHp)
                {
                    CurrentHp = PlayerData.MaxHp;
                    _hpBar.SetCurrentHp(CurrentHp);
                }
            }
        }
    }

    // �ǰ� ���
    public void TakeDamage(int damage)
    {
        CurrentHp -= damage;
        _hpBar.SetCurrentHp(CurrentHp);

        if (CurrentHp <= 0)
        {
            // ���� ��Ȱ��ȭ
            GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
            foreach (GameObject monster in monsters)
            {
                if (monster.GetComponent<BossTimer>() != null)
                {
                    monster.GetComponent<BossTimer>().DeactivateTimer();
                }

                monster.SetActive(false);
            }

            StartCoroutine(DeathWithDelay());
        }
    }

    public IEnumerator DeathWithDelay()
    {
        _animationManager.SetState(CharacterState.Death);
        PlayerSpeechBubble.Instance.ShowMessage("(시간을 되돌리는 중...)", SpeechLength.Short, "#0EFCFE");
        SoundManager.Instance.Play("TimeReversed");
        yield return new WaitForSeconds(4f);
        StageManager.StageReset();
        CurrentHp = PlayerData.MaxHp;
        _hpBar.SetCurrentHp(CurrentHp);
        _animationManager.SetState(CharacterState.Idle);
    }

    public void SetUsingSkill(bool usingSkill)
    {
        _isUsingSkill = usingSkill;
    }

    public void ApplyAttackBuff(int amount)
    {
        Debug.Log($"Applying attack buff : {amount}");
        PlayerData.Damage += amount;
        UpdateDamage();
    }

    private void UpdateDamage()
    {
        Debug.Log($"Updating damage. Base: {PlayerData.Damage}, Buff: {PlayerData.Damage}");
        CurrentDamage = PlayerData.Damage;
        Damage = CurrentDamage;
        Debug.Log($"New damage: {CurrentDamage}");
    }

    public void Heal(int amount) // ���� �÷��̾� �ǰ� ������ ���� ����
    {
    }


    public void StartAttacking()
    {
        if (_attackCoroutine == null)
        {
            _attackCoroutine = StartCoroutine(Attack());
        }
    }

    public void StopAttacking()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }
}