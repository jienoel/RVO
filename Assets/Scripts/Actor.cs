using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RVO;
using Vector2 = UnityEngine.Vector2;
public class Actor : MonoBehaviour
{

    public bool isDrawGizmo;

    public Actor target;
    public Actor neightbor;

    public float radius;
    public float maxSpeed;
   // public float neighborDist;
    public float timeHorizon;
    public Agent agent;
    public Vector2 velocity;
    public bool runing;
    void Start()
    {
       // BindAgent();
    }

    public void BindAgent()
    {
        agent  = new Agent();
        agent.actor = this;
        agent.id_ = GetInstanceID();
        agent.maxNeighbors_ = 10;
        agent.radius_ = radius;
        agent.maxSpeed_ = maxSpeed;
        //agent.neighborDist_ = neighborDist;
        agent.timeHorizon_ = timeHorizon;
        agent.velocity_ = new RVO.Vector2(0,0);
    }

    public void SetPreferredVelocities()
    {
        RVO.Vector2 goalVector = new RVO.Vector2(target.transform.position.x, target.transform.position.z) - agent.position_;
        if (RVOMath.absSq(goalVector) > 1.0f)
        {
            goalVector = RVOMath.normalize(goalVector);
        }
        agent.prefVelocity_ = goalVector;
    }

    private void OnDrawGizmosSelected()
    {
		    if(!isDrawGizmo)
                return;
        Color tmp = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawLine( transform.position, target.transform.position );
        Gizmos.color = Color.blue;
        Gizmos.DrawRay( transform.position, new Vector3(agent.velocity_ .x(),0,agent.velocity_ .y()));
        Gizmos.DrawRay( neightbor.transform.position, new Vector3(neightbor.agent.velocity_.x(),0, neightbor.agent.velocity_.y()) );
        Gizmos.color = Color.green;
        Gizmos.DrawLine(  transform.position, neightbor.transform.position);
    }

    class Line
    {
        public Vector2 point;
        public Vector2 direction;
    }
    List<Line> obstacleLines = new List<Line>();

    float Det( Vector2 a, Vector2 b )
    {
        return a.x * b.y - a.y * b.x;
    }

    void CalculateLine()
    {
        float invTimeHorizon = 1/timeHorizon;
        var tmp = neightbor.transform.position - transform.position;
        UnityEngine.Vector2 relativePos = new UnityEngine.Vector2(tmp.x, tmp.z);
        Vector2 relativeVelocity = velocity - neightbor.velocity;
        float distSq = relativePos.sqrMagnitude;
        float combinedRadius = radius + neightbor.radius;
        float combinedRadiusSq = combinedRadius * combinedRadius;
        Vector2 u;
        Line line = new Line();
        //Debug.Log( string.Format( " pos:{0}" ) );
        if( distSq > combinedRadiusSq )
        {
            Vector2 w = relativeVelocity - invTimeHorizon * relativePos;
            float wLengthSq = w.sqrMagnitude;
            float dotProduct1 = Vector2.Dot( w , relativePos );
            Debug.Log( "w :" + w  + "wLengthSq:   "+wLengthSq+"dotProduct1:  "+ dotProduct1);
            if( dotProduct1 < 0 && Mathf.Pow( dotProduct1, 2 ) > combinedRadiusSq * wLengthSq )
            {
                float wLength = Mathf.Sqrt( wLengthSq );
            
                Vector2 unitW = w.normalized;
                line.direction = new Vector2(unitW.y, unitW.x);
                u = (combinedRadius * invTimeHorizon - wLength) * unitW;
            }
            else
            {
                float leg = Mathf.Sqrt( distSq - combinedRadiusSq );
                if( Det( relativePos, w ) > 0 )
                {
                    line.direction = new Vector2(relativePos.x * leg - relativePos.y * combinedRadius,  relativePos.x * combinedRadius + relativePos.y*leg)/distSq;
                }
                else
                {
                    line.direction = -new Vector2( relativePos.x * leg + relativePos.y * combinedRadius,
                                                   -relativePos.x * combinedRadius + relativePos.y * leg ) / distSq;
                }
                float dotProduct2 = Vector2.Dot( relativeVelocity, line.direction );
                u = dotProduct2 * line.direction - relativeVelocity;//dotproduct2 * line.direction 代表 relativeVelocity 在line.direction方向上的投影
            }
        }
    }
}
