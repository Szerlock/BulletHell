using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentUITab : MonoBehaviour
{
    //[SerializeField] private TMP_Text nameText;
    //[SerializeField] private TMP_Text descriptionText;
    //[SerializeField] private Image iconImage;
    [SerializeField] private Button pickButton;

    public AugmentBase augment;

    public void Init(AugmentBase augment)
    {
        this.augment = augment;
        //nameText.text = augment.augmentName;
        //descriptionText.text = augment.description;
        //iconImage.sprite = augment.icon;

        pickButton.onClick.RemoveAllListeners(); 
        pickButton.onClick.AddListener(() =>
        {
            augment.Picked();
        });
    }
}
