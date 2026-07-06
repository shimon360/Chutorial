using UnityEngine;

public class CatAreaController : MonoBehaviour
{
    [SerializeField]
    private CatController cat;
    [SerializeField]
    private CameraController cameraController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            cat.StartCat();

            cameraController.FocusOn(cat.transform);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            cat.StopCat();
        }
    }

}