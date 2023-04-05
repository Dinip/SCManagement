namespace SCManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public bool IsEnabled { get; set; }
        public NotificationType Type { get; set; }
    }

    public enum NotificationType : int
    {
        Event_Created = 1,
        Event_Edited = 2,
        Event_Canceled = 3,
        Event_Joined = 4,
        Event_Left = 5,
        //missing 6, dont change
        Team_Added = 7,
        Team_Removed = 8,
        TrainingPlan_Assigned = 9,
        TrainingPlan_Deleted = 10,
        TrainingPlan_Edited = 11,
        MealPlan_Assigned = 12,
        MealPlan_Deleted = 13,
        MealPlan_Edited = 14,
        Goal_Assigned = 15,
        Goal_Deleted = 16,
        Goal_Edited = 17,
        Goal_Completed = 18,
        Plan_Discontinued = 19,
        Athletes_Number_Almost_Full = 20,
        Payment_Late = 21,
        Payment_Received = 22,
        Subscription_Started = 23,
        Subscription_Renewed = 24,
        Subscription_Expired = 25,
        Subscription_Canceled = 26,
        Club_Quota_Update = 27,
        Subscription_RenewTime = 28,
    }
}
