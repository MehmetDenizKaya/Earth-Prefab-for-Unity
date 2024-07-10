using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Script : MonoBehaviour
{
    
    Animator m_Animator;
    public GameObject world;
    float m_MySliderValue;

    void Start()
    {
        //Get the animator, attached to the GameObject you are intending to animate.
        m_Animator = world.GetComponent<Animator>();
    }

    void OnGUI()
    {
        //Create a Label in Game view for the Slider
        GUI.Label(new Rect(0, 25, 40, 60), "Speed");
        //Create a horizontal Slider to control the speed of the Animator. Drag the slider to 1 for normal speed.

        m_MySliderValue = GUI.HorizontalSlider(new Rect(45, 25, 200, 60), m_MySliderValue, 0.0F, 1.0F);
        m_Animator.speed = m_MySliderValue;
    }
    
}