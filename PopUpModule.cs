using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpModule : MonoBehaviour, IPopUp
{
    public EnumSets.PopUpType popUpType;

    public GameObject body;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ActivatePopUp()
    {
        this.body.SetActive(true);
    }

    public void DeActivatePopUp()
    {
        this.body.SetActive(false);
    }
}
