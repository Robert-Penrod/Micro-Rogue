using UnityEngine;

public class StartRoomPlatform : MonoBehaviour
{
    private void Start()
    {
        this.DelayedInvoke(Time.fixedDeltaTime, () =>
        {
            var cols = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x);
            for(int i = 0; i < cols.Length; i++)
            {
                if (cols[i] is CompositeCollider2D) continue;
                Destroy(cols[i].gameObject);
            }
        });
    }
}
