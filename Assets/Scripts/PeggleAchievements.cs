using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeggleAchievements : MonoBehaviour
{
    public enum AchievementRarity
    {
        Bronze,
        Silver,
        Gold,
        Platinum,
        Diamond
    }


    public delegate void AchievementUnlocked(string title, string message, AchievementRarity rarity);
    public static event AchievementUnlocked OnAchievementUnlocked;


    //public PeggleManager peggleManager;
    private bool pacifistAchieved=false;
    private bool comboAchieved=false;
    private bool neverSurrenderAchieved= false;

    private int bumpersActivated=0;
    private int bumpersDestroyed=0;

    void OnEnable()
    {
        Bumper.OnBumperActivated+= OnBumperActivated;
        Bumper.OnBumperDestroyed+= OnBumperDestroyed;
        PeggleManager.OnNewRound+= OnNewRound;
    }
    void OnDisable()
    {
        Bumper.OnBumperActivated-= OnBumperActivated;
        Bumper.OnBumperDestroyed-= OnBumperDestroyed;
        PeggleManager.OnNewRound-= OnNewRound;
    }
    void OnBumperActivated(Bumper bumper) 
    {
        bumpersActivated++;
        CheckComboAchievement();
    }
    void OnBumperDestroyed (Bumper bumper)
    {
        bumpersDestroyed++;
    }

    void OnNewRound()
    {
        CheckPacifistAchievement();
        bumpersActivated=0;
        CheckNeverSurrender();
    }

    void CheckPacifistAchievement()
    {
        if(bumpersActivated==0 && bumpersDestroyed==0 && !pacifistAchieved)
        {
            pacifistAchieved=true;
            //Debug.Log("Logro! Pierde una bola sin destruir ningun bumper.");

            if (OnAchievementUnlocked !=null)
                OnAchievementUnlocked("The Pacifist!", "Try harder next time!", AchievementRarity.Bronze);
        }  

    }

    void CheckComboAchievement()
    {
        if(bumpersActivated>=2 && !comboAchieved)
        {
            comboAchieved= true;
            //Debug.Log("Logro! Combo conseguido.");

            if (OnAchievementUnlocked != null)
                OnAchievementUnlocked("Combo breaker!", "Keep it up!", AchievementRarity.Gold);
        }
    }

    void CheckNeverSurrender()
    {
        if (bumpersActivated>=2 && !neverSurrenderAchieved)
        {
            neverSurrenderAchieved=true;
            //Debug.Log("Logro! Never surrender");
            if (OnAchievementUnlocked != null)
                OnAchievementUnlocked("Never", "Ever surrender!", AchievementRarity.Silver);
        }

    }

}
