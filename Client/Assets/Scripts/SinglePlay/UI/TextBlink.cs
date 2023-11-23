using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextBlink : MonoBehaviour
{
	TextMeshProUGUI flashingText;

	// Use this for initialization
	void Start()
	{
		flashingText = GetComponent<TextMeshProUGUI>();
		StartCoroutine(BlinkText());
	}

	public IEnumerator BlinkText()
    {
        bool isTransparent = true;
		byte transparent=0;
        while (true)
		{
            if (isTransparent) // 지금 투명하면
            {
				flashingText.color = new Color32(255, 255, 255, transparent);
				transparent+=5;
                if (transparent == 255)
                {
					isTransparent = false;
                }
				yield return new WaitForSeconds(.01f);
			}
            else // 안투명하면
            {
				flashingText.color = new Color32(255, 255, 255, transparent);
				transparent-=5;
				if (transparent == 0)
				{
					isTransparent = true;
				}
				yield return new WaitForSeconds(.01f);
			}

			//flashingText.color = new Color32(255, 255, 255, 255);
			////flashingText.text = "";
			//yield return new WaitForSeconds(.5f);
			//flashingText.color = new Color32(255, 255, 255, 0);
			//yield return new WaitForSeconds(.5f);
		}
	}
}
