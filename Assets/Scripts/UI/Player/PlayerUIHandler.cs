using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIHandler : MonoBehaviour
{
    [Header("UI Throw Power Change Settings")]
    public float lerpTimer;
    public float chipSpeed = 2;
    public float maxHealthStats;
    public float currentHealthStats;
    
    public TextMeshProUGUI healthText;
    public Image powerThrowFill1;
    public Image powerThrowFill2;

    [Header("UI Stamina Settings")]
    public float maxStaminaStats;
    public float currentStaminaStats;

    public TextMeshProUGUI staminaText;
    public Image staminaBarFill;

    [Header("UI Throw Settings")]
    public TextMeshProUGUI currentThrow;

    [Header("Points Handler")]
    public TextMeshProUGUI currentPointsCarried;
    public TextMeshProUGUI currentTotalPoints;
    public TextMeshProUGUI scoreTimer;

    /**
    [Header("UI Dash Skill Settings")]
    public TextMeshProUGUI dashAmountText;
    public Image dashSkillFill1;
    public Image dashSkillFill2;

    public float dash1Percentage = 0.5f;
    private const float dashBar1FillAmount = 1f;
    **/
    [Header("Script References")]
    public PlayerStats playerStats;
    public PlayerStateHandler playerStates;
    public PlayerMovement playerMovement;
    public Throwing throwScript;
    public PointsHandler pointsHandler;
    
    private void Awake()
    {
        FindObjects();
        staminaBarFill.fillAmount = playerStats.maxStamina;
        maxHealthStats = playerStats.maxHealth;
        maxStaminaStats = playerStats.maxStamina;

        healthText.SetText((throwScript.throwForce + " / " + throwScript.maxForce).ToString());
    }

    private void Update()
    {
        UpdatePowerUI();
        UpdateStaminaUI();
        //UpdateDashSkillUI();
        UpdateThrowUI();
        UpdatePoints();
    }
    public void FindObjects(){
        healthText = GameObject.Find("healthCounter").GetComponent<TextMeshProUGUI>();
        powerThrowFill1 = GameObject.Find("healthBarFill1").GetComponent<Image>();
        powerThrowFill2 = GameObject.Find("healthBarFill2").GetComponent<Image>();

        staminaText = GameObject.Find("staminaCounter").GetComponent<TextMeshProUGUI>();
        staminaBarFill = GameObject.Find("staminaBarFill1").GetComponent<Image>();

        /**
        dashAmountText = GameObject.Find("dashCounter").GetComponent<TextMeshProUGUI>();
        dashSkillFill1 = GameObject.Find("dashSkillFill1").GetComponent<Image>();
        dashSkillFill2 = GameObject.Find("dashSkillFill2").GetComponent<Image>();
        **/
    }
    public void UpdatePowerUI(){
        float fill1 = powerThrowFill1.fillAmount;
        float fill2 = powerThrowFill2.fillAmount;
        float hFraction = throwScript.throwForce / throwScript.maxForce;
        if(fill2 > hFraction){
            powerThrowFill1.fillAmount = hFraction;
            powerThrowFill2.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            powerThrowFill2.fillAmount = Mathf.Lerp(fill2, hFraction, percentComplete);
            SetPowerText(percentComplete);
        }
        
        if(fill1 < hFraction){
            powerThrowFill2.color = Color.green;
            powerThrowFill2.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            powerThrowFill1.fillAmount = Mathf.Lerp(fill1, powerThrowFill2.fillAmount, percentComplete);
            SetPowerText(percentComplete);
        }
    }
    public void UpdateStaminaUI(){
        float staminaFill1 = staminaBarFill.fillAmount;
        float sFraction = playerStats.currentStamina / playerStats.maxStamina;
        staminaBarFill.fillAmount = sFraction;
        staminaText.SetText((Mathf.RoundToInt(playerStats.currentStamina) + " / " + maxStaminaStats).ToString());
    }
    void SetPowerText(float percentComplete){
        currentHealthStats = Mathf.Lerp(currentHealthStats, playerStats.currentHealth, percentComplete);
        healthText.SetText((Mathf.RoundToInt(throwScript.throwForce) + " / " + throwScript.maxForce).ToString());
    }
    /**
    public void UpdateDashSkillUI(){
        dashAmountText.SetText((playerMovement.playerCurrentDashAmount).ToString());
        if(playerMovement.playerCurrentDashAmount == 1){
            float hFraction = playerMovement.uiDashCooldown / playerStats.oneDashCooldown;
            dashSkillFill2.fillAmount = hFraction;
        }else if(playerMovement.playerCurrentDashAmount == 0){
            dashBar1();
            dashBar2();
        }
    }
    void dashBar1(){
        float hFraction = playerMovement.uiDashCooldown / playerStats.dashCooldown;
        float dashBar1Fill = hFraction / dash1Percentage;

        dashBar1Fill *= dashBar1FillAmount;
        dashSkillFill1.fillAmount = dashBar1Fill;
    }
    void dashBar2(){
        float dashBar1Amount = dash1Percentage * playerStats.dashCooldown;
        float dashBar2Cooldown = playerMovement.uiDashCooldown - dashBar1Amount;
        float dashBar2TotalCooldown = playerStats.dashCooldown - dashBar1Amount;  
        float dashBar2Fill = dashBar2Cooldown / dashBar2TotalCooldown;
        dashSkillFill2.fillAmount = dashBar2Fill;
    }
    **/
    public void UpdateDeathUI(){
        if(playerStates.states == PlayerStates.Dead){
            
        }
    }
    public void UpdateThrowUI(){
        currentThrow.SetText("THROWS:" + Mathf.RoundToInt(throwScript.totalThrows).ToString());
    }
    public void UpdatePoints(){
        currentPointsCarried.SetText(Mathf.RoundToInt(pointsHandler.carriedPoints).ToString());
        currentTotalPoints.SetText("SCORE:" + (Mathf.RoundToInt(pointsHandler.totalPoints)).ToString());
        if(!pointsHandler.strike){
            if(pointsHandler.timer != 0){
                scoreTimer.SetText("SCORE TIME: " + (Mathf.RoundToInt(pointsHandler.timer)).ToString());
            }
        }
        if (pointsHandler.carriedPoints == 10f && pointsHandler.timer >= 5f){
            pointsHandler.strike = true;
            scoreTimer.SetText("STRIKE!");
            pointsHandler.timer = 2f;
        }
    }

}


