using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Upgrades", menuName = "Upgrade Info")]
public class UpgradesInfo : ScriptableObject
{
    public List<UpgradeList> upgrades = new List<UpgradeList>();
}

[Serializable]
public class UpgradeList
{
    public string name;
    public List<Upgrade> elementList;
}

[Serializable]
public class Upgrade
{
    public float Value;
    public int Cost;
}
