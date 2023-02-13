namespace GravityMod;

public class GravityComponent : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private bool isEnemy;
    private bool hasDamage;
    private int count;

    public void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        isEnemy = (gameObject.GetComponent<HealthManager>() != null);
        hasDamage = (gameObject.GetComponent<DamageHero>() != null);
        if (rb2d.bodyType == RigidbodyType2D.Static && !hasDamage)
        {
            enabled = false;
        }
        if (!isEnemy && !hasDamage)
        {
            enabled = false;
        }
    }

    public void FixedUpdate()
    {
        count++;
        if (count > 30)
        {
            count = 0;
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            rb2d.isKinematic = false;
            rb2d.freezeRotation = false;

            rb2d.gravityScale = 1.75f;

            Collider2D[] components = gameObject.GetComponents<Collider2D>();
            for (int i = 0; i < components.Length; i++)
            {
                components[i].isTrigger = false;
            }
        }
    }
}