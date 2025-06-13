using UnityEngine;

public class AreaOfEffectHandler : MonoBehaviour
{

    public float max_lifetime = 2f;
    public float cooldown = 1;
    public float start_strength = 15f;
    private float strength;
    //private float cooldown_elapse = cooldown;
    //private bool cooldown_active = false;
    private float lifetime;
    private bool dead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        lifetime = max_lifetime;
        dead = false;
        strength = start_strength;
    }
    
    void OnEnable()
    {
        lifetime = max_lifetime;
        dead = false;
        strength = start_strength;
        //cooldown_elapse = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime <= max_lifetime/6f){
            strength = -start_strength*6;
        }
        if(lifetime <= 0f && !dead){
            dead = true;
            Die();
        }
    }

    void FixedUpdate(){

    }

    private void Die(){
        // do death things
        lifetime = max_lifetime;
        dead = false;
        strength = start_strength;
        GameObjectPoolManager.Deactivate(gameObject);
    }

    void OnTriggerEnter(Collider col){

    }

    void OnTriggerStay(Collider col){
        float enemy_hp = -2f;
        if (col.gameObject.TryGetComponent<EnemyStats>(out var enemy)) {
            enemy_hp = enemy.TakeDamage(0.1f);
            print(gameObject.name + " hit " + enemy.name + " for " + enemy_hp +" health left!");
        }
        if (col.attachedRigidbody != null) {
            Vector3 vector = gameObject.transform.position - col.transform.position;
            col.attachedRigidbody.AddForce(vector * strength, ForceMode.Acceleration);
        }
    }
}
