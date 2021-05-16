using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class SpinWheel : MonoBehaviour
{
	public List<priz> _prize;
	public List<AnimationCurve> animationCurves;
	public bool TimerOn;
	public float Timer;
	private bool spinning;	
	private float anglePerItem;	
	private int randomTime;
	private int itemNumber;
	public Text timeText;
	float timeToDisplay = 120;

	void DisplayTime()
	{
		timeToDisplay -= Time.deltaTime;
		float minutes = Mathf.FloorToInt(timeToDisplay / 60);
		float seconds = Mathf.FloorToInt(timeToDisplay % 60);
		float Hour = Mathf.FloorToInt(timeToDisplay / 60 /60);

		timeText.text = string.Format("{0:00}:{1:00}:{2:00}",Hour, minutes, seconds);
	}
	void Start()
	{
		TimerOn = false;
		spinning = false;
		anglePerItem = 360 / _prize.Count;

	}

		void Update()
		{
			
			if (Input.GetKeyDown(KeyCode.Space) && !spinning && !TimerOn)
			{

				randomTime = UnityEngine.Random.Range(1, 4);
				itemNumber = UnityEngine.Random.Range(0, _prize.Count);
				Debug.Log(itemNumber);
				float maxAngle = 360 * randomTime + (itemNumber * anglePerItem);

				StartCoroutine(SpinTheWheel(5 * randomTime, maxAngle));
			}
			
			if (TimerOn)
			{
			DisplayTime();
			}
			if (timeToDisplay < 0)
			{
			TimerOn = false;
			}
		}

		IEnumerator SpinTheWheel(float time, float maxAngle)
		{
			spinning = true;
			TimerOn = true;
			float timer = 0.0f;
			float startAngle = transform.eulerAngles.z;
			maxAngle = maxAngle + 180 - startAngle;

			int animationCurveNumber = UnityEngine.Random.Range(0, animationCurves.Count);
			Debug.Log("Animation Curve No. : " + animationCurveNumber);

			while (timer < time)
			{
				//to calculate rotation
				float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
				transform.eulerAngles = new Vector3(0.0f, 0.0f, angle + startAngle);
				timer += Time.deltaTime;
				yield return 0;
			}

			transform.eulerAngles = new Vector3(0.0f, 0.0f, maxAngle + startAngle);
			spinning = false;

			Debug.Log("Prize: " + _prize[itemNumber].allPrize + " Is Coin:" + _prize[itemNumber].isCoin);
		}

	} 