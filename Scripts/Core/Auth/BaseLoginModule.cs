namespace Scripts.Core.Auth
{
    public abstract class BaseLoginModule
    {
        public bool pIsInitialized { get; protected set; }
        public abstract void Initialize();

        public bool pIsEditor { get; protected set; } = false;

        protected const int cMaxTryCount = 3;
        public int pTryCount { get; protected set; }

        public virtual string pUserID { get; }

        protected void ClearTryCount()
        {
            pTryCount = 0;
        }

        public bool IsAvailableTry()
        {
            return pTryCount < cMaxTryCount;
        }
        
        public abstract void Login(System.Action<bool, string> aCallback);
        public abstract void Logout(System.Action<bool, string> aCallback);
    }
}