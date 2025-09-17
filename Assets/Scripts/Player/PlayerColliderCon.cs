using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderCon : MonoBehaviour
{
    public PlayerController thePlayer;
    public BoxCollider2D playerCol;

    public CircleCollider2D groundcheckCollider;

    public Transform groundcheck;

    [SerializeField] Vector2 standOffset, standSize;
    [SerializeField] Vector2 crouchOffset, crouchSize;

    [SerializeField] Vector2 standGroundCheckOffset, crouchGroundCheckOffset;
    [SerializeField] Vector2 standGroundCheckColliderPos, crouchGroundCheckColliderPos;
    [SerializeField] List<Vector2> standAttackPointOffsets;  // Offsets for each attack point when standing
    [SerializeField] List<Vector2> crouchAttackPointOffsets;
    public List<Transform> attackPoints;

    // Start is called before the first frame update
    void Start()
    {
        thePlayer = GetComponent<PlayerController>();

        standSize = playerCol.size;
        standOffset = playerCol.offset;

        standGroundCheckColliderPos = groundcheckCollider.offset;
        standGroundCheckOffset = groundcheck.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (thePlayer.crouching)
        {
            playerCol.size = crouchSize;
            playerCol.offset = crouchOffset;

            groundcheckCollider.offset = crouchGroundCheckColliderPos;
            groundcheck.localPosition = crouchGroundCheckOffset;

            for (int i = 0; i < attackPoints.Count; i++)
            {
                attackPoints[i].localPosition = crouchAttackPointOffsets[i];
            }
        }

        if (!thePlayer.crouching)
        {
            playerCol.size = standSize;
            playerCol.offset = standOffset;

            groundcheckCollider.offset = standGroundCheckColliderPos;

            groundcheck.localPosition = standGroundCheckOffset;

            for (int i = 0; i < attackPoints.Count; i++)
            {
                attackPoints[i].localPosition = standAttackPointOffsets[i];
            }
        }
    }
}
