using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class UIButtonColorChange : MonoBehaviour, IFocusable, IInputHandler {

    private Renderer rend;
    private Color originalColor;
    public Color focusedColor;
    public Color clickedColor;

    public void OnFocusEnter()
    {
        // change material color directly
        Material mat2 = rend.material;
        mat2.color = focusedColor;
    }

    public void OnFocusExit()
    {
        Material mat2 = rend.material;
        mat2.color = originalColor;
    }

    void Start () {

    }

    void Update () {
		
	}

    void OnEnable()
    {
        rend = this.transform.Find("RestartButton").gameObject.GetComponent<Renderer>();
        originalColor = rend.material.color;
        Material mat2 = rend.material;
        mat2.color = originalColor;
    }

    public void OnInputDown(InputEventData eventData)
    {
        Material mat2 = rend.material;
        mat2.color = clickedColor;
    }

    public void OnInputUp(InputEventData eventData)
    {

    }
}
