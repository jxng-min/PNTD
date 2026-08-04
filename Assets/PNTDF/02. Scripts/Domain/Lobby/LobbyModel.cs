namespace PNTD
{
    public class LobbyModel
    {
        public LobbyDomain Domain { get; }
        public LobbyCompositor Compositor { get; }

        public LobbyModel(LobbyDomain domain, LobbyCompositor compositor)
        {
            Domain = domain;
            Compositor = compositor;
        }

        public void Initialize()
        {
            Compositor.BindEvents();
            Domain.Initialize();
            Compositor.Initialize();
        }

        public void Dispose()
        {
            Compositor.ReleaseEvents();
        }

        public void Show()
        {
            Domain.VisibilitySystem.Show();
        }

        public void Hide()
        {
            Domain.VisibilitySystem.Hide();
        }
    }
}
