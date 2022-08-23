using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//types of items
public enum ItemsTypes
{
    NONE = 0,
    VERTICAL_STRIPPED,
    HORIZONTAL_STRIPPED,
    PACKAGE,
    BOMB,
    INGREDIENT
}

public class Item : MonoBehaviour
{
    //items sprites array
    public Sprite[] items;
    public GameObject[] prefabs;
    //stripe extra items array
    public List<StripedItem> stripedItems = new List<StripedItem>();
    //arrays for extra items
    public Sprite[] packageItems;
    public Sprite[] bombItems;
    public Sprite[] ingredientItems;

    //sprite rendered reference
    public SpriteRenderer sprRenderer;
    //square object reference
    public Square square;
    //is that item dragging
    public bool dragThis;

    public Vector3 mousePos;
    public Vector3 deltaPos;
    public Vector3 switchDirection;
    private Square neighborSquare;
    private Item switchItem;
    public bool falling;
    private ItemsTypes nextType = ItemsTypes.NONE;
    public ItemsTypes currentType = ItemsTypes.NONE;
    public ItemsTypes debugType = ItemsTypes.NONE;
    public int COLORView;
    private int COLOR;

    public int color
    {
        get
        {
            return COLOR;
        }
        set
        {
            COLOR = value;
        }
    }

    public ItemsTypes NextType
    {
        get
        {
            return nextType;
        }

        set
        {
            //      print(value);
            nextType = value;
        }
    }

    public Animator anim;
    public bool destroying;
    public bool appeared;
    public bool animationFinished;
    public bool justCreatedItem;
    private float xScale;
    private float yScale;
    public bool boost;
    public Vector2 moveDirection;
    //1.6
    // Use this for initialization
    void Start()
    {
        gameObject.name = "item " + GetInstanceID();
        falling = true;
        GenColor();
        // sprRenderer = GetComponentInChildren<SpriteRenderer>();
        if (NextType != ItemsTypes.NONE)
        {
            debugType = NextType;
            currentType = NextType;
            NextType = ItemsTypes.NONE;
            transform.position = square.transform.position;
            falling = false;
        }
        else if (LevelManager.THIS.limitType == LIMIT.TIME && Random.Range(0, 28) == 1)
        {
            GameObject fiveTimes = Instantiate(Resources.Load("Prefabs/5sec")) as GameObject;
            fiveTimes.transform.SetParent(transform);
            fiveTimes.name = "5sec";
            fiveTimes.transform.localScale = Vector3.one * 2;
            fiveTimes.transform.localPosition = Vector3.zero;
        }
        xScale = transform.localScale.x;
        yScale = transform.localScale.y;

        //StartCoroutine(GenRandomSprite());
    }

    public void GenColor(int exceptColor = -1, bool onlyNONEType = false)
    {
        int row = square.row;
        int col = square.col;

        List<int> remainColors = new List<int>();
        for (int i = 0; i < LevelManager.Instance.colorLimit; i++)
        {
            bool canGen = true;
            if (col > 1)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row, col - 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoInto() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
            if (col < LevelManager.Instance.maxCols - 1)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row, col + 1);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
            if (row < LevelManager.Instance.maxRows)
            {
                Square neighbor = LevelManager.Instance.GetSquare(row + 1, col);
                if (neighbor != null)
                {
                    if (neighbor.item != null)
                    {
                        if (neighbor.CanGoOut() && neighbor.item.color == i)
                            canGen = false;
                    }
                }
            }
            if (canGen && i != exceptColor)
            {
                remainColors.Add(i);
            }
        }

        //       print(remainColors.Count);
        int randColor = Random.Range(0, LevelManager.Instance.colorLimit);
        if (remainColors.Count > 0)
            randColor = remainColors[Random.Range(0, remainColors.Count)];
        if (exceptColor == randColor)
            randColor = (randColor++) % items.Length;
        LevelManager.THIS.lastRandColor = randColor;
        sprRenderer.sprite = items[randColor];
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
            sprRenderer.sprite = stripedItems[color].horizontal;
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
            sprRenderer.sprite = stripedItems[color].vertical;
        else if (NextType == ItemsTypes.PACKAGE)
            sprRenderer.sprite = packageItems[color];
        else if (NextType == ItemsTypes.BOMB)
            sprRenderer.sprite = bombItems[0];
        else
        {
            var ingredients = LevelManager.THIS.targetObject.Where(i=>i.type == Target.INGREDIENT).ToArray();
            if (ingredients.Any())
            {
                foreach (var targetObject in ingredients)
                {
                    if( Random.Range(0, LevelManager.THIS.Limit) == 0 && square.row + 1 < LevelManager.THIS.maxRows && !onlyNONEType
                        && !targetObject.Done() && LevelManager.THIS.GetIngredientsCount(targetObject.icon.name) < targetObject.GetCount())
                    {
                        StartCoroutine(FallingCor(square, true));
                        color = 1000;
                        currentType = ItemsTypes.INGREDIENT;
                        sprRenderer.sprite = targetObject.icon;
                        name = "ingredient_" + color;
                    }
                    else
                    {
                        StartCoroutine(FallingCor(square, true));
                        color = Array.IndexOf(items, sprRenderer.sprite);
                    }
                }
            }
            else
            {
                StartCoroutine(FallingCor(square, true));
                color = Array.IndexOf(items, sprRenderer.sprite);
            }
        }

        if (prefabs.Length > 0)
        {
            sprRenderer.enabled = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != "Sprite")
                { Destroy(transform.GetChild(i)); }
            }
            GameObject obj = Instantiate(prefabs[color]);
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
        }

    }

    public void SetColor(int col)
    {
        color = col;
        if (color < items.Length)
            sprRenderer.sprite = items[color];
    }

    public void SetAppeared()
    {
        appeared = true;
        StartIdleAnim();
        if (currentType == ItemsTypes.PACKAGE)
            anim.SetBool("package_idle", true);

    }

    public void StartIdleAnim()
    {
        StartCoroutine(AnimIdleStart());

    }

    IEnumerator AnimIdleStart()
    {
        float xScaleDest1 = xScale - 0.05f;
        float xScaleDest2 = xScale;
        float speed = Random.Range(0.02f, 0.07f);

        bool trigger = false;
        while (true)
        {
            if (!trigger)
            {
                if (xScale > xScaleDest1)
                {
                    xScale -= Time.deltaTime * speed;
                    yScale += Time.deltaTime * speed;
                }
                else
                    trigger = true;
            }
            else
            {
                if (xScale < xScaleDest2)
                {
                    xScale += Time.deltaTime * speed;
                    yScale -= Time.deltaTime * speed;
                }
                else
                    trigger = false;
            }
            transform.localScale = new Vector3(xScale, yScale, 1);
            yield return new WaitForFixedUpdate();
        }
        //anim.SetBool("stop", true);
        //if (square.col % 2 == 0 || square.row % 2 == 0)
        //    yield return new WaitForSeconds(1);
        //anim.SetBool("stop", false);
    }

    IEnumerator GenRandomSprite()
    {
        Sprite spr = null;
        while (true)
        {
            spr = items[Random.Range(0, items.Length)];
            yield return new WaitForFixedUpdate();
            break;
        }

        sprRenderer.sprite = spr;
    }

    void ResetDrag()
    {
        dragThis = false;
        //   transform.position = square.transform.position + Vector3.back * 0.2f;
        switchDirection = Vector3.zero;
        //    switchItem.transform.position = neighborSquare.transform.position + Vector3.back * 0.2f;
        neighborSquare = null;
        switchItem = null;

    }

    void Update()
    {
        COLORView = color;
        if (currentType != debugType && currentType != ItemsTypes.INGREDIENT)
        {
            NextType = debugType;
            ChangeType();

        }
        if (dragThis)
        {
            deltaPos = mousePos - GetMousePosition();
            if (switchDirection == Vector3.zero)
            {
                SwitchDirection(deltaPos);
            }
        }
    }

    public void SwitchDirection(Vector3 delta)
    {
        deltaPos = delta;
        if (Vector3.Magnitude(deltaPos) > 0.1f)
        {
            if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x > 0)
                switchDirection.x = 1;
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && deltaPos.x < 0)
                switchDirection.x = -1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y > 0)
                switchDirection.y = 1;
            else if (Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && deltaPos.y < 0)
                switchDirection.y = -1;
            if (switchDirection.x > 0)
            {
                neighborSquare = square.GetNeighborLeft();
            }
            else if (switchDirection.x < 0)
            {
                neighborSquare = square.GetNeighborRight();
            }
            else if (switchDirection.y > 0)
            {
                neighborSquare = square.GetNeighborBottom();
            }
            else if (switchDirection.y < 0)
            {
                neighborSquare = square.GetNeighborTop();
            }
            if (neighborSquare != null)
                switchItem = neighborSquare.item;
            if (switchItem != null)
            {
                if (switchItem.square.type != SquareTypes.WIREBLOCK)
                    StartCoroutine(Switching());
                else if ((currentType != ItemsTypes.NONE || switchItem.currentType != ItemsTypes.NONE) && switchItem.square.type != SquareTypes.WIREBLOCK)   //1.4.1   1.6.1
                    StartCoroutine(Switching());
                else
                    ResetDrag();//1.6.1

            }
            else
                ResetDrag();
        }

    }

    IEnumerator Switching()
    {
        if (switchDirection != Vector3.zero && neighborSquare != null)
        {
            bool backMove = false;
            LevelManager.THIS.DragBlocked = true;
            neighborSquare.item = this;
            square.item = switchItem;
            int matchesHere = neighborSquare.FindMatchesAround().Count;
            int matchesInThisItem = matchesHere;
            int matchesInNeithborItem = square.FindMatchesAround().Count;
            bool thisItemHaveMatch = false;
            if (matchesInThisItem >= 4)
                thisItemHaveMatch = true;
            if (matchesInNeithborItem >= 4)
                thisItemHaveMatch = false;
            int matchesHereOneColor = matchesHere;
            matchesHere += matchesInNeithborItem;

            if ((currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT))
                matchesHere++;
            if (currentType > 0 && switchItem.currentType > 0)
                matchesHere++;
            if (currentType == ItemsTypes.INGREDIENT && switchItem.currentType == ItemsTypes.INGREDIENT)
                matchesHere = 0;
            float startTime = Time.time;
            Vector3 startPos = transform.position;

            float speed = 5;
            float distCovered = 0;
            while (distCovered < 1)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.3f, distCovered);
                switchItem.transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.2f, startPos, distCovered);
                yield return new WaitForFixedUpdate();
            }
            if (matchesHere <= 0 && matchesInNeithborItem <= 0 && LevelManager.THIS.ActivatedBoost.type != BoostType.Hand ||
                ((currentType == ItemsTypes.BOMB || switchItem.currentType == ItemsTypes.BOMB) && (currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)) ||
                (((int)currentType >= 1 || (int)switchItem.currentType >= 1) && (currentType == ItemsTypes.INGREDIENT || switchItem.currentType == ItemsTypes.INGREDIENT) && (matchesHere + matchesInNeithborItem <= 2)))
            {//1.6.1

                    neighborSquare.item = switchItem;
                    square.item = this;
                    backMove = true;
                    SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.wrongMatch);


            }
            else
            {
                if (LevelManager.THIS.ActivatedBoost.type != BoostType.Hand)
                {
                    if (LevelManager.Instance.limitType == LIMIT.MOVES)
                        LevelManager.THIS.Limit--;
                    LevelManager.THIS.moveID++;
                }
                if (LevelManager.THIS.ActivatedBoost.type == BoostType.Hand)
                    LevelManager.THIS.ActivatedBoost = null;
                switchItem.square = square;
                square = neighborSquare;
                LevelManager.THIS.lastDraggedItem = this;
                LevelManager.THIS.lastSwitchedItem = switchItem;

                if (matchesHereOneColor == 4 || matchesInNeithborItem == 4)
                {
                    if (thisItemHaveMatch)
                        SetStrippedExtra(startPos - neighborSquare.transform.position);
                    else
                    {
                        LevelManager.THIS.lastDraggedItem = switchItem;
                        LevelManager.THIS.lastSwitchedItem = this;
                        switchItem.SetStrippedExtra(startPos - square.transform.position);
                        if (matchesInThisItem == 4)
                            SetStrippedExtra(startPos - neighborSquare.transform.position);
                    }
                }

                if (matchesHere >= 5)
                {
                    if (thisItemHaveMatch && matchesHereOneColor >= 5)
                        NextType = ItemsTypes.BOMB;
                    else if (!thisItemHaveMatch && matchesInNeithborItem >= 5)
                    {
                        LevelManager.THIS.lastDraggedItem = switchItem;
                        LevelManager.THIS.lastSwitchedItem = this;
                        switchItem.NextType = ItemsTypes.BOMB;
                        if (matchesInThisItem >= 5)
                            NextType = ItemsTypes.BOMB;
                    }

                }
                if (currentType != ItemsTypes.INGREDIENT && switchItem.currentType != ItemsTypes.INGREDIENT)
                {
                    CheckChocoBomb(this, switchItem);
                    if (currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                        CheckChocoBomb(switchItem, this);
                }

                if ((currentType == ItemsTypes.HORIZONTAL_STRIPPED || currentType == ItemsTypes.VERTICAL_STRIPPED) && (switchItem.currentType == ItemsTypes.HORIZONTAL_STRIPPED || switchItem.currentType == ItemsTypes.VERTICAL_STRIPPED))
                {
                    //1.6
                    switchItem.currentType = ItemsTypes.NONE; //2.0
                    switchItem.nextType = ItemsTypes.NONE;  //2.0
                    switchItem.debugType = ItemsTypes.NONE; //2.0
                    DestroyHorizontal();
                    DestroyVertical();
                    //					switchItem.DestroyItem ();
                }

                CheckPackageAndStripped(this, switchItem);
                CheckPackageAndStripped(switchItem, this);


                CheckPackageAndPackage(this, switchItem);
                CheckPackageAndPackage(switchItem, this);

                if (currentType != ItemsTypes.BOMB || switchItem.currentType != ItemsTypes.BOMB)
                    LevelManager.THIS.FindMatches();
            }
            //Debug.Break();
            //yield return new WaitForSeconds(2);


            startTime = Time.time;
            distCovered = 0;
            while (distCovered < 1 && backMove)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.position = Vector3.Lerp(neighborSquare.transform.position + Vector3.back * 0.3f, startPos, distCovered);
                switchItem.transform.position = Vector3.Lerp(startPos, neighborSquare.transform.position + Vector3.back * 0.2f, distCovered);
                yield return new WaitForFixedUpdate();
            }

            if (backMove)
            {
                LevelManager.THIS.DragBlocked = false;
                StartCoroutine(AI.THIS.CheckPossibleCombines());//2.1.5 prevents early move
            }
        }
        ResetDrag();
    }

    void CheckPackageAndPackage(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.PACKAGE && item2.currentType == ItemsTypes.PACKAGE)
        {
            int i = 0;
            List<Item> bigList = new List<Item>();
            List<Item> itemsList = LevelManager.THIS.GetItemsAround(item2.square);
            foreach (Item item in itemsList)
            {
                if (item != null)
                {

                    bigList.AddRange(LevelManager.THIS.GetItemsAround(item.square));
                }
            }
            foreach (Item item in bigList)
            {
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
            }

            item1.DestroyPackage();
            item2.DestroyPackage();
        }
    }


    void CheckPackageAndStripped(Item item1, Item item2)
    {
        if (((item1.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item1.currentType == ItemsTypes.VERTICAL_STRIPPED) && item2.currentType == ItemsTypes.PACKAGE))
        {
            int i = 0;
            List<Item> itemsList = LevelManager.THIS.GetItemsAround(item2.square);
            int direction = 1;//2.0
            foreach (Item item in itemsList)
            {
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    {//1.6.1
                        if (direction > 0)//2.0
                            item.currentType = ItemsTypes.HORIZONTAL_STRIPPED;
                        else
                            item.currentType = ItemsTypes.VERTICAL_STRIPPED;
                        direction *= -1;
                        //						item.currentType = (ItemsTypes)((i) % 2) + 1;
                        i++;
                    }
                }
            }
            item2.DestroyPackage();
        }
    }

    public void CheckChocoBomb(Item item1, Item item2)
    {
        if (item1.currentType == ItemsTypes.INGREDIENT || item2.currentType == ItemsTypes.INGREDIENT)
            return;
        if (item1.currentType == ItemsTypes.BOMB)
        {
            if (item2.currentType == ItemsTypes.NONE)
                DestroyColor(item2.color);
            else if (item2.currentType == ItemsTypes.HORIZONTAL_STRIPPED || item2.currentType == ItemsTypes.VERTICAL_STRIPPED)
                LevelManager.THIS.SetTypeByColor(item2.color, ItemsTypes.HORIZONTAL_STRIPPED);
            else if (item2.currentType == ItemsTypes.PACKAGE)
                LevelManager.THIS.SetTypeByColor(item2.color, ItemsTypes.PACKAGE);
            else if (item2.currentType == ItemsTypes.BOMB)
                LevelManager.THIS.DestroyDoubleBomb(square.col);



            item1.DestroyItem();
        }

    }

    void SetStrippedExtra(Vector3 dir)
    {
        moveDirection = dir;//1.6
                            //		print ("set striped");
        if (Math.Abs(dir.x) > Math.Abs(dir.y))
            NextType = ItemsTypes.HORIZONTAL_STRIPPED;
        else
            NextType = ItemsTypes.VERTICAL_STRIPPED;
    }

    Vector3 GetDeltaPositionFromSquare(Square sq, Vector3 delta)
    {
        Vector3 newPos = (sq.transform.position - delta) + Vector3.back * 0.3f;
        newPos.x = Mathf.Clamp(newPos.x, sq.GetNeighborLeft(true).transform.position.x, sq.GetNeighborRight(true).transform.position.x);
        newPos.y = Mathf.Clamp(newPos.y, sq.GetNeighborBottom(true).transform.position.y, sq.GetNeighborTop(true).transform.position.y);
        return newPos;
    }


    public Vector3 GetMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void CheckNeedToFall(Square _square)
    {
        _square.item = this;
        square.item = null;
        square = _square;   //need to count all falling items and drop them down in the same time
    }

    public void StartFalling()
    {
        if (!falling)
            StartCoroutine(FallingCor(square, true));
    }

    IEnumerator FallingCor(Square _square, bool animate)
    {
        falling = true;
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        float speed = 10;
        if (LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
            speed = 10;
        float distance = Vector3.Distance(startPos, _square.transform.position);
        float fracJourney = 0;
        if (distance > 0.5f)
        {
            while (fracJourney < 1)
            {
                speed += 0.2f;
                float distCovered = (Time.time - startTime) * speed;
                fracJourney = distCovered / distance;
                transform.position = Vector3.Lerp(startPos, _square.transform.position + Vector3.back * 0.2f, fracJourney);
                yield return new WaitForFixedUpdate();

            }
        }
        transform.position = _square.transform.position + Vector3.back * 0.2f;
        if (distance > 0.5f && animate)
        {
            anim.SetTrigger("stop");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.drop[Random.Range(0, SoundBase.Instance.drop.Length)]);
        }
        falling = false;
        justCreatedItem = false;
        transform.position = _square.transform.position + Vector3.back * 0.2f;//1.6.1
    }

    public bool GetNearEmptySquares()
    {
        bool nearEmptySquareDetected = false;
        if (square.row < LevelManager.Instance.maxRows - 1 && square.col < LevelManager.Instance.maxCols)
        {
            Square checkingSquare = LevelManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null && !falling)
            {//2.0
                checkingSquare = LevelManager.Instance.GetSquare(square.col + 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartFalling();//2.0
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }
        if (square.row < LevelManager.Instance.maxRows - 1 && square.col > 0)
        {
            Square checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null && !falling)
            {//2.0
                checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartFalling();//2.0
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }

        return nearEmptySquareDetected;
    }

    public bool GetNearEmptySquaresLeft()
    {
        bool nearEmptySquareDetected = false;
        if (square.row < LevelManager.Instance.maxRows - 1 && square.col > 0)
        {
            Square checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
            if (checkingSquare.CanGoInto() && checkingSquare.item == null)
            {
                checkingSquare = LevelManager.Instance.GetSquare(square.col - 1, square.row + 1, true);
                if (checkingSquare.CanGoInto())
                {
                    if (checkingSquare.item == null)
                    {
                        square.item = null;
                        checkingSquare.item = this;
                        square = checkingSquare;
                        StartCoroutine(FallingCor(square, true));
                        nearEmptySquareDetected = true;
                    }
                }
            }
        }
        return nearEmptySquareDetected;
    }


    public Item GetLeftItem()
    {
        Square sq = square.GetNeighborLeft();
        if (sq != null)
        {
            if (sq.item != null)
                return sq.item;
        }
        return null;
    }

    public Item GetTopItem()
    {
        Square sq = square.GetNeighborTop();
        if (sq != null)
        {
            if (sq.item != null)
                return sq.item;
        }
        return null;
    }


    public void ChangeType()
    {
        if (this != null)
            StartCoroutine(ChangeTypeCor());
    }

    IEnumerator ChangeTypeCor()
    {
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
        }
        else if (NextType == ItemsTypes.PACKAGE)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearPackage);

        }
        else if (NextType == ItemsTypes.BOMB)
        {
            anim.SetTrigger("appear");
            SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.appearStipedColorBomb);
            color = 555;
        }
        while (!appeared)
            yield return new WaitForFixedUpdate();

        if (NextType == ItemsTypes.NONE)
            yield break;
        // sprRenderer.enabled = true;
        if (NextType == ItemsTypes.HORIZONTAL_STRIPPED)
            sprRenderer.sprite = stripedItems[color].horizontal;
        else if (NextType == ItemsTypes.VERTICAL_STRIPPED)
            sprRenderer.sprite = stripedItems[color].vertical;
        else if (NextType == ItemsTypes.PACKAGE)
            sprRenderer.sprite = packageItems[color];
        else if (NextType == ItemsTypes.BOMB)
            sprRenderer.sprite = bombItems[0];

        //     square.DestroyBlock();

        debugType = NextType;
        currentType = NextType;
        NextType = ItemsTypes.NONE;

    }

    public void SetAnimationDestroyingFinished()
    {
        LevelManager.THIS.itemsHided = true;
        animationFinished = true;
    }

    #region Destroying

    public void DestroyItem(bool showScore = false, string anim_name = "", bool explEffect = false)
    {

        if (destroying)
            return;
        // if (nextType != ItemsTypes.NONE) return;
        if (this == null)
            return;
        StopCoroutine(AnimIdleStart());
        destroying = true;
        square.item = null;
        GetComponent<BoxCollider2D>().enabled = false;//2.1.5 prevents to early moving

        if (this == null)
            return;

        StartCoroutine(DestroyCor(showScore, anim_name, explEffect));


    }

    IEnumerator DestroyCor(bool showScore = false, string anim_name = "", bool explEffect = false)
    {
        //if (anim_name == "")
        //{
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            PlayDestroyAnimation("destroy");
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            PlayDestroyAnimation("destroy");
        else if (currentType == ItemsTypes.PACKAGE)
        {
            PlayDestroyAnimation("destroy");
            //sprRenderer.enabled = false;
            //GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            //partcl.GetComponent<ItemAnimEvents>().item = this;
            //partcl.transform.localScale = Vector3.one * 0.5f;
            //partcl.GetComponent<Animator>().SetInteger("color", COLOR);
            //partcl.GetComponent<Animator>().SetBool("package", true);
            yield return new WaitForSeconds(0.1f);

            GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/Firework"), transform.position, Quaternion.identity) as GameObject;
            partcl.GetComponent<ParticleSystem>().startColor = LevelManager.THIS.scoresColors[color];
            Destroy(partcl, 1f);
        }
        else if (currentType != ItemsTypes.INGREDIENT && currentType != ItemsTypes.BOMB)
        {
            PlayDestroyAnimation("destroy");
            // GameObject partcl = Instantiate(Resources.Load("Prefabs/Effects/ItemExpl"), transform.position, Quaternion.identity) as GameObject;
            GameObject partcl = LevelManager.THIS.GetExplFromPool();
            if (partcl != null)
            {
                partcl.GetComponent<ItemAnimEvents>().item = this;
                partcl.transform.localScale = Vector3.one * 0.5f;
                partcl.transform.position = transform.position;
                partcl.GetComponent<Animator>().SetInteger("color", color);
                SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroy[Random.Range(0, SoundBase.Instance.destroy.Length)]);
                //   Destroy(partcl, 1f);
            }
            if (explEffect)
            {
                GameObject partcl1 = Instantiate(Resources.Load("Prefabs/Effects/Replace"), transform.position, Quaternion.identity) as GameObject;
                Destroy(partcl1, 1f);

            }
        }
        //}
        //else
        //    PlayDestroyAnimation(anim_name);

        if (LevelManager.THIS.limitType == LIMIT.TIME && transform.Find("5sec") != null)
        {
            GameObject FiveSec = transform.Find("5sec").gameObject;
            FiveSec.transform.SetParent(null);
#if UNITY_5
			FiveSec.GetComponent<Animation>().clip.legacy = true;
#endif

            FiveSec.GetComponent<Animation>().Play("5secfly");
            Destroy(FiveSec, 1);
            if (LevelManager.THIS.gameStatus == GameState.Playing)
                LevelManager.THIS.Limit += 5;
        }

        //Color color = sprRenderer.sprite.texture.GetPixel(sprRenderer.sprite.texture.width / 2 - 10, sprRenderer.sprite.texture.height / 2 - 10);
        if (showScore)
            LevelManager.THIS.PopupScore(LevelManager.THIS.scoreForItem, transform.position, color);

        LevelManager.THIS.CheckCollectedTarget(gameObject);

        while (!animationFinished && currentType == ItemsTypes.NONE)
            yield return new WaitForFixedUpdate();

        square.DestroyBlock();
        if (currentType == ItemsTypes.HORIZONTAL_STRIPPED)
            DestroyHorizontal();
        else if (currentType == ItemsTypes.VERTICAL_STRIPPED)
            DestroyVertical();
        else if (currentType == ItemsTypes.PACKAGE)
            DestroyPackage();
        else if (currentType == ItemsTypes.BOMB && LevelManager.THIS.gameStatus == GameState.PreWinAnimations)
            CheckChocoBomb(this, LevelManager.THIS.GetRandomItems(1)[0]);

        if (NextType != ItemsTypes.NONE)
        {
            Item i = square.GenItem();
            i.NextType = NextType;
            i.SetColor(color);
            i.ChangeType();
        }

        if (destroying)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyHorizontal()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        LevelManager.THIS.StrippedShow(gameObject, true);
        List<Item> itemsList = LevelManager.THIS.GetRow(square.row);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = LevelManager.THIS.GetRowSquaresObstacles(square.row);
        foreach (Square item in sqList)
        {
            if (item != null)
                item.DestroyBlock();
        }
    }

    public void DestroyVertical()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.strippedExplosion);
        LevelManager.THIS.StrippedShow(gameObject, false);
        List<Item> itemsList = LevelManager.THIS.GetColumn(square.col);
        foreach (Item item in itemsList)
        {
            if (item != null)
            {
                if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                    item.DestroyItem(true);
            }
        }
        List<Square> sqList = LevelManager.THIS.GetColumnSquaresObstacles(square.col);
        foreach (Square item in sqList)
        {
            if (item != null)
                item.DestroyBlock();
        }


    }

    public void DestroyPackage()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.destroyPackage);

        List<Item> itemsList = LevelManager.THIS.GetItemsAround(square);
        foreach (Item item in itemsList)
        {
            if (item != null)
                if (item != null)
                {
                    if (item.currentType != ItemsTypes.BOMB && item.currentType != ItemsTypes.INGREDIENT)
                        item.DestroyItem(true, "destroy_package");
                }
        }
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.explosion);
        currentType = ItemsTypes.NONE;
        DestroyItem(true);
    }

    public void DestroyColor(int p)
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.colorBombExpl);

        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<Item>().color == p)
                item.GetComponent<Item>().DestroyItem(true, "", true);
        }
    }

    void PlayDestroyAnimation(string anim_name)
    {
        anim.SetTrigger(anim_name);

    }

    public void SmoothDestroy()
    {
        StartCoroutine(SmoothDestroyCor());
    }

    IEnumerator SmoothDestroyCor()
    {
        square.item = null;
        GetComponent<BoxCollider2D>().enabled = false;//2.1.5 prevents to early moving
        anim.SetTrigger("disAppear");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    #endregion

}

[Serializable]
public class StripedItem
{
    public Sprite horizontal;
    public Sprite vertical;
}
