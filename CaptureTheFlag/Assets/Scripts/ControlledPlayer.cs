using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlledPlayer : GeneralPlayer
{
    private PlayerMovement movement;
    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<PlayerMovement>();
        ScoreSide = GameMenuManager.Side.Right;

    }

    protected override IEnumerator Invincibility() {
        Invincible = true;
        movement.EnableControls(false);
        yield return new WaitForSeconds(invincibilityTime);
        movement.EnableControls(true);
        Invincible = false;
    }

    public override void ScoreFlag() {
        transform.position = startPos;
        base.ScoreFlag();
    }
}
