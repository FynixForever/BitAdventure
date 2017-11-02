using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2DScript : MonoBehaviour {

    struct RayCastOrigins
    {
        public Vector2 bottomLeft, bottomRight;
        public Vector2 topLeft, topRight;
    }
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool semiFloor;

        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;

        public Vector3 velocityOld;

        public void Reset(Vector3 velocity)
        {
            above = below = false;
            left = right = false;

            semiFloor = false;

            climbingSlope = descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            velocityOld = velocity;
        }
    }

    new BoxCollider2D collider;
    RayCastOrigins rcOrigins;

    public LayerMask collisionMask;

    const float skinWidth = 0.02f;

    public int horRayCount = 4;
    public int verRayCount = 4;

    float horRaySpacing;
    float verRaySpacing;

    public float maxClimbAngle = 75f;
    public float maxDescendAngle = 75f;

    public CollisionInfo collisions;

    void Start() {
        collider = GetComponent<BoxCollider2D>();

        CalculateRayCastSpacing();
    }

    public void MovePlayer(Vector3 velocity, bool moveThroughFloor)
    {
        //Calculate raycast positions every frame
        UpdateRayCastOrigins();

        //Reset saved collisions from last frame
        collisions.Reset(velocity);

        //Check collisions for descending slopes
        if (velocity.y < 0)
            DescendSlope(ref velocity);

        //Check raycast in vertical and horizontal direction
        if (velocity.x != 0)
            HorizontalMove(ref velocity);
        if (velocity.y != 0)
            VerticalMove(ref velocity, moveThroughFloor);

        //Actually move the player
        transform.Translate(velocity);
    }

    void HorizontalMove(ref Vector3 velocity)
    {
        //Check which direction the player is going
        float direction = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        Vector2 rayOrigin = (direction == 1) ? rcOrigins.bottomRight : rcOrigins.bottomLeft;

        for (int i = 0; i < horRayCount; i++)
        {
            //Fire raycast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * direction, rayLength, collisionMask);

            if (hit)
            {
                //Don't collide a FloorOnly from the bottom or when moving up
                string tag = hit.collider.tag;
                if (tag != "Solids")
                    return;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //Check if can climb a slope
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    //For smooth transitions between descending and climbing slopes
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }

                    //Climb the slope
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * direction;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * direction;
                }


                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    //Change velocity accordingly and change rayLength
                    velocity.x = (hit.distance - skinWidth) * direction;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);

                    //Save collisions
                    collisions.right = direction == 1;
                    collisions.left = direction == -1;
                }
            }

            //DEBUG
            //Debug.DrawRay(rayOrigin, Vector2.right * direction * rayLength, Color.red);

            //Next ray
            rayOrigin += Vector2.up * horRaySpacing;
        }
    }
    void VerticalMove(ref Vector3 velocity, bool moveThroughFloor)
    {
        //Check which direction the player is going
        float direction = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        Vector2 rayOrigin = (direction == 1) ? rcOrigins.topLeft : rcOrigins.bottomLeft;

        for (int i = 0; i < verRayCount; i++)
        {
            //Fire raycast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * direction, rayLength, collisionMask);

            if (hit)
            {
                //Don't collide a FloorOnly from the bottom or when moving up
                string tag = hit.collider.tag;
                if (tag == "Floor Only" && (direction == 1 || moveThroughFloor))
                    return;

                //Change velocity accordingly, and change rayLength to be able to stand on corners
                velocity.y = (hit.distance - skinWidth) * direction;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);

                //Save collisions
                collisions.above = direction == 1;
                collisions.below = direction == -1;
                collisions.semiFloor = tag == "Floor Only";
            }

            //DEBUG
            //Debug.DrawRay(rayOrigin, Vector2.up * direction * rayLength, Color.red);

            //Next ray
            rayOrigin += Vector2.right * verRaySpacing;
        }

        //For smooth transitions between slopes of different angles
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            rayOrigin = ((directionX == -1) ? rcOrigins.bottomLeft : rcOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                //Don't collide a FloorOnly from the bottom or when moving up
                string tag = hit.collider.tag;
                if (tag == "Floor Only")
                    return;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float distanceSign = Mathf.Sign(velocity.x);

        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * distanceSign;

            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }
    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? rcOrigins.bottomRight : rcOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            //Don't collide a FloorOnly from the bottom or when moving up
            string tag = hit.collider.tag;
            if (tag != "Solids")
                return;
            
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;
                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    void UpdateRayCastOrigins()
    {
        if (collider == null)
            return;

        Bounds bounds = collider.bounds;

        bounds.Expand(skinWidth * -2);

        rcOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        rcOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        rcOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        rcOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRayCastSpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        
        horRayCount = Mathf.Clamp(horRayCount, 2, int.MaxValue);
        verRayCount = Mathf.Clamp(verRayCount, 2, int.MaxValue);

        horRaySpacing = bounds.size.y / (horRayCount - 1);
        verRaySpacing = bounds.size.x / (verRayCount - 1);
    }
}
