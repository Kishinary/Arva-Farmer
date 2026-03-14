using UnityEngine;

public interface IWeapon
{
    void NormalAttack();
    void SpecialAttack();
    string GetNormalShake();
    string GetSpecialShake();
}
