using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkillInfo : MonoBehaviour
{
    // Start is called before the first frame update

    public int SkillProfession = 0;
    public int SkillID = 0;
    public int isEquiped = 0;

    public float Damage = 2, CoolDown = 1, Duration = 0.2f, DamageInterval = 1, MPCost = 1;

    public bool isRefresh = false, isPre = false;

}
