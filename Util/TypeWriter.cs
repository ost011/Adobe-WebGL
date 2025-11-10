using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;
using System.Text;

public class TypeWriter: MonoBehaviour
{
	public float startDelayAmount = 0f;

	[Space]
	public float termWhileNextCharAppear = 0f;

	[Space]
	public string fullText = "";

	[Space]
	public UnityEvent onFinishedFunc = null;

	private TextMeshProUGUI textMeshProUGUI = null;
	private StringBuilder sb = new StringBuilder();
	private WaitForSeconds startDelay = null;
	private WaitForSeconds waitForSecondsWhileNextCharAppear = null;

	void Awake()
	{
		InitTypeWriter();
	}

	void Start()
    {
		StartTypeWriting();
	}

	private void InitTypeWriter()
    {
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();

		startDelay = new WaitForSeconds(startDelayAmount);
		waitForSecondsWhileNextCharAppear = new WaitForSeconds(termWhileNextCharAppear);
	}

	private void StartTypeWriting()
    {
		StartCoroutine(CorStartTypeWriting());
	}

	private IEnumerator CorStartTypeWriting()
	{
		sb.Clear();

		yield return startDelay;

		foreach (char c in fullText)
		{
			sb.Append(c);

			textMeshProUGUI.text = sb.ToString();

			yield return waitForSecondsWhileNextCharAppear;
		}

		onFinishedFunc?.Invoke();
	}
}