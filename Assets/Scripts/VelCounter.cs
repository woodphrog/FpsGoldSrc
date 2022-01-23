using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class VelCounter : MonoBehaviour
{
    public GameObject Player;
    public CharacterController cc;
    // Start is called before the first frame update

    private void Start()
    {
        var t = GetComponent<TMP_Text>();
        t.SetText("x: "+cc.velocity.x+ "\ny: " + cc.velocity.y + "\nz: "+cc.velocity.z +"\nmagnitude: "+cc.velocity.magnitude+"\n");
    }

    // Update is called once per frame
    private void Update()
    {
        var s = new StringBuilder();

        s.Append(Keyboard.current.wKey.isPressed ? "w" : "_");
        s.Append(Keyboard.current.aKey.isPressed ? "a" : "_");
        s.Append(Keyboard.current.sKey.isPressed ? "s" : "_");
        s.Append(Keyboard.current.dKey.isPressed ? "d" : "_");
        s.Append(Mouse.current.scroll.y.IsActuated() ? " j" : " _");
        var str = s.ToString();
        
        
        var t = GetComponent<TMP_Text>();
        var velocity = cc.velocity;
        t.SetText("x: "+velocity.x+ "\ny: " + velocity.y + "\nz: "+velocity.z +"\nmagnitude: "+velocity.magnitude+"\n"+str);
        
    }
}
