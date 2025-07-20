using UnityEngine;

public class WarriorDetector : MonoBehaviour
{
    [Header("Alarm Settings")]
    [SerializeField] private GameObject alertMark;
    [SerializeField] private AudioSource alarmSound;
    [SerializeField] private float alertCooldown = 3f;

    private Detector detector;
    private Animator animator;

    bool hasAlerted = false;
    float alertTimer = 0f;

    private void Awake()
    {
        detector = GetComponent<Detector>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (detector.Target != null && detector.TargetVisible && !hasAlerted)
        {
            Debug.Log("Detected player!");
            TriggerAlarm();
        }

        if (detector.Target == null || !detector.TargetVisible)
        {
            if (hasAlerted)
            {
                StopAlarm();
            }
        }

        if (hasAlerted)
        {
            alertTimer -= Time.deltaTime;
            if (alertTimer <= 0f)
            {
                hasAlerted = false;
            }
        }
    }

    private void TriggerAlarm()
    {
        hasAlerted = true;
        alertTimer = alertCooldown;

        if (animator != null)
        {
            animator.SetTrigger("FoundPlayer");
        }

        if (alarmSound != null && !alarmSound.isPlaying)
        {
            alarmSound.Play();
        }
        if (alertMark != null)
        {
            alertMark.SetActive(true);
        }
    }


    private void StopAlarm()
    {
        hasAlerted = false;
        alertTimer = 0;

        if (alertMark != null)
        {
            alertMark.SetActive(false);
        }

        if (alarmSound != null && alarmSound.isPlaying)
        {
            alarmSound.Stop();
        }
    }
}
