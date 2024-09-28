namespace ActionService
{
    public abstract class ListenerPriority
    {
        public const int LOWEST = 0;
        public const int LOW = 250;
        public const int MEDIUM = 500;
        public const int HIGH = 750;
        public const int HIGHEST = 1000;

        public static string GetPriorityName(int priority)
        {
            switch (priority)
            {
                case LOWEST:
                    return "LOWEST";
                case LOW:
                    return "LOW";
                case MEDIUM:
                    return "MEDIUM";
                case HIGH:
                    return "HIGH";
                case HIGHEST:
                    return "HIGHEST";
                default:
                    return "UNKNOWN";
            }
        }
    }
}