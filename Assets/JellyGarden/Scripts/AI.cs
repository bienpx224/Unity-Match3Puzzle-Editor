using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CombineType
{
	LShape,
	VShape
}

public class AI : MonoBehaviour
{
	/// <summary>
	/// The reference to this object
	/// </summary>
	public static AI THIS;
	/// <summary>
	/// have got a tip
	/// </summary>
	public bool gotTip;
	/// <summary>
	/// The allow show tip
	/// </summary>
	public bool allowShowTip;
	/// <summary>
	/// The tip identifier
	/// </summary>
	int tipID;
	/// <summary>
	/// The count of coroutines
	/// </summary>
	public int corCount;
	/// <summary>
	/// The tip items
	/// </summary>
	private List<Item> nextMoveItems;
	// Use this for initialization
	void Start()
	{
		THIS = this;
	}

	public Vector3 vDirection;
	public CombineType combineType;
	public Item tipItem;

	/// <summary>
	/// Gets the square. Return square by row and column
	/// </summary>
	/// <param name="row">The row.</param>
	/// <param name="col">The column.</param>
	/// <returns></returns>
	Square GetSquare(int row, int col)
	{
		return LevelManager.THIS.GetSquare(col, row);
	}

	/// <summary>
	/// Checks the square. Is the color of item of this square is equal to desired color. If so we add the item to nextMoveItems array.
	/// </summary>
	/// <param name="square">The square.</param>
	/// <param name="COLOR">The color.</param>
	/// <param name="moveThis">is the item should be movable?</param>
	void CheckSquare(Square square, int COLOR, bool moveThis = false)
	{
		if (square == null)
			return;
		if (square.item != null)
		{
			if (square.item.color == COLOR)
			{
				if (moveThis && square.type != SquareTypes.WIREBLOCK)
				{
					nextMoveItems.Add(square.item);
				}
				else if (!moveThis)
					nextMoveItems.Add(square.item);
			}
		}

	}

	public List<Item> GetCombine()
	{
		return nextMoveItems;
	}

	/// <summary>
	/// Loop of searching possible combines
	/// </summary>
	/// <returns></returns>
	public IEnumerator CheckPossibleCombines()
	{
		//waiting for 1 second just in case to be sure that field was built
		yield return new WaitForSeconds(1);

		//allow to show tips
		allowShowTip = true;

		//get max positions of squares
		int maxRow = LevelManager.THIS.maxRows;
		int maxCol = LevelManager.THIS.maxCols;

		//variable to check: are we got tip or not
		gotTip = false;

		//break, if the main scripts have not ready yet
		while (LevelManager.THIS == null)
		{
			yield return new WaitForEndOfFrame();
		}
		//if game is not in Playing status - wait
		while (LevelManager.THIS.gameStatus != GameState.Playing)
		{
			yield return new WaitForEndOfFrame();
		}

		//if drag have not blocked and game status Playing - continue
		if (!LevelManager.THIS.DragBlocked && LevelManager.THIS.gameStatus == GameState.Playing)
		{
			nextMoveItems = new List<Item>();

			if (LevelManager.THIS.gameStatus != GameState.Playing)
				yield break;


			Item it = GameObject.FindGameObjectWithTag("Item").GetComponent<Item>();

			//Iteration for search possible combination 
			for (int COLOR = 0; COLOR < it.items.Length; COLOR++)
			{
				for (int col = 0; col < LevelManager.THIS.maxCols; col++)
				{
					for (int row = 0; row < LevelManager.THIS.maxRows; row++)
					{
						Square square = LevelManager.THIS.GetSquare(col, row);
						if (square.type == SquareTypes.WIREBLOCK || square.item == null)
							continue;
						//current square called x
						//o-o-x
						//	  o
						vDirection = Vector3.zero;
						combineType = CombineType.LShape;
						if (col > 1 && row < maxRow - 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							showTip(nextMoveItems);
							tipItem = nextMoveItems[0];
							vDirection = Vector3.up;
							yield break;
						}
						else
							nextMoveItems.Clear();

						//    o
						//o-o x
						if (col > 1 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//x o o
						//o
						if (col < maxCol - 2 && row < maxRow - 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							vDirection = Vector3.up;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//o
						//x o o
						if (col < maxCol - 2 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//o
						//o
						//x o
						if (col < maxCol - 1 && row > 1)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							// StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//x o
						//o
						//o
						if (col < maxCol - 1 && row < maxRow - 2)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//	o
						//  o
						//o x
						if (col > 0 && row > 1)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//o x
						//  o
						//  o
						if (col > 0 && row < maxRow - 2)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//o-x-o-o
						if (col < maxCol - 2 && col > 0)
						{
							CheckSquare(GetSquare(row, col - 1), COLOR, true);
							CheckSquare(GetSquare(row, col + 1), COLOR);
							CheckSquare(GetSquare(row, col + 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.right));
							vDirection = Vector3.right;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();
						//o-o-x-o
						if (col < maxCol - 1 && col > 1)
						{
							CheckSquare(GetSquare(row, col + 1), COLOR, true);
							CheckSquare(GetSquare(row, col - 1), COLOR);
							CheckSquare(GetSquare(row, col - 2), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.left));
							vDirection = Vector3.left;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();
						//o
						//x
						//o
						//o
						if (row < maxRow - 2 && row > 0)
						{
							CheckSquare(GetSquare(row - 1, col), COLOR, true);
							CheckSquare(GetSquare(row + 1, col), COLOR);
							CheckSquare(GetSquare(row + 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//  StartCoroutine(showTip(nextMoveItems[0], Vector3.down));
							vDirection = Vector3.down;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();

						//o
						//o
						//x
						//o
						if (row < maxRow - 2 && row > 1)
						{
							CheckSquare(GetSquare(row + 1, col), COLOR, true);
							CheckSquare(GetSquare(row - 1, col), COLOR);
							CheckSquare(GetSquare(row - 2, col), COLOR);
						}
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto())
						{
							//   StartCoroutine(showTip(nextMoveItems[0], Vector3.up));
							vDirection = Vector3.up;
							tipItem = nextMoveItems[0];
							showTip(nextMoveItems);
							yield break;
						}
						else
							nextMoveItems.Clear();
						//  o
						//o x o
						//  o
						int h = 0;
						int v = 0;
						combineType = CombineType.VShape;

						if (row < maxRow - 1)
						{
							square = GetSquare(row + 1, col);
							if (square)
							{//1.6
								if (square.item != null)
								{
									if (square.item.color == COLOR)
									{
										vDirection = Vector3.up;
										nextMoveItems.Add(square.item);
										v++;
									}
								}
							}
						}
						if (row > 0)
						{
							square = GetSquare(row - 1, col);
							if (square)
							{//1.6
								if (square.item != null)
								{
									if (square.item.color == COLOR)
									{
										vDirection = Vector3.down;
										nextMoveItems.Add(square.item);
										v++;
									}
								}
							}
						}
						if (col > 0)
						{
							square = GetSquare(row, col - 1);
							if (square)
							{//1.6
								if (square.item != null)
								{
									if (square.item.color == COLOR)
									{
										vDirection = Vector3.right;
										nextMoveItems.Add(square.item);
										h++;
									}
								}
							}
						}
						if (col < maxCol - 1)
						{
							square = GetSquare(row, col + 1);
							if (square)
							{//1.6
								if (square.item != null)
								{
									if (square.item.color == COLOR)
									{
										vDirection = Vector3.left;
										nextMoveItems.Add(square.item);
										h++;
									}
								}
							}
						}

						//if we found 3or more items and they not lock show tip
						if (nextMoveItems.Count == 3 && GetSquare(row, col).CanGoInto() && GetSquare(row, col).type != SquareTypes.WIREBLOCK)
						{
							if (v > h && nextMoveItems[2].square.type != SquareTypes.WIREBLOCK)
							{ //StartCoroutine(showTip(nextMoveItems[2], new Vector3(Random.Range(-1f, 1f), 0, 0)));
								tipItem = nextMoveItems[2];
								if (tipItem.transform.position.x > nextMoveItems[0].transform.position.x)
									vDirection = Vector3.left;
								else
									vDirection = Vector3.right;
								showTip(nextMoveItems);
								yield break;

							}
							else if (v < h && nextMoveItems[0].square.type != SquareTypes.WIREBLOCK)
							{ // StartCoroutine(showTip(nextMoveItems[0], new Vector3(0, Random.Range(-1f, 1f), 0)));
								tipItem = nextMoveItems[0];
								if (tipItem.transform.position.y > nextMoveItems[0].transform.position.y)
									vDirection = Vector3.down;
								else
									vDirection = Vector3.up;

								showTip(nextMoveItems);
								yield break;

							}
							else
								nextMoveItems.Clear();
						}
						else
							nextMoveItems.Clear();

					}
				}


			}
			//if we don't get any tip.  call nomatches to regenerate level
			if (!LevelManager.THIS.DragBlocked)
			{
				if (!gotTip)
					LevelManager.THIS.NoMatches();
			}

		}
		yield return new WaitForEndOfFrame();
		//find possible combination again 
		if (!LevelManager.THIS.DragBlocked)
			StartCoroutine(CheckPossibleCombines());

		// }
	}

	//show tip function calls coroutine for
	void showTip(List<Item> nextMoveItems)
	{
		//        print("show tip");
		StopCoroutine(showTipCor(nextMoveItems));
		StartCoroutine(showTipCor(nextMoveItems));
	}

	//show tip coroutine
	IEnumerator showTipCor(List<Item> nextMoveItems)
	{
		gotTip = true;
		corCount++;
		if (corCount > 1)
		{
			corCount--;
			yield break;
		}
		if (LevelManager.THIS.DragBlocked && !allowShowTip)
		{
			corCount--;
			yield break;
		}
		tipID = LevelManager.THIS.moveID;
		//while (!LevelManager.THIS.DragBlocked && allowShowTip)
		//{
		yield return new WaitForSeconds(1);
		if (LevelManager.THIS.DragBlocked && !allowShowTip && tipID != LevelManager.THIS.moveID)
		{
			corCount--;
			yield break;
		}
		foreach (Item item in nextMoveItems)
		{
			if (item == null)
			{
				corCount--;
				yield break;
			}

		}
		//call animation trigger for every found item to show tip
		foreach (Item item in nextMoveItems)
		{
			if (item != null)
				item.anim.SetTrigger("tip");
		}
		yield return new WaitForSeconds(0);
		StartCoroutine(CheckPossibleCombines());
		corCount--;
		// }
	}


}
