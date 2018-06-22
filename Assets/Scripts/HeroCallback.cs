using UnityEngine;

public class HeroCallback : MonoBehaviour
{
    public Hero hero;

    public void DidChain(int chain)
    {
        hero.DidChain(chain);
    }

    public void DidJumpAttack()
    {
        hero.DidJumpAttack();
    }
}
