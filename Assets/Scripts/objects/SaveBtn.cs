//test/debug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBtn : MonoBehaviour {

    public void Save()
    { 
        PlayerData.SaveToFile("TestData");
    }
}
