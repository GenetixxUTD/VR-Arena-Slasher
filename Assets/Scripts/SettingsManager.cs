using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class SettingsManager : MonoBehaviour
{
    public TMP_Text movementTypeText;
    public TMP_Text rotationTypeText;
    public GameObject snapAngle;
    public TMP_Text snapAngleText;

    public void onMovementTypeSlider(float value)
    {
        switch (value)
        {
            case 0:
                movementTypeText.text = "Continuous";
                PlayerPrefs.SetInt("movementtype", 0);
                break;
            case 1:
                movementTypeText.text = "Teleport";
                PlayerPrefs.SetInt("movementtype", 1);
                break;
        }
    }

    public void onRotationTypeSlider(float value)
    {
        switch (value)
        {
            case 0:
                rotationTypeText.text = "Snap Turn";
                snapAngle.SetActive(true);
                PlayerPrefs.SetInt("rotationtype", 0);
                break;
            case 1:
                rotationTypeText.text = "Smooth Turn";
                snapAngle.SetActive(false);
                PlayerPrefs.SetInt("rotationtype", 1);
                break;
        }
    }

    public void onRotationValueSlider(float value)
    {
        PlayerPrefs.SetInt("snapangle", (int)value);
        snapAngleText.text = "Snap Angle: " + value;
    }

}
