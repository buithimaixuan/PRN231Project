namespace PostService.DAOs
{
    public class SingletonBaseDAO<T> where T : class, new()
    {
        private static T _instance;
        private static readonly object _lock = new object();
        public static PostDbContext _context { get; set; } = new PostDbContext();

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
            set { _instance = value; }
        }
    }
}
