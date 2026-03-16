using UnityEngine;

public interface IWeapon
{
    bool NormalAttack();
    bool SpecialAttack();
    string GetNormalShake();
    string GetSpecialShake();
}
