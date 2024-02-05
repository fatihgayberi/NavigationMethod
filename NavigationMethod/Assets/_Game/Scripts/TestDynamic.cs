using UnityEngine;
using Wonnasmith.ProjectileMotion;

public class TestDynamic : MonoBehaviour
{
    public Transform startTR;
    public Transform endTR;

    public BernsteinPosData startBernstein;
    public BernsteinPosData endBernstein;

    public ProjectileDynamic projectile;
    public float height;

    public float speed;
    public bool isMove = false;
    public bool isDraw = false;

    [ContextMenu("Init")]
    private void Start()
    {
        startBernstein = new BernsteinPosData(startTR);
        endBernstein = new BernsteinPosData(endTR);

        projectile = new ProjectileDynamic(startBernstein, endBernstein, height);
    }

    private void OnDrawGizmos()
    {
        if (!isDraw) return;

        if (projectile == null) return;
        if (projectile._bernsteinPolynomalPos == null) return;
        if (projectile._bernsteinPolynomalPos.Length == 0) return;

        for (int v = 0; v < projectile._bernsteinPolynomalPos.Length; v++)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(projectile._bernsteinPolynomalPos[v].GetPos(), 5f);
        }
    }

    private void Update()
    {
        if (!isMove) return;

        Vector2 zz = projectile.BernsteinMoveUpdate(speed).Item1;
        transform.position = zz;
    }
}
