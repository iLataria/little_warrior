using Zenject;

public class FadeControllerInstaller : Installer<FadeControllerInstaller>
{
    public override void InstallBindings()
    {
        //Container.Bind<FadeController>().FromSubContainerResolve().ByInstaller<FadeControllerSubContainerInstaller>().AsSingle();
    }

    private class FadeControllerSubContainerInstaller : Installer
    {
        public override void InstallBindings()
        {
            //Container.Bind<FadeController>().AsSingle();
        }
    }
}