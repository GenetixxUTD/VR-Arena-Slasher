using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public enum Quadrant
    {
        Top,
        Left,
        Right,
        Bottom
    }

    private int health;

    private bool[] occupiedQuadrants = { false, false, false, false }; //0= top, 1= left, 2= down, 3= right

    public Quadrant quadrantResult;

    private Camera mainCamera;
    public GameObject playerSword;

    private void Start()
    {
        mainCamera = Camera.main;
        health = 10;
    }

    public int getSwordQuadrant()
    {
        Vector3 dirToSword = playerSword.transform.position - mainCamera.transform.position;
        float angle = Vector3.SignedAngle(mainCamera.transform.position, playerSword.transform.position, mainCamera.transform.up);

        if (angle <= 45 && angle >= -45)
        {
            return 0;
        }
        else if(angle > 45 && angle <= 135)
        {
            return 3;
        }
        else if(angle < -45 && angle >= -135)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

    public bool checkQuadrantOccupancy(int quadrant)
    {
        return occupiedQuadrants[quadrant];
    }

    public void occupyQuadrant(int quadrant)
    {
        occupiedQuadrants[(int)quadrant] = true;
    }

    public void leaveQuadrant(int quadrant)
    {
        occupiedQuadrants[((int)quadrant)] = false;
    }

    public void modifyHealth(int modify)
    {
        health += modify;
    }
}
