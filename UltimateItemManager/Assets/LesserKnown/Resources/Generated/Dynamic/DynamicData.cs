using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Item", menuName ="LesserKnown/Item")]
public class DynamicData:ScriptableObject
{	
	public string itemName;
	public Sprite itemIcon;
		public float damage;
				public int health;
}
