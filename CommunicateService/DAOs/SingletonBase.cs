namespace CommunicateService.DAOs
{
    public class SingletonBase<T> where T : class
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static IServiceProvider _serviceProvider;

        // Inject IServiceProvider để lấy DbContext từ DI
        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (_serviceProvider == null)
                        {
                            throw new InvalidOperationException("ServiceProvider is not configured. Call Configure() first.");
                        }

                        // Lấy instance từ DI container
                        _instance = _serviceProvider.GetRequiredService<T>();
                    }

                    return _instance;
                }
            }
            set => _instance = value;
        }
    }
}
