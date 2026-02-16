// =====================================================
// ENHANCED ENTITY MODELS FOR .NET 10 - PRODUCTION READY
// =====================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantApp.Models
{
    // =====================================================
    // AUDIT & CONFIGURATION
    // =====================================================

    [Table("audit_logs")]
    public class AuditLog
    {
        [Key]
        [Column("audit_id")]
        public long AuditId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("table_name")]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [Column("record_id")]
        public int RecordId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("action")]
        public string Action { get; set; } = string.Empty;

        [Column("old_values", TypeName = "jsonb")]
        public string? OldValues { get; set; }

        [Column("new_values", TypeName = "jsonb")]
        public string? NewValues { get; set; }

        [Column("changed_by")]
        public int? ChangedBy { get; set; }

        [MaxLength(20)]
        [Column("changed_by_type")]
        public string? ChangedByType { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    [Table("system_settings")]
    public class SystemSetting
    {
        [Key]
        [Column("setting_id")]
        public int SettingId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("setting_key")]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        [Column("setting_value")]
        public string SettingValue { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("data_type")]
        public string DataType { get; set; } = "string";

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_public")]
        public bool IsPublic { get; set; } = false;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    // =====================================================
    // MENU WITH OPTIONS
    // =====================================================

    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }

    [Table("menu_items")]
    public class MenuItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [MaxLength(500)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [MaxLength(500)]
        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("is_featured")]
        public bool IsFeatured { get; set; } = false;

        [Column("preparation_time_minutes")]
        public int PreparationTimeMinutes { get; set; } = 15;

        [Column("calories")]
        public int? Calories { get; set; }

        [Column("spice_level")]
        public int SpiceLevel { get; set; } = 0;

        [Column("is_vegetarian")]
        public bool IsVegetarian { get; set; } = false;

        [Column("is_vegan")]
        public bool IsVegan { get; set; } = false;

        [Column("is_gluten_free")]
        public bool IsGlutenFree { get; set; } = false;

        [Column("is_dairy_free")]
        public bool IsDairyFree { get; set; } = false;

        [Column("is_nut_free")]
        public bool IsNutFree { get; set; } = false;

        [Column("allergen_info")]
        public string? AllergenInfo { get; set; }

        [Column("max_quantity_per_order")]
        public int MaxQuantityPerOrder { get; set; } = 10;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<MenuItemOptionGroup> OptionGroups { get; set; } = new List<MenuItemOptionGroup>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<MenuItemIngredient> Ingredients { get; set; } = new List<MenuItemIngredient>();
    }

    [Table("menu_item_option_groups")]
    public class MenuItemOptionGroup
    {
        [Key]
        [Column("option_group_id")]
        public int OptionGroupId { get; set; }

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("is_required")]
        public bool IsRequired { get; set; } = false;

        [MaxLength(20)]
        [Column("selection_type")]
        public string SelectionType { get; set; } = "single";

        [Column("min_selections")]
        public int MinSelections { get; set; } = 0;

        [Column("max_selections")]
        public int MaxSelections { get; set; } = 1;

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ItemId")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        public virtual ICollection<MenuItemOption> Options { get; set; } = new List<MenuItemOption>();
    }

    [Table("menu_item_options")]
    public class MenuItemOption
    {
        [Key]
        [Column("option_id")]
        public int OptionId { get; set; }

        [Required]
        [Column("option_group_id")]
        public int OptionGroupId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("price_adjustment", TypeName = "decimal(10,2)")]
        public decimal PriceAdjustment { get; set; } = 0;

        [Column("calories_adjustment")]
        public int CaloriesAdjustment { get; set; } = 0;

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OptionGroupId")]
        public virtual MenuItemOptionGroup OptionGroup { get; set; } = null!;

        public virtual ICollection<OrderItemOption> OrderItemOptions { get; set; } = new List<OrderItemOption>();
    }

    // =====================================================
    // CUSTOMERS
    // =====================================================

    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("date_of_birth")]
        public DateOnly? DateOfBirth { get; set; }

        [MaxLength(500)]
        [Column("profile_image_url")]
        public string? ProfileImageUrl { get; set; }

        [Column("loyalty_points")]
        public int LoyaltyPoints { get; set; } = 0;

        [Column("total_orders")]
        public int TotalOrders { get; set; } = 0;

        [Column("total_spent", TypeName = "decimal(10,2)")]
        public decimal TotalSpent { get; set; } = 0;

        [Column("average_rating", TypeName = "decimal(3,2)")]
        public decimal? AverageRating { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("is_verified")]
        public bool IsVerified { get; set; } = false;

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
        public virtual ICollection<CustomerPaymentMethod> PaymentMethods { get; set; } = new List<CustomerPaymentMethod>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    [Table("customer_addresses")]
    public class CustomerAddress
    {
        [Key]
        [Column("address_id")]
        public int AddressId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [MaxLength(50)]
        [Column("label")]
        public string? Label { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("address_line1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [MaxLength(255)]
        [Column("address_line2")]
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("city")]
        public string City { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("state")]
        public string? State { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("postal_code")]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("country")]
        public string Country { get; set; } = "USA";

        [Column("latitude", TypeName = "decimal(10,8)")]
        public decimal? Latitude { get; set; }

        [Column("longitude", TypeName = "decimal(11,8)")]
        public decimal? Longitude { get; set; }

        [Column("delivery_instructions")]
        public string? DeliveryInstructions { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }

    [Table("customer_payment_methods")]
    public class CustomerPaymentMethod
    {
        [Key]
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payment_type")]
        public string PaymentType { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("token")]
        public string Token { get; set; } = string.Empty;

        [MaxLength(4)]
        [Column("last_four")]
        public string? LastFour { get; set; }

        [MaxLength(20)]
        [Column("card_brand")]
        public string? CardBrand { get; set; }

        [Column("expiry_month")]
        public int? ExpiryMonth { get; set; }

        [Column("expiry_year")]
        public int? ExpiryYear { get; set; }

        [Column("is_default")]
        public bool IsDefault { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }

    // =====================================================
    // STAFF & ROLES
    // =====================================================

    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("permissions", TypeName = "jsonb")]
        public string Permissions { get; set; } = "[]";

        public virtual ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
    }

    [Table("staff")]
    public class Staff
    {
        [Key]
        [Column("staff_id")]
        public int StaffId { get; set; }

        [Required]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(500)]
        [Column("profile_image_url")]
        public string? ProfileImageUrl { get; set; }

        [Required]
        [Column("hire_date")]
        public DateTime HireDate { get; set; }

        [MaxLength(20)]
        [Column("employment_status")]
        public string EmploymentStatus { get; set; } = "active";

        [Column("hourly_rate", TypeName = "decimal(10,2)")]
        public decimal? HourlyRate { get; set; }

        [Column("last_login_at")]
        public DateTime? LastLoginAt { get; set; }

        [Column("failed_login_attempts")]
        public int FailedLoginAttempts { get; set; } = 0;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Order> OrdersAsChef { get; set; } = new List<Order>();
        public virtual ICollection<Order> OrdersAsDriver { get; set; } = new List<Order>();
    }

    // =====================================================
    // TABLES & RESERVATIONS
    // =====================================================

    [Table("restaurant_tables")]
    public class RestaurantTable
    {
        [Key]
        [Column("table_id")]
        public int TableId { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("table_number")]
        public string TableNumber { get; set; } = string.Empty;

        [Required]
        [Column("capacity")]
        public int Capacity { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "available";

        [MaxLength(50)]
        [Column("location")]
        public string? Location { get; set; }

        [MaxLength(500)]
        [Column("qr_code_url")]
        public string? QrCodeUrl { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    [Table("reservations")]
    public class Reservation
    {
        [Key]
        [Column("reservation_id")]
        public int ReservationId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("table_id")]
        public int? TableId { get; set; }

        [Required]
        [Column("reservation_date")]
        public DateOnly ReservationDate { get; set; }

        [Required]
        [Column("reservation_time")]
        public TimeOnly ReservationTime { get; set; }

        [Required]
        [Column("party_size")]
        public int PartySize { get; set; }

        [MaxLength(20)]
        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("special_requests")]
        public string? SpecialRequests { get; set; }

        [Column("customer_notes")]
        public string? CustomerNotes { get; set; }

        [Column("staff_notes")]
        public string? StaffNotes { get; set; }

        [MaxLength(20)]
        [Column("confirmation_code")]
        public string? ConfirmationCode { get; set; }

        [Column("reminded_at")]
        public DateTime? RemindedAt { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("TableId")]
        public virtual RestaurantTable? Table { get; set; }
    }

    // =====================================================
    // ORDERS - WITH LIFECYCLE
    // =====================================================

    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("staff_id")]
        public int? StaffId { get; set; }

        [Column("assigned_chef_id")]
        public int? AssignedChefId { get; set; }

        [Column("assigned_driver_id")]
        public int? AssignedDriverId { get; set; }

        [Column("table_id")]
        public int? TableId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("order_type")]
        public string OrderType { get; set; } = "dine_in"; // pickup, delivery, dine_in

        [Required]
        [MaxLength(20)]
        [Column("order_status")]
        public string OrderStatus { get; set; } = "pending";

        [Required]
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; } = 0;

        [Column("tax_rate", TypeName = "decimal(5,4)")]
        public decimal TaxRate { get; set; } = 0.08m;

        [Required]
        [Column("tax_amount", TypeName = "decimal(10,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column("delivery_fee", TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; } = 0;

        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [MaxLength(50)]
        [Column("discount_code")]
        public string? DiscountCode { get; set; }

        [Column("loyalty_points_used")]
        public int LoyaltyPointsUsed { get; set; } = 0;

        [Column("loyalty_points_discount", TypeName = "decimal(10,2)")]
        public decimal LoyaltyPointsDiscount { get; set; } = 0;

        [Column("tip_amount", TypeName = "decimal(10,2)")]
        public decimal TipAmount { get; set; } = 0;

        [Required]
        [Column("total_amount", TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; } = 0;

        [Column("estimated_ready_time")]
        public DateTime? EstimatedReadyTime { get; set; }

        [Column("actual_ready_time")]
        public DateTime? ActualReadyTime { get; set; }

        [Column("estimated_delivery_time")]
        public DateTime? EstimatedDeliveryTime { get; set; }

        [Column("actual_delivery_time")]
        public DateTime? ActualDeliveryTime { get; set; }

        [Column("customer_notes")]
        public string? CustomerNotes { get; set; }

        [Column("kitchen_notes")]
        public string? KitchenNotes { get; set; }

        [Column("delivery_notes")]
        public string? DeliveryNotes { get; set; }

        [Column("delivery_address_id")]
        public int? DeliveryAddressId { get; set; }

        [Column("delivery_latitude", TypeName = "decimal(10,8)")]
        public decimal? DeliveryLatitude { get; set; }

        [Column("delivery_longitude", TypeName = "decimal(11,8)")]
        public decimal? DeliveryLongitude { get; set; }

        [Column("customer_rating")]
        public int? CustomerRating { get; set; }

        [Column("customer_feedback")]
        public string? CustomerFeedback { get; set; }

        [Column("cancelled_at")]
        public DateTime? CancelledAt { get; set; }

        [Column("cancellation_reason")]
        public string? CancellationReason { get; set; }

        [MaxLength(20)]
        [Column("cancelled_by_type")]
        public string? CancelledByType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }

        [ForeignKey("AssignedChefId")]
        public virtual Staff? AssignedChef { get; set; }

        [ForeignKey("AssignedDriverId")]
        public virtual Staff? AssignedDriver { get; set; }

        [ForeignKey("TableId")]
        public virtual RestaurantTable? Table { get; set; }

        [ForeignKey("DeliveryAddressId")]
        public virtual CustomerAddress? DeliveryAddress { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

    [Table("order_status_history")]
    public class OrderStatusHistory
    {
        [Key]
        [Column("history_id")]
        public long HistoryId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [MaxLength(20)]
        [Column("from_status")]
        public string? FromStatus { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("to_status")]
        public string ToStatus { get; set; } = string.Empty;

        [Column("changed_by_id")]
        public int? ChangedById { get; set; }

        [MaxLength(20)]
        [Column("changed_by_type")]
        public string? ChangedByType { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }

    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("unit_price", TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column("subtotal", TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Column("item_notes")]
        public string? ItemNotes { get; set; }

        [MaxLength(20)]
        [Column("item_status")]
        public string ItemStatus { get; set; } = "pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ItemId")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        public virtual ICollection<OrderItemOption> SelectedOptions { get; set; } = new List<OrderItemOption>();
    }

    [Table("order_item_options")]
    public class OrderItemOption
    {
        [Key]
        [Column("order_item_option_id")]
        public int OrderItemOptionId { get; set; }

        [Required]
        [Column("order_item_id")]
        public int OrderItemId { get; set; }

        [Required]
        [Column("option_id")]
        public int OptionId { get; set; }

        [MaxLength(100)]
        [Column("option_group_name")]
        public string? OptionGroupName { get; set; }

        [MaxLength(100)]
        [Column("option_name")]
        public string? OptionName { get; set; }

        [Column("price_adjustment", TypeName = "decimal(10,2)")]
        public decimal PriceAdjustment { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderItemId")]
        public virtual OrderItem OrderItem { get; set; } = null!;

        [ForeignKey("OptionId")]
        public virtual MenuItemOption Option { get; set; } = null!;
    }

    // =====================================================
    // PAYMENTS
    // =====================================================

    [Table("payments")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payment_method")]
        public string PaymentMethod { get; set; } = "cash";

        [Required]
        [Column("amount", TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "pending";

        [MaxLength(255)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        [MaxLength(50)]
        [Column("gateway")]
        public string? Gateway { get; set; }

        [Column("gateway_response", TypeName = "jsonb")]
        public string? GatewayResponse { get; set; }

        [Column("refund_amount", TypeName = "decimal(10,2)")]
        public decimal RefundAmount { get; set; } = 0;

        [Column("refund_reason")]
        public string? RefundReason { get; set; }

        [Column("refunded_at")]
        public DateTime? RefundedAt { get; set; }

        [Column("payment_date")]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }

    // =====================================================
    // INVENTORY
    // =====================================================

    [Table("inventory_categories")]
    public class InventoryCategory
    {
        [Key]
        [Column("inventory_category_id")]
        public int InventoryCategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
    }

    [Table("inventory_items")]
    public class InventoryItem
    {
        [Key]
        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Column("inventory_category_id")]
        public int? InventoryCategoryId { get; set; }

        [MaxLength(50)]
        [Column("sku")]
        public string? Sku { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("unit_of_measure")]
        public string UnitOfMeasure { get; set; } = string.Empty;

        [Required]
        [Column("current_quantity", TypeName = "decimal(10,2)")]
        public decimal CurrentQuantity { get; set; } = 0;

        [Required]
        [Column("minimum_quantity", TypeName = "decimal(10,2)")]
        public decimal MinimumQuantity { get; set; } = 0;

        [Required]
        [Column("reorder_quantity", TypeName = "decimal(10,2)")]
        public decimal ReorderQuantity { get; set; } = 0;

        [Required]
        [Column("unit_cost", TypeName = "decimal(10,2)")]
        public decimal UnitCost { get; set; }

        [MaxLength(200)]
        [Column("supplier_name")]
        public string? SupplierName { get; set; }

        [MaxLength(200)]
        [Column("supplier_contact")]
        public string? SupplierContact { get; set; }

        [Column("last_restocked_at")]
        public DateTime? LastRestockedAt { get; set; }

        [Column("next_restock_date")]
        public DateOnly? NextRestockDate { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("InventoryCategoryId")]
        public virtual InventoryCategory? InventoryCategory { get; set; }

        public virtual ICollection<MenuItemIngredient> MenuItemIngredients { get; set; } = new List<MenuItemIngredient>();
        public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }

    [Table("inventory_transactions")]
    public class InventoryTransaction
    {
        [Key]
        [Column("transaction_id")]
        public long TransactionId { get; set; }

        [Required]
        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("transaction_type")]
        public string TransactionType { get; set; } = string.Empty;

        [Required]
        [Column("quantity_change", TypeName = "decimal(10,2)")]
        public decimal QuantityChange { get; set; }

        [Required]
        [Column("quantity_after", TypeName = "decimal(10,2)")]
        public decimal QuantityAfter { get; set; }

        [Column("unit_cost", TypeName = "decimal(10,2)")]
        public decimal? UnitCost { get; set; }

        [Column("reference_id")]
        public int? ReferenceId { get; set; }

        [MaxLength(50)]
        [Column("reference_type")]
        public string? ReferenceType { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("staff_id")]
        public int? StaffId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem InventoryItem { get; set; } = null!;

        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }
    }

    [Table("menu_item_ingredients")]
    public class MenuItemIngredient
    {
        [Key]
        [Column("menu_item_ingredient_id")]
        public int MenuItemIngredientId { get; set; }

        [Required]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [Column("inventory_item_id")]
        public int InventoryItemId { get; set; }

        [Required]
        [Column("quantity_required", TypeName = "decimal(10,3)")]
        public decimal QuantityRequired { get; set; }

        [ForeignKey("ItemId")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        [ForeignKey("InventoryItemId")]
        public virtual InventoryItem InventoryItem { get; set; } = null!;
    }

    // =====================================================
    // PROMOTIONS & DISCOUNTS
    // =====================================================

    [Table("discount_codes")]
    public class DiscountCode
    {
        [Key]
        [Column("discount_code_id")]
        public int DiscountCodeId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("discount_type")]
        public string DiscountType { get; set; } = string.Empty;

        [Required]
        [Column("discount_value", TypeName = "decimal(10,2)")]
        public decimal DiscountValue { get; set; }

        [Column("minimum_order_amount", TypeName = "decimal(10,2)")]
        public decimal MinimumOrderAmount { get; set; } = 0;

        [Column("max_uses")]
        public int? MaxUses { get; set; }

        [Column("uses_per_customer")]
        public int UsesPerCustomer { get; set; } = 1;

        [Column("current_uses")]
        public int CurrentUses { get; set; } = 0;

        [Required]
        [Column("valid_from")]
        public DateTime ValidFrom { get; set; }

        [Required]
        [Column("valid_until")]
        public DateTime ValidUntil { get; set; }

        [Column("applicable_order_types")]
        public string[]? ApplicableOrderTypes { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<DiscountCodeUsage> Usages { get; set; } = new List<DiscountCodeUsage>();
    }

    [Table("discount_code_usage")]
    public class DiscountCodeUsage
    {
        [Key]
        [Column("usage_id")]
        public int UsageId { get; set; }

        [Required]
        [Column("discount_code_id")]
        public int DiscountCodeId { get; set; }

        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; }

        [Required]
        [Column("discount_amount", TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        [Column("used_at")]
        public DateTime UsedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("DiscountCodeId")]
        public virtual DiscountCode DiscountCode { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }

    // =====================================================
    // NOTIFICATIONS
    // =====================================================

    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("notification_id")]
        public long NotificationId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("recipient_type")]
        public string RecipientType { get; set; } = string.Empty;

        [Required]
        [Column("recipient_id")]
        public int RecipientId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("notification_type")]
        public string NotificationType { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("related_entity_type")]
        public string? RelatedEntityType { get; set; }

        [Column("related_entity_id")]
        public int? RelatedEntityId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("delivery_channel")]
        public string DeliveryChannel { get; set; } = string.Empty;

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("read_at")]
        public DateTime? ReadAt { get; set; }

        [Column("sent_at")]
        public DateTime? SentAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    // =====================================================
    // REVIEWS & RATINGS
    // =====================================================

    [Table("reviews")]
    public class Review
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        [Column("food_rating")]
        public int? FoodRating { get; set; }

        [Column("service_rating")]
        public int? ServiceRating { get; set; }

        [Column("delivery_rating")]
        public int? DeliveryRating { get; set; }

        [Column("review_text")]
        public string? ReviewText { get; set; }

        [Column("response_text")]
        public string? ResponseText { get; set; }

        [Column("responded_by_id")]
        public int? RespondedById { get; set; }

        [Column("responded_at")]
        public DateTime? RespondedAt { get; set; }

        [Column("is_published")]
        public bool IsPublished { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("RespondedById")]
        public virtual Staff? RespondedBy { get; set; }
    }
}
