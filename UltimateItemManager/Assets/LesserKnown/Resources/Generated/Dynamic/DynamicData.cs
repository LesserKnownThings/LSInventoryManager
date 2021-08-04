using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///*******************************************************************
/// This is class is dynamically generated                           *
/// !!!!!!!!DO NOT ADD ANYTHING TO IT OR CHANGE IT!!!!!!!            *
/// If you add or change something it might break the other scripts  *
/// See documentation on how to change or add variables to this class*
///*******************************************************************
/// </summary>
[CreateAssetMenu(fileName ="Item", menuName ="LesserKnown/Item")]
public class DynamicData:ScriptableObject
{	
	public string itemName;
	public string inGameName;
	public ItemCategoriesEnum category;
	[TextArea]
	public string description;
	public Sprite itemIcon;
	public bool isStackable;
	public int stackAmount;	
}
