using UnityEngine;

public interface IWeapon
{
    float GetFinalDamage();
    bool NormalAttack();
    bool SpecialAttack();
    string GetNormalShake();
    string GetSpecialShake();
}
