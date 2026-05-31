using TestTask.Networking.Demo;
using TestTask.Networking.Services;
using Zenject;

namespace TestTask.Installers
{
    public sealed class NetworkServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<MirrorNetworkMessageSubscriptionService>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<HelloMessageClient>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesAndSelfTo<HelloMessageServer>()
                .AsSingle()
                .NonLazy();
        }
    }
}