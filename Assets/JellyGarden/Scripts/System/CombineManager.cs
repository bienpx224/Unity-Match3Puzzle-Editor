using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//1.6
public class CombineManager
{
	List<Combine> combines = new List<Combine> ();
	List<Combine> tempCombines = new List<Combine> ();
	Dictionary<Item, Combine> dic = new Dictionary<Item, Combine> ();
	private int maxCols;
	private int maxRows;
	bool vChecking;

	public List<List<Item>> GetCombine ()
	{

		List<List<Item>> combinedItems = new List<List<Item>> ();
		maxCols = LevelManager.THIS.maxCols;
		maxRows = LevelManager.THIS.maxRows;
		combines.Clear ();
		tempCombines.Clear ();
		dic.Clear ();
		int color = -1;
		Combine combine = new Combine ();
		vChecking = false;
		//Horrizontal searching
		for (int row = 0; row < maxRows; row++) {
			color = -1;
			for (int col = 0; col < maxCols; col++) {
				Square square = LevelManager.THIS.GetSquare (col, row);
				if (IsSquareNotNull (square)) {
					CheckMatches (square.item, color, ref combine);
					color = square.item.color;
				}
			}
		}
		vChecking = true;
		//Vertical searching
		for (int col = 0; col < maxCols; col++) {
			color = -1;
			for (int row = 0; row < maxRows; row++) {
				Square square = LevelManager.THIS.GetSquare (col, row);
				if (IsSquareNotNull (square)) {
					CheckMatches (square.item, color, ref combine);
					color = square.item.color;
				}
			}
		}

//		Debug.Log (" test combines detected " + tempCombines.Count);
		CheckCombines ();
//		Debug.Log ("combines detected " + combines.Count);
		//inspect combines
		foreach (Combine cmb in combines) {
//			Debug.Log ("h: " + cmb.hCount + " v: " + cmb.vCount);
//			Debug.Log (cmb.items.Count);
//			Debug.Log (cmb.nextType);

			if (cmb.nextType != ItemsTypes.NONE) {
				Item item = cmb.items [UnityEngine.Random.Range (0, cmb.items.Count)];

				Item draggedItem = LevelManager.THIS.lastDraggedItem;
				if (draggedItem) {
					if (draggedItem.color != item.color)
						draggedItem = LevelManager.THIS.lastSwitchedItem;
					//check the dragged item found in this combine or not and change this type
					if (cmb.items.IndexOf (draggedItem) >= 0) {
						item = draggedItem;
					}
				}
				item.NextType = cmb.nextType;



			}
			combinedItems.Add (cmb.items);			
		}
		return combinedItems;
	}

	void CheckCombines ()
	{
		List<Combine> countedCombines = new List<Combine> ();

		//find and merge cross combines (+)
		foreach (Combine comb in tempCombines) {
			if (tempCombines.Count >= 2) {
				foreach (Item item in comb.items) {
					Combine newComb = FindCombineInDic (item);  
					if (comb != newComb && countedCombines.IndexOf (newComb) < 0 && countedCombines.IndexOf (comb) < 0 && IsCombineMatchThree (newComb)) {
						countedCombines.Add (newComb);
						countedCombines.Add (comb);
						Combine mergedCombine = MergeCombines (comb, newComb);
						combines.Add (mergedCombine);
						foreach (Item item_ in comb.items) {
							dic [item_] = mergedCombine;						
						}
						foreach (Item item_ in newComb.items) {
							dic [item_] = mergedCombine;						
						}

						break;
					}
				}
			}
		} 

		//find simple combines (3,4,5) 
		foreach (Combine comb in tempCombines) {
			if (combines.IndexOf (comb) < 0 && IsCombineMatchThree (comb) && countedCombines.IndexOf (comb) < 0) {
				combines.Add (comb);
				comb.nextType = SetNextItemType (comb);
			}
		}


//		foreach (var pair in dic) {
//
//			if (combines.IndexOf (pair.Value) < 0 && IsCombineMatchThree (pair.Value)) {
//				pair.Value.nextType = SetNextItemType (pair.Value);
//				combines.Add (pair.Value);
//			}
//		}
	}

	Combine MergeCombines (Combine comb1, Combine comb2)
	{ 
		Combine combine = new Combine ();
		combine.hCount = comb1.hCount + comb2.hCount - 1;
		combine.vCount = comb1.vCount + comb2.vCount - 1;
		combine.items.AddRange (comb1.items);
		combine.items.AddRange (comb2.items);
		combine.nextType = SetNextItemType (combine);
		return combine;
	}

	ItemsTypes SetNextItemType (Combine combine)
	{
//		Debug.Log (combine.hCount + " " + combine.vCount);
		if (combine.hCount > 4 || combine.vCount > 4)
			return ItemsTypes.BOMB;
		if (combine.hCount >= 3 && combine.vCount >= 3)
			return ItemsTypes.PACKAGE;
		if (combine.hCount > 3 || combine.vCount > 3) {
			if (LevelManager.THIS.lastDraggedItem) {
				Vector2 dir = LevelManager.THIS.lastDraggedItem.moveDirection;
				if (Math.Abs (dir.x) > Math.Abs (dir.y))
					return ItemsTypes.HORIZONTAL_STRIPPED;
				else
					return ItemsTypes.VERTICAL_STRIPPED;
				
			}
			return (ItemsTypes)UnityEngine.Random.Range (1, 3);
		}
		return ItemsTypes.NONE;
	}

	void CheckMatches (Item item, int color, ref Combine combine)
	{
		//Debug.Log("color " + item.color);
		//if (color != item.color) {

		combine = FindCombine (item);
		//}
		AddItemToCombine (combine, item);
	}

	void AddItemToCombine (Combine combine, Item item)
	{
		combine.AddingItem = item;
		dic [item] = combine;

		if (IsCombineMatchThree (combine)) {
			if (tempCombines.IndexOf (combine) < 0) {
				tempCombines.Add (combine);
				//Debug.Log("add " + combine.GetHashCode());
			}
		}
	}

	bool IsCombineMatchThree (Combine combine)
	{
		if (combine.hCount > 2 || combine.vCount > 2) {
			return true;
		}
		return false;
	}

	bool IsSquareNotNull (Square square)
	{
		if (square == null)
			return false;
		if (square.item == null)
			return false;
		return true;
	}

	Combine FindCombine (Item item)
	{
		Combine combine = null;
		Item leftItem = item.GetLeftItem ();
		if (CheckColor (item, leftItem) && !vChecking)
			combine = FindCombineInDic (leftItem);
		if (combine != null)
			return combine;
		Item topItem = item.GetTopItem ();
		if (CheckColor (item, topItem) && vChecking)
			combine = FindCombineInDic (topItem);
		if (combine != null)
			return combine;

		return new Combine ();

	}

	Combine FindCombineInDic (Item item)
	{
		Combine combine;
		if (dic.TryGetValue (item, out combine)) {
			return combine;
		}
		return new Combine ();
	}

	bool CheckColor (Item item, Item nextItem)
	{
		if (nextItem) {
			if (nextItem.color == item.color && nextItem.currentType != ItemsTypes.BOMB && nextItem.currentType != ItemsTypes.INGREDIENT)//2.0
				return true;
		}
		return false;
	}

}

public class Combine
{
	private Item addingItem;
	public List<Item> items = new List<Item> ();
	public int vCount;
	public int hCount;
	Vector2 latestItemPositionH = new Vector2 (-1, -1);
	Vector2 latestItemPositionV = new Vector2 (-1, -1);
	public ItemsTypes nextType;

	public Item AddingItem {
		get {
			return addingItem;
		}

		set {
			addingItem = value;
			if (CompareColumns (addingItem)) {
				if (latestItemPositionH.y != addingItem.square.row && latestItemPositionH.y > -1)
					hCount = 0;
				hCount++;
				latestItemPositionH = new Vector2 (addingItem.square.col, addingItem.square.row);

			} else if (CompareRows (addingItem)) {
				if (latestItemPositionV.x != addingItem.square.col && latestItemPositionV.x > -1)
					vCount = 0;
				vCount++;
				latestItemPositionV = new Vector2 (addingItem.square.col, addingItem.square.row);

			}
			if (hCount > 0 && vCount == 0) {
				vCount = 1;
			}
			items.Add (addingItem);
			//Debug.Log(" c: " + addingItem.square.col + " r: " + addingItem.square.row + " h: " + hCount + " v: " + vCount + " color: " + addingItem.color + " code: " + GetHashCode());
		}

	}

	bool CompareRows (Item item)
	{
		if (items.Count > 0) {
			if (item.square.row > PreviousItem ().square.row)
				return true;
		} else
			return true;

		return false;
	}

	bool CompareColumns (Item item)
	{
		if (items.Count > 0) {
			if (item.square.col > PreviousItem ().square.col)
				return true;
		} else
			return true;

		return false;
	}


	Item PreviousItem ()
	{
		return items [items.Count - 1];
	}
}
