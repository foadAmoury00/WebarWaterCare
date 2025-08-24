using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class NavigateToZone : MonoBehaviour
{
    public enum Zones {
        Cave,
        Forest,
        Desert,
        Park,
        Ocean,
        Lava,

    }

    [SerializeField]
    private List<Button> zoneButtons;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < zoneButtons.Count; i++) {
            int zoneIndex = i; 
            zoneButtons[i].onClick.AddListener(() => Navigate((Zones)zoneIndex));
        }
    }

    void Update()
    {
        
    }
    private void Navigate(Zones zone) {
        Debug.Log($"Navigating to: {zone.ToString()}");
        SceneManager.LoadSceneAsync((int)(zone + 2));


    }

    public void AddZoneButton(Button newButton, Zones associatedZone) {
        zoneButtons.Add(newButton);
        newButton.onClick.AddListener(() => Navigate(associatedZone));
    }

}
