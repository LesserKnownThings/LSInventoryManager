using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="DynamicClass", menuName = "Lesser/Dynamic")]
public class DynamicData
{	
	private static DynamicData instance;
	public static DynamicData Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new DynamicData();
			}
			return instance;
			}
	}	
	public class ItemsClass
	{
		public float damage;
					public bool isInvincible;
		public ItemsClass
		(
		float damage
			,		bool isInvincible
		) 
		{
		this.damage = damage;
					this.isInvincible = isInvincible;
		}
	}
	public ItemsClass sword = new ItemsClass
	(
	25f,
true
	);
}
