using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class ActiveSkillUITester : MonoBehaviour
{
    [SerializeField] ActiveSkillUI _activeSkillUI;
    [SerializeField] EffectData A;
    [SerializeField] EffectData B;
    [SerializeField] EffectData C;
    [SerializeField] EffectData D;

    #region Test Function

    public void Btn_SetSkill01()
    {
        _activeSkillUI.SetSkill(0, new ActiveSkillDetail(A,Size.Small), () => { Debug.Log("Press skill01"); });
        _activeSkillUI.SetSkill(1, new ActiveSkillDetail(B, Size.Medium), () => { Debug.Log("Press skill02"); });
        _activeSkillUI.SetSkill(2, new ActiveSkillDetail(C, Size.Big), () => { Debug.Log("Press skill03"); });
    }

    public void Btn_SetSkill02()
    {
        _activeSkillUI.SetSkill(2, new ActiveSkillDetail(D, Size.Big), () => { Debug.Log("Press skill04"); });
    }

    #endregion
}
