using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class TurnSystem : SerializedMonoBehaviour
{
    private List<GameObject> charactersOnSceneList;

    // Event To raise when is new turn
    [SerializeField]
    private GameEvent newTurnEvent;


    [SerializeField, Tooltip("Which prioperty use to decite order in queue")]
    private CharacterInfoType characterInfoTypeSortBy;

    // count how many turn passed
    private int _turnCounter;

    private List<GameObject> turnQueue;
    //private int characterID;

    private GameObject currentActiveCharacter;

    //public delegate void OnNextMove(GameObject gameObject);
    //public event OnNextMove onNextMove;

    public Scriptablefloat copyToscripable;

    public GameObject CurrentActive
    {
        get
        {
            return currentActiveCharacter;
        }
    }

    private void Awake()
    {
        charactersOnSceneList = new List<GameObject>();
        turnQueue = new List<GameObject>();
    }

	
	// Update is called once per frame
	void Update ()
    {
        NextMove();
	}

    public void AddToSystem(GameObject gameObject)
    {
        if (!charactersOnSceneList.Contains(gameObject))
        {
            charactersOnSceneList.Add( gameObject);
        }
        else
        {
            Debug.LogError("This " + gameObject.ToString() + " is already in system. Check if TurnSystemAgent isn't duplicated for this GameObject");
        }
    }

    public void RemoveFromSystem(GameObject gameObject)
    {
        if (charactersOnSceneList.Contains(gameObject))
        {
            charactersOnSceneList.Remove(gameObject);

            LogOnConsole("Removed gameobject from charactersOnSceneList");

            if (turnQueue.Contains(gameObject))
            {
                turnQueue.Remove(gameObject);

                LogOnConsole("Removed gameobject from turnQueue");
            }
        }
        else
        {
            Debug.LogError("Tryied to remove gameObject from list but this gameobject is not present.");
        }
    }

    private void LogOnConsole(string message)
    {
        Debug.Log(message);
    }

    private void prepareNextTurn()
    {
        turnQueue.AddRange(charactersOnSceneList);

        SortTurnQueueBySpeed();
    }

    private void NextMove()
    {
        if (turnQueue.Count != 0)
        {
            if (currentActiveCharacter == null)
            {
                currentActiveCharacter = turnQueue[0];

                newTurnEvent.Raise();

                //currentActiveCharacter.GetComponent<TurnSystemAgent>().ActiveTarget(currentActiveCharacter);
            }
        }
        else
        {
            prepareNextTurn();
            RaiseTurnValue();
        }
    }

    /// <summary>
    /// Remove active game object from queue.
    /// </summary>
    /// <param name="gameObject"></param>
    public void MoveDone(GameObject gameObject)
    {
        if( IsThisCurrentActiveObject(gameObject))
        {
            currentActiveCharacter = null;

            turnQueue.RemoveAt(0);
        }
    }

    private bool IsThisCurrentActiveObject(GameObject gameObject)
    {
        if (currentActiveCharacter != gameObject)
        {
            Debug.LogWarning(gameObject.ToString() + "is not a object which have turn now.");
            return false;
        }

        return true;
    }

    private void SortTurnQueueBySpeed()
    {
        GameObject temp;

        for (int i = turnQueue.Count - 1; i > 0; i--)
        {
            for (int j = 0; j < i; j++)
            {
                float valueA = turnQueue[j].GetComponent<CharacterInfo>().GetCharacterInfo(characterInfoTypeSortBy);
                float valueB = turnQueue[j + 1].GetComponent<CharacterInfo>().GetCharacterInfo(characterInfoTypeSortBy);

                if (valueA < valueB)
                {
                    temp = turnQueue[j];
                    turnQueue[j] = turnQueue[j + 1];
                    turnQueue[j + 1] = temp;
                }
            }
        }
    }

    private void RaiseTurnValue()
    {
        _turnCounter++;
        Debug.Log(_turnCounter);
        if (copyToscripable != null)
        {
            copyToscripable.value = _turnCounter;
        }
    }
}
