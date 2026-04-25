using System.Collections;
using UnityEngine;

// public class BulletColision : MonoBehaviour
// {

//     void Start()
//     {

//         Destroy(gameObject, 3f); // ทำลายกระสุนใน 3 วินาที
//     }
//     void OnCollisionEnter(Collision collision)
//     {
//         if (impactEffect1 != null && impactEffect2 != null)
//         {
//             // ถ้ามี Particle Effect ให้โชว์ตรงจุดที่โดน

//             if (hitting)
//             {
//                 // ใช้ข้อมูลจุดปะทะแรก
//                 RaycastHit contact = hit;
//                 // ใช้ 'normal' ของพื้นผิว เพื่อให้ Effect หันหน้าออกจากพื้นผิวพอดี
//                 Quaternion rotation = Quaternion.LookRotation(contact.normal);

//                 Instantiate(impactEffect1, contact.point, rotation);
//                 Instantiate(impactEffect2, contact.point, rotation);
//                 impactEffect1.transform.SetParent(hit.transform);
//                 impactEffect2.transform.SetParent(hit.transform);
//                 impactEffect1.transform.localScale = Vector3.one;
//                 impactEffect2.transform.localScale = Vector3.one;
//             }
//             else
//             {
//                 // ใช้ข้อมูลจุดปะทะแรก
//                 ContactPoint contact = collision.contacts[0];
//                 // ใช้ 'normal' ของพื้นผิว เพื่อให้ Effect หันหน้าออกจากพื้นผิวพอดี
//                 Quaternion rotation = Quaternion.LookRotation(contact.normal);

//                 Instantiate(impactEffect1, contact.point, rotation);
//                 Instantiate(impactEffect2, contact.point, rotation);
//                 impactEffect1.transform.SetParent(collision.transform);
//                 impactEffect2.transform.SetParent(collision.transform);
//                 impactEffect1.transform.localScale = Vector3.one;
//                 impactEffect2.transform.localScale = Vector3.one;
//             }

//             // ทำลาย effect ทิ้งหลังจากเล่นจบ (เช่น 2 วินาที)
//             Destroy(impactEffect1, 2f);
//             Destroy(impactEffect2, 2f);
//    // เช็คว่า Object นั้นรับดาเมจได้ไหม
//             IDamageable target = collision.transform.GetComponent<IDamageable>();
//             if (target != null)
//             {
//                 target.TakeDamage(damage);
//                 EnemyHealth HealthScript = collision.transform.GetComponent<EnemyHealth>();
//                 Debug.Log(HealthScript.health);
//             }

//         }


//         // 2. ลบลูกกระสุนทิ้ง
//         Destroy(gameObject);
//     }
// }




public class BulletColision : MonoBehaviour
{
    public float damage = 30f;
    public RaycastHit hit;
    public bool IsHit = false;
    public float speed;
    public float Range;

    public GameObject impactEffect1;
    public GameObject impactEffect2;
    public AudioSource sound;
    public float explosionRadius = 5f; // รัศมีระเบิด
    public float explosionDamage = 30f; // ดาเมจที่จะทำ
    public LayerMask CreatureLayer; // กำหนด Layer ของศัตรูไว้ใน Inspector

    private Vector3 startPosition;
    void Start()
    {
        // คำนวณเวลาที่กระสุนควรอยู่ได้ตามระยะทาง
        Destroy(gameObject, 10f);

        // เก็บตำแหน่งเริ่มต้นไว้
        startPosition = transform.position;
    }

    // ส่วนที่ใช้เช็คการชนแบบแม่นยำ (กันกระสุนทะลุ)
    void FixedUpdate()
    {
        if(IsHit == false) return;
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) return;

        float currentDistance = Vector3.Distance(startPosition, transform.position);

        if (currentDistance >= Range)
        {
                if (hit.transform)
                {
                    HandleCollision(hit.point, hit.normal, hit.transform);
                    IsHit = false;
                }
                else
                {
                    HandleCollision(hit.point, hit.normal, null);
                }
        }
    }

    // กรณีที่ชนปกติ (เผื่อกรณีชนช้าๆ แล้ว Raycast พลาด)
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Gun"))
        {
            ContactPoint contact = collision.contacts[0];
            HandleCollision(contact.point, contact.normal, collision.transform);
        }
    }

    void HandleCollision(Vector3 pos, Vector3 normal, Transform hitTarget)
    {

        Quaternion rotation = Quaternion.LookRotation(normal);
        GameObject PrefabimpactEffect1 = Instantiate(impactEffect1, pos, rotation);
        GameObject PrefabimpactEffect2 = Instantiate(impactEffect2, pos, rotation);


        // 1. เช็คก่อนว่า hitTarget มีตัวตนจริงไหม ถึงค่อย SetParent
        if (hitTarget != null)
        {
            PrefabimpactEffect1.transform.SetParent(hitTarget);
            PrefabimpactEffect2.transform.SetParent(hitTarget);
            // 1. หาวัตถุทั้งหมดที่อยู่ในรัศมีระเบิด
                    Collider[] hitColliders = Physics.OverlapSphere(pos, explosionRadius, CreatureLayer);

                    foreach (var hitCollider in hitColliders)
                    {
                        Debug.Log(hitCollider.transform.name);
                        // 2. ถ้าเจอศัตรู ให้เรียกใช้ฟังก์ชันลดเลือด (สมมติว่าศัตรูมีสคริปต์ชื่อ EnemyHealth)
                        EnermiesBehaviour enemy = hitCollider.transform.root.GetComponent<EnermiesBehaviour>();
                        if (hitCollider.transform.root.CompareTag("ManTreeChoper"))
                        {
                            enemy.Health -= explosionDamage;
                            break;
                        }
                    }
        }
        
        Destroy(PrefabimpactEffect1, 2f);
        Destroy(PrefabimpactEffect2, 2f);
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        sound.Play();
        Renderer rend = gameObject.GetComponent<Renderer>();
        // ดึงสีปัจจุบันออกมา
        Color color = rend.material.color;
        // ปรับค่า alpha (0.0 ถึง 1.0)
        color.a = 0;
        // ใส่สีที่ปรับแล้วกลับเข้าไป
        rend.material.color = color;
        yield return new WaitForSeconds(sound.clip.length);
        // ทำลายตัว effect และลูกกระสุน
        
        Destroy(gameObject);
    }
}