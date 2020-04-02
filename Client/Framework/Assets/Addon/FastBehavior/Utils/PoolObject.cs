namespace FastBehavior
{
    public abstract class PoolObject
    {
        public bool delete;
        public abstract void Dispose();

        public virtual void Update()
        {
            
        }
    }
}