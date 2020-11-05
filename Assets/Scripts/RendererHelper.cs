using UnityEngine;

public class RendererHelper : MonoBehaviour
{
	void Update()
	{
		if (GetComponent<Renderer>().IsVisibleFrom(Camera.main)) Debug.Log("Visible");
		else Debug.Log("Not visible");
	}
}