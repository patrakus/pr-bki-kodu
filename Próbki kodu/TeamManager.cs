using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    /// <summary>
    /// Team 0 -> Index 0 //player
    /// Team 1 -> Index 1 //enemy
    /// </summary>
    private List<GameObject>[] teamsListArray;

    // index for team members
    private int indexForTeamZero;
    private int indexForTeamOne;

    public int CurrentActive(GameObject gameObject)
    {
        if (teamsListArray[0].Contains(gameObject))
        {
            return 0;
        }
        else if (teamsListArray[1].Contains(gameObject))
        {
            return 1;
        }
        //else
        //{
        //    Debug.LogError("Something is wrong.");
        //    
        //}

        return -1;
    }

    /// <summary>
    /// This property check if indexForTeamZero is greater than actual team count or less. If so then indexForTeamZero will have set proper value not
    /// exceeding scope of team count.
    /// </summary>
    private int TeamZero
    {
        get
        {
            return indexForTeamZero;
        }

        set
        {
            if (value >= teamsListArray[0].Count)
            {
                indexForTeamZero = 0;
            }
            else if (value < 0)
            {
                indexForTeamZero = teamsListArray[0].Count - 1;
            }
            else
            {
                indexForTeamZero = value;
            }
        }
    }

    /// <summary>
    /// This property check if indexForTeamOne is greater than actual team count or less. If so then indexForTeamOne will have set proper value not
    /// exceeding scope of team count.
    /// </summary>
    private int TeamOne
    {
        get
        {
            return indexForTeamOne;
        }

        set
        {
            if (value >= teamsListArray[1].Count)
            {
                indexForTeamOne = 0;
            }
            else if (value < 0)
            {
                indexForTeamOne = teamsListArray[1].Count - 1;
            }
            else
            {
                indexForTeamOne = value;
            }
        }
    }

    private void Awake()
    {
        teamsListArray = new List<GameObject>[2];
        for (int i = 0; i < 2; i++)
        {
            teamsListArray[i] = new List<GameObject>();
        }
    }

    public bool IsTeamZeroAlive
    {
        get
        {
            return teamsListArray[0].Count > 0;
        }
    }

    public bool IsTeamOneAlive
    {
        get
        {
            return teamsListArray[1].Count > 0;
        }
    }

    /// <summary>
    /// Add GameObject to corresponding team described by team value
    /// </summary>
    /// <param name="team">Team number</param>
    /// <param name="gameObject">GameObject which works as character in game scene</param>
    public void AddToTeam(int team, GameObject gameObject)
    {
        for (int i = 0; i < teamsListArray.Length; i++)
        {
            if (teamsListArray[i].Contains(gameObject))
            {
                Debug.LogError("This game object is already in team " + i);
                return;
            }
        }

        teamsListArray[team].Add(gameObject);
    }

    /// <summary>
    /// Removes GameObject from corresponding team described by team value
    /// </summary>
    /// <param name="team">Team number</param>
    /// <param name="gameObject">GameObject which is already in team</param>
    public void RemoveFromTeam(int team, GameObject gameObject)
    {
        teamsListArray[team].Remove(gameObject);
    }

    /// <summary>
    /// Return next teammate from given team
    /// </summary>
    /// <param name="team">nr. zespołu z którego ma być zwrócony obiekt</param>
    /// <returns></returns>
    public GameObject GetNextTeamMember(int team)
    {
        if (team == 0)
        {
            return teamsListArray[team][TeamZero++];
        }
        else if (team == 1)
        {
            return teamsListArray[team][TeamOne++];
        }
        else
        {
            Debug.LogError("Value of team is out of scope");
            return null;
        }

        
    }
}
