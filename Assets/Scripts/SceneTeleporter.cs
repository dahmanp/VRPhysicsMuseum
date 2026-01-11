using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class SceneTeleporter : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
