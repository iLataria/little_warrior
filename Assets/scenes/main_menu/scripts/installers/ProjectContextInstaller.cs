using UnityEngine;
using Zenject;

using IDBunker;

public class ProjectContextInstaller : MonoInstaller
{
    [SerializeField] private ScreenFadeController _fadeController;

    public override void InstallBindings()
    {
        Container.Bind<LoaderModule>().AsSingle();
        Container.BindInstance(_fadeController).AsSingle();
    }
}