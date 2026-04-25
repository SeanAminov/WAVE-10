using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Color normalColor = new Color(0.55f, 0f, 0f);
    [SerializeField] Color hoverColor = new Color(1f, 0.1f, 0.1f);

    Image buttonImage;

    void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
            buttonImage.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.color = normalColor;
    }
}