using com.game.player;
using UnityEngine;
using Zenject;
public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerStats>().FromComponentInHierarchy().AsSingle();
        Container.Bind<SoundFXManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerOrbController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<PlayerCombatant>().FromComponentInHierarchy().AsSingle();
        //Container.Bind<ZenjectMemoryPool>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        //Container.BindMemoryPool<GameObject,ZenjectMemoryPool>
    }
}