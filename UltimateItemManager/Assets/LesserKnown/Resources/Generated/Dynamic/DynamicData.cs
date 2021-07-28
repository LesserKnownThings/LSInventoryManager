using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Item", menuName ="LesserKnown/Item")]
public class DynamicData:ScriptableObject
{	
	public string itemName;
	public Sprite itemIcon;
	public bool isStackable;
	public int stackAmount;
		public int damage;
				public int magicDamage;
				public int armor;
				public int healthAmount;
}
