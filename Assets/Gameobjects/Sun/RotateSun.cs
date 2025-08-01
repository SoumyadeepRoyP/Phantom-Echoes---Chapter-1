using UnityEngine;

public class RotateSun : MonoBehaviour
{
	public float speed;
	GameObject[] streetlights;
	Light[] lights;

	void Start()
	{
		streetlights = GameObject.FindGameObjectsWithTag("Light Emitter");
		lights = new Light[streetlights.Length];
		for (int i = 0; i < streetlights.Length; i++)
		{
			lights[i] = streetlights[i].GetComponent<Light>();
		}
	}
	void Update()
	{
		transform.Rotate(speed * Time.deltaTime, 0, 0);
		if (transform.eulerAngles.x%360 > 180f)
		{
			//Enable lights
			foreach (var light in lights)
			{
				if (!light.enabled)
				{
					light.enabled = !light.enabled;
				}
			}
		}
		else
		{
			//Disable lights
			foreach (var light in lights)
			{
				if (light.enabled)
				{
					light.enabled = !light.enabled;
				}
			}
		}
    }
}
