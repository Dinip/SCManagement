namespace SCManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public bool IsEnabled { get; set; }
        public NotificationType Type { get; set; }
        
        public enum NotificationType : int
        {
            Event_Created = 1,
            Event_Edited = 2,
            Event_Canceled = 3,
            Plan_Descontinued = 4,
            Team_Added = 5,
            Team_Removed = 6,
            TrainingPlan_Assigned = 7,
            TrainingPlan_Deleted = 8,
            TrainingPlan_Edited = 9,
            MealPlan_Assigned = 10,
            MealPlan_Deleted = 11,
            MealPlan_Edited = 12,
            Goal_Assigned = 13,
            Goal_Deleted = 14,
            Goal_Edited = 15,
            Goal_Completed = 16,
            Athletes_Number_Almost_Full = 17,
            Payment_Late = 18,
            Payment_Received = 19,
            Subscription_Canceled = 20,
            Subscription_Expired = 21,
            Subscription_Extended = 22,
            Subscription_Renewed = 23,
            Subscription_Started = 24,
            Club_Quota_Update = 25,
        }
    }
}
