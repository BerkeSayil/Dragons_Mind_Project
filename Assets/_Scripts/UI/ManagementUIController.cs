
using UnityEngine;

public class ManagementUIController : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    // indexes are according to buttons

    private void Awake() {
        
    }


    //Contracts Panel

    //Employee Hiring Panel
    public void HireEmployee(string occupation) {

        

    }



    //RnD Panel
    
    //Company Management Panel



    // Close all other panels and activate the one we need.
    public void LoadContractsTab() {

        CloseAllPanels();

        if (panels[0].activeInHierarchy == false) panels[0].SetActive(true);
    }
    public void LoadEmployeeHiringTab() {

        CloseAllPanels();

        if (panels[1].activeInHierarchy == false) panels[1].SetActive(true);

    }
    public void LoadRnDTab() {

        CloseAllPanels();

        if (panels[2].activeInHierarchy == false) panels[2].SetActive(true);

    }
    public void LoadCompanyManagementTab() {

        CloseAllPanels();

        if (panels[3].activeInHierarchy == false) panels[3].SetActive(true);

    }

    private void CloseAllPanels() {
        foreach (var panel in panels) {

            if (panel.activeInHierarchy) panel.SetActive(false);

        }
    }
}
