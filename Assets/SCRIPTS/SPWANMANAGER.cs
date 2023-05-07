using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SPWANMANAGER : MonoBehaviour
{
     enum ClickAble
    {
        Cube ,
        Sphere
    }
    ClickAble ClickToSwitch;
    RaycastHit hit;

    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    //  variables for cube and sphere size
    [SerializeField] Vector3 cubeSize = Vector3.one;
    [SerializeField] float sphereRadius = 0.5f , yaxis = 0.5f;

    // lists to keep track of all instantiated objects
    private List<GameObject> cubes = new List<GameObject>();
    private List<GameObject> spheres = new List<GameObject>();

    // reference to the object being edited (null if not in edit mode)
    [SerializeField] GameObject 
        selectedObject = null;

    // material for the edit mode
    [SerializeField]
    Material
        editMaterial,
        normalMaterial ,
        black;

    // flag to indicate if we are in translate or rotate mode
    private bool 
        isRotateMod = false,
        isEditmod = false;
   


    // Start is called before the first frame update
    void Start()
    {
        // create a default plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = Vector3.zero;
        plane.GetComponent<MeshRenderer>().material = black;
    }

    // Update is called once per frame
    void Update()
    {
        OnMOuseSeleected();
        EditMod();
    }
    // create a cube at the given position
    void CreateCube(Vector3 position)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;

        cube.transform.localScale = cubeSize;
        cubes.Add(cube);
    }

    // create a sphere at the given position
    void CreateSphere(Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = Vector3.one * sphereRadius;
        spheres.Add(sphere);
    }

    public void ToggleMod(string value)
    {
        if(value == "Cube") ClickToSwitch = ClickAble.Cube;
        else if(value == "sphere") ClickToSwitch = ClickAble.Sphere;
    }
    // Get Position
    public Vector3 MousePosition() {

        Ray newray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(newray , out hit , 100))
        {
            return new Vector3(hit.point.x,yaxis,hit.point.z);
        }
        return Vector3.one;


    }
    // Create Gameobject based on Toggle mod
    public void CreateObject()
    {
       
            if(ClickToSwitch == ClickAble.Cube)
            {
                Debug.Log(MousePosition());
                CreateCube(MousePosition());
            }
            else
            {
                Debug.Log(MousePosition());
               CreateSphere(MousePosition());
            }
        
    }   
    public void EditMod()
    {
 
        // check if the delete key is pressed and an object is selected
        if (Input.GetKeyDown(KeyCode.Delete) && selectedObject != null)
        {
            // remove the object from the list and destroy it
            if (cubes.Contains(selectedObject))
            {
                cubes.Remove(selectedObject);
            }
            else if (spheres.Contains(selectedObject))
            {
                spheres.Remove(selectedObject);
            }
            Destroy(selectedObject);
            selectedObject = null; // reset selected object
        }

        // check if we are in edit mode and the selected object is not null
        if (selectedObject != null && selectedObject.GetComponent<Renderer>() != null)
        {
            textMeshProUGUI.text = "EditMod";
            // change the material to the edit material
            MetarialChange();
            selectedObject.GetComponent<Renderer>().material = editMaterial;

            if(selectedObject != null) { isEditmod = true; }

            // check if the rotate button is pressed
            if (Input.GetKeyDown(KeyCode.R))
            {
                isRotateMod = !isRotateMod; // toggle between translate and rotate mode
            }

            // check if the mouse button is pressed
            if (Input.GetMouseButton(0))
            {
                // get the mouse position in the world space
                Vector3 mousePos = MousePosition();
                mousePos.y = 1; // set the y position to 0 to stay on the plane
                  

                if (isRotateMod)
                {
                    // rotate the object based on the mouse movement
                    float rotationSpeed = 10.0f;
                    float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
                    float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
                    selectedObject.transform.Rotate(Vector3.up, -horizontalRotation, Space.World);
                    selectedObject.transform.Rotate(Vector3.right, verticalRotation, Space.World);
                }
                else
                {
                    
                    selectedObject.transform.position = mousePos;
                }
            }
            else
            {
                isEditmod = false;
            }
        }

    }
    //change mats
    public void MetarialChange()
    {
        foreach (var obj in spheres)
        {
            obj.GetComponent<MeshRenderer>().material = normalMaterial;
        }
        foreach (var obj in cubes)
        {
            obj.GetComponent<MeshRenderer>().material = normalMaterial;
        }
    }

    // handle mouse clicks
    void OnMOuseSeleected()
    {
      
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
              
                // check if the object is a cube or sphere and select it for edit mode
                if (cubes.Contains(hit.transform.gameObject))
                {
                    
                    selectedObject = hit.transform.gameObject;
                }
                else if (spheres.Contains(hit.transform.gameObject))
                {
                    selectedObject = hit.transform.gameObject;
                }
                else
                {
                    selectedObject = null;
                    isEditmod = false;
                    CreateObject(); textMeshProUGUI.text = "Normal Mod";
                    MetarialChange();
                    
                }
            }
            else
            {
                selectedObject = null; // reset selected object if we clicked on empty space

            }
        }
    }

    #region Save And Load System

    // save the scene state using player prefs
    void SaveState()
    {
        PlayerPrefs.SetInt("NumCubes", cubes.Count);
        PlayerPrefs.SetInt("NumSpheres", spheres.Count);
        for (int i = 0; i < cubes.Count; i++)
        {
            PlayerPrefs.SetFloat("CubePosX" + i, cubes[i].transform.position.x);
            PlayerPrefs.SetFloat("CubePosY" + i, cubes[i].transform.position.y);
            PlayerPrefs.SetFloat("CubePosZ" + i, cubes[i].transform.position.z);
        }
        for (int i = 0; i < spheres.Count; i++)
        {
            PlayerPrefs.SetFloat("SpherePosX" + i, spheres[i].transform.position.x);
            PlayerPrefs.SetFloat("SpherePosY" + i, spheres[i].transform.position.y);
            PlayerPrefs.SetFloat("SpherePosZ" + i, spheres[i].transform.position.z);
        }
    }

    // load the scene state from player prefs
    void LoadState()
    {
        int numCubes = PlayerPrefs.GetInt("NumCubes");
        int numSpheres = PlayerPrefs.GetInt("NumSpheres");
        for (int i = 0; i < numCubes; i++)
        {
            float x = PlayerPrefs.GetFloat("CubePosX" + i);
            float y = PlayerPrefs.GetFloat("CubePosY" + i);
            float z = PlayerPrefs.GetFloat("CubePosZ" + i);
            CreateCube(new Vector3(x, y, z));
        }
        for (int i = 0; i < numSpheres; i++)
        {
            float x = PlayerPrefs.GetFloat("SpherePosX" + i);
            float y = PlayerPrefs.GetFloat("SpherePosY" + i);
            float z = PlayerPrefs.GetFloat("SpherePosZ" + i);
            CreateSphere(new Vector3(x, y, z));
        }
    }
    #endregion
    // save the scene state when the application is closed
    private void OnApplicationQuit()
    {
        SaveState();
    }

    // load the scene state when the application starts
    private void Awake()
    {
        LoadState();
    }

}
