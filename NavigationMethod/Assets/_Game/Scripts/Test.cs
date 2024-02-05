using UnityEngine;
using Wonnasmith.ProjectileMotion;

public class Test : MonoBehaviour
{
    public Transform startTR;
    public Transform endTR;

    public float height;

    public Projectile projectile;

    public float speed;
    public bool isMove = false;
    public bool isDraw = false;

    [ContextMenu("Init")]
    private void Start()
    {
        projectile = new Projectile(startTR.position, endTR.position, height);
    }

    private void Update()
    {
        if (!isMove) return;

        Vector3 zz = projectile.BernsteinMoveUpdate(speed).Item1;
        transform.position = zz;
    }

    private void OnDrawGizmos()
    {
        if (!isDraw) return;

        if (projectile == null) return;
        if (projectile._bernsteinPolynomalPos == null) return;
        if (projectile._bernsteinPolynomalPos.Length == 0) return;

        for (int v = 0; v < projectile._bernsteinPolynomalPos.Length; v++)
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawSphere(projectile._bernsteinPolynomalPos[v], 5f);
        }
    }
}
