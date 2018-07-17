
namespace Engine.Singletons
{
    public abstract class Singleton
    {
        public Singleton()
        {
            Initialize();
        }

        public abstract void Initialize();
    }
}