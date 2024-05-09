using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class SingleMonsterBossController : CreatureController
{
    public BossMonster boss;
    public SingleMyPlayerController _target;
    public SingleGameManager gameManager;
    protected Animator anim;
    public GameObject bullet;
    public Slider hpSlider;
    public bool _canChase = false, phaseChange=true;
    public int maxHp;
    private float lastInstantiateTime; // 마지막으로 Instantiate가 호출된 시간
    public float instantiateInterval = 0.3f; // Instantiate를 호출할 간격 (1초로 설정)
    public SingleMyPlayerController Target { get { return _target; } set { _target = value; } }
    private NavMeshAgent navMesh;
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        navMesh.enabled = true;
        navMesh.speed = boss.movingspeed;
        navMesh.updateRotation = false;
        navMesh.updateUpAxis = false;
        _target = FindObjectOfType<SingleMyPlayerController>();
        boss=GetComponent<BossMonster>();
        anim=GetComponent<Animator>();
        maxHp = boss.hp;
        gameManager = GameObject.Find("SingleGameManager").GetComponent<SingleGameManager>();
    }
    protected void LookAtPlayer(bool _setAnim, bool _reverse = false)
    {
        if(_setAnim)
            anim.SetBool("isAttack", true);
        
        Vector2 value = _target.transform.position - transform.position;
        if(_reverse)
            value = transform.position - _target.transform.position ;
        transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
        if (Time.time - lastInstantiateTime >= instantiateInterval)
        {
            GameObject bulletGo = GetBullet();
            if (_reverse)
            {
                bulletGo.GetComponent<BossBulletController>().Dir = -transform.up;
                bulletGo.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 180);
            }
            lastInstantiateTime = Time.time; // 마지막 호출 시간을 업데이트
        }
    }
    protected void Spin()  //페이즈 원
    {
        anim.SetBool("isAttack", false);
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z+ Time.deltaTime* (boss.spinningSpeed));
        if (Time.time - lastInstantiateTime >= instantiateInterval)
        {
            GetBullet(true);
            lastInstantiateTime = Time.time; // 마지막 호출 시간을 업데이트
        }
    }
    private  GameObject GetBullet(bool _reverse = false)
    {
        GameObject bulletGo = ObejctPoolManager.instance.bossBulletPool.Get();
        bulletGo.GetComponent<BossBulletController>().bulletDamage = boss.damage;
        bulletGo.GetComponent<BossBulletController>().Dir = -transform.up;
        bulletGo.transform.position = transform.position;
        if(!_reverse)
            bulletGo.transform.rotation = transform.rotation;
        else
            bulletGo.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 180);
        return bulletGo;
    }
    protected void FireBall(bool _setAnim, bool _threeShot, bool _reverse = false)
    {
        Vector2 value = _target.transform.position - transform.position;
        if (_reverse)
            value = transform.position - _target.transform.position;
        transform.eulerAngles = new Vector3(0, 0, -Mathf.Atan2(value.x, value.y) * Mathf.Rad2Deg);
        if (Time.time - lastInstantiateTime >= 1.3f)
        {
            if (_setAnim)
                anim.SetTrigger("doAttack");

            GameObject fireBall = Managers.Resource.Instantiate("SinglePlay/Creature/Boss/Fireball");
            SetFireBall(fireBall, _reverse);
            if (_threeShot)
                StartCoroutine(CoFireThree(_reverse));
            lastInstantiateTime = Time.time; // 마지막 호출 시간을 업데이트
        }
    }
    private IEnumerator CoFireThree(bool _reverse)
    {
        yield return new WaitForSeconds(0.3f);
        GameObject fireBall2 = Managers.Resource.Instantiate("SinglePlay/Creature/Fireball");
        SetFireBall(fireBall2, _reverse);

        yield return new WaitForSeconds(0.3f);
        GameObject fireBall3 = Managers.Resource.Instantiate("SinglePlay/Creature/Fireball");
        SetFireBall(fireBall3, _reverse);
    }
    private void SetFireBall(GameObject _fireBall, bool _reverse)
    {
        _fireBall.GetComponent<BossFireballController>().Dir = transform.up;
        _fireBall.transform.position = transform.position;
        _fireBall.transform.rotation = transform.rotation;
        if (_reverse)
        {
            _fireBall.GetComponent<BossFireballController>().Dir = -transform.up;
            _fireBall.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z - 180);
        }
    }
    public void WaitCrash()
    {
        if (Vector2.Distance(transform.position, _target.transform.position) <= 4f)
        {
            _target.GapHp = boss.damage;
            _target.Hp -= boss.damage;
            _target.OnDamaged();
        }
    }
    public override void OnDamaged()
    {
        base.OnDamaged();
        if (Managers.Sound.SoundOn == true)
            Managers.Sound.Play("Effect/Player/HitSound");
    }
    protected void applyHpSlider()
    {
        hpSlider.value = (float)boss.hp / (float)maxHp;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<SingleMyPlayerController>().GapHp = boss.damage;
            collision.GetComponent<SingleMyPlayerController>().Hp -= boss.damage;
            collision.GetComponent<SingleMyPlayerController>().OnDamaged();
        }
    }
    protected void FollowPlayer()
    {
        navMesh.SetDestination(_target.transform.position);
        float dist = Vector2.Distance(_target.transform.position, transform.position);
        if (dist <= 3)
            navMesh.isStopped = true;
        else
            navMesh.isStopped = false;
    }
    protected void StopFollow()
    {
        navMesh.isStopped = true;
    }
}
