-- =====================================================
-- ENHANCED RESTAURANT DATABASE SCHEMA - PRODUCTION READY
-- =====================================================
-- Features:
-- 1. Menu item options & customization
-- 2. Order notes & lifecycle tracking
-- 3. Enhanced order types (Pickup, Delivery, DineIn)
-- 4. Production-ready features (audit, soft delete, etc.)
-- =====================================================

-- Enable required extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm"; -- For full-text search

-- =====================================================
-- AUDIT & CONFIGURATION TABLES
-- =====================================================

-- Audit log for tracking all changes
CREATE TABLE audit_logs (
    audit_id BIGSERIAL PRIMARY KEY,
    table_name VARCHAR(100) NOT NULL,
    record_id INT NOT NULL,
    action VARCHAR(20) NOT NULL CHECK (action IN ('INSERT', 'UPDATE', 'DELETE')),
    old_values JSONB,
    new_values JSONB,
    changed_by INT, -- staff_id or customer_id
    changed_by_type VARCHAR(20), -- 'staff' or 'customer'
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_audit_logs_table ON audit_logs(table_name, record_id);
CREATE INDEX idx_audit_logs_created ON audit_logs(created_at);

-- System configuration
CREATE TABLE system_settings (
    setting_id SERIAL PRIMARY KEY,
    setting_key VARCHAR(100) UNIQUE NOT NULL,
    setting_value TEXT NOT NULL,
    data_type VARCHAR(20) DEFAULT 'string' CHECK (data_type IN ('string', 'number', 'boolean', 'json')),
    description TEXT,
    is_public BOOLEAN DEFAULT FALSE, -- Can be exposed to frontend
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert default settings
INSERT INTO system_settings (setting_key, setting_value, data_type, description, is_public) VALUES
('tax_rate', '0.08', 'number', 'Sales tax rate (8%)', TRUE),
('delivery_fee', '5.99', 'number', 'Standard delivery fee', TRUE),
('minimum_delivery_order', '15.00', 'number', 'Minimum order amount for delivery', TRUE),
('pickup_discount_percentage', '0.10', 'number', 'Discount for pickup orders (10%)', TRUE),
('restaurant_name', 'My Restaurant', 'string', 'Restaurant name', TRUE),
('restaurant_phone', '+1234567890', 'string', 'Restaurant phone number', TRUE),
('restaurant_email', 'info@restaurant.com', 'string', 'Restaurant email', TRUE),
('max_concurrent_orders', '50', 'number', 'Maximum concurrent orders kitchen can handle', FALSE),
('preparation_buffer_minutes', '5', 'number', 'Buffer time added to preparation estimates', FALSE),
('loyalty_points_per_dollar', '1', 'number', 'Loyalty points earned per dollar spent', TRUE),
('loyalty_points_redemption_rate', '0.01', 'number', 'Dollar value per loyalty point ($0.01)', TRUE);

-- =====================================================
-- CATEGORIES & MENU ITEMS
-- =====================================================

CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    image_url VARCHAR(500),
    display_order INT DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    deleted_at TIMESTAMP, -- Soft delete
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE menu_items (
    item_id SERIAL PRIMARY KEY,
    category_id INT NOT NULL REFERENCES categories(category_id) ON DELETE RESTRICT,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL CHECK (price >= 0),
    image_url VARCHAR(500),
    thumbnail_url VARCHAR(500),
    is_available BOOLEAN DEFAULT TRUE,
    is_featured BOOLEAN DEFAULT FALSE,
    preparation_time_minutes INT DEFAULT 15,
    calories INT,
    spice_level INT DEFAULT 0 CHECK (spice_level BETWEEN 0 AND 5),
    is_vegetarian BOOLEAN DEFAULT FALSE,
    is_vegan BOOLEAN DEFAULT FALSE,
    is_gluten_free BOOLEAN DEFAULT FALSE,
    is_dairy_free BOOLEAN DEFAULT FALSE,
    is_nut_free BOOLEAN DEFAULT FALSE,
    allergen_info TEXT,
    max_quantity_per_order INT DEFAULT 10,
    deleted_at TIMESTAMP, -- Soft delete
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_valid_prep_time CHECK (preparation_time_minutes > 0)
);

-- Menu item option groups (e.g., "Choose your protein", "Add extras")
CREATE TABLE menu_item_option_groups (
    option_group_id SERIAL PRIMARY KEY,
    item_id INT NOT NULL REFERENCES menu_items(item_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL, -- e.g., "Choose Protein", "Select Size"
    description TEXT,
    is_required BOOLEAN DEFAULT FALSE,
    selection_type VARCHAR(20) DEFAULT 'single' CHECK (selection_type IN ('single', 'multiple')),
    min_selections INT DEFAULT 0,
    max_selections INT DEFAULT 1,
    display_order INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_valid_selections CHECK (min_selections <= max_selections)
);

-- Individual options within a group
CREATE TABLE menu_item_options (
    option_id SERIAL PRIMARY KEY,
    option_group_id INT NOT NULL REFERENCES menu_item_option_groups(option_group_id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL, -- e.g., "Chicken", "Beef", "Extra Cheese"
    price_adjustment DECIMAL(10, 2) DEFAULT 0, -- Additional cost (+$2.00) or discount (-$0.50)
    calories_adjustment INT DEFAULT 0,
    is_available BOOLEAN DEFAULT TRUE,
    is_default BOOLEAN DEFAULT FALSE,
    display_order INT DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_menu_items_category ON menu_items(category_id) WHERE deleted_at IS NULL;
CREATE INDEX idx_menu_items_available ON menu_items(is_available) WHERE deleted_at IS NULL;
CREATE INDEX idx_menu_items_featured ON menu_items(is_featured) WHERE is_featured = TRUE AND deleted_at IS NULL;
CREATE INDEX idx_option_groups_item ON menu_item_option_groups(item_id);

-- =====================================================
-- CUSTOMERS
-- =====================================================

CREATE TABLE customers (
    customer_id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone VARCHAR(20),
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    password_reset_token_hash VARCHAR(256),
    password_reset_token_expires_at TIMESTAMP,
    last_password_reset_sent_at TIMESTAMP,
    date_of_birth DATE,
    profile_image_url VARCHAR(500),
    loyalty_points INT DEFAULT 0,
    total_orders INT DEFAULT 0,
    total_spent DECIMAL(10, 2) DEFAULT 0,
    average_rating DECIMAL(3, 2), -- Rating they give to restaurant
    is_active BOOLEAN DEFAULT TRUE,
    is_verified BOOLEAN DEFAULT FALSE, -- Email/phone verified
    last_login_at TIMESTAMP,
    deleted_at TIMESTAMP, -- Soft delete
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE customer_addresses (
    address_id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers(customer_id) ON DELETE CASCADE,
    label VARCHAR(50), -- 'Home', 'Work', 'Other'
    address_line1 VARCHAR(255) NOT NULL,
    address_line2 VARCHAR(255),
    city VARCHAR(100) NOT NULL,
    state VARCHAR(50),
    postal_code VARCHAR(20) NOT NULL,
    country VARCHAR(100) DEFAULT 'USA',
    latitude DECIMAL(10, 8),
    longitude DECIMAL(11, 8),
    delivery_instructions TEXT,
    is_default BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Customer saved payment methods (tokens only, never store full card data)
CREATE TABLE customer_payment_methods (
    payment_method_id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers(customer_id) ON DELETE CASCADE,
    payment_type VARCHAR(20) NOT NULL CHECK (payment_type IN ('credit_card', 'debit_card', 'digital_wallet')),
    token VARCHAR(255) NOT NULL, -- Payment gateway token
    last_four VARCHAR(4),
    card_brand VARCHAR(20), -- Visa, Mastercard, etc.
    expiry_month INT,
    expiry_year INT,
    is_default BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_customers_email ON customers(email) WHERE deleted_at IS NULL;
CREATE INDEX idx_customers_phone ON customers(phone) WHERE deleted_at IS NULL;
CREATE INDEX idx_customer_addresses_customer ON customer_addresses(customer_id);

-- =====================================================
-- STAFF & ROLES
-- =====================================================

CREATE TABLE roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    permissions JSONB DEFAULT '[]'::jsonb -- Array of permission strings
);

INSERT INTO roles (role_name, description, permissions) VALUES
('Admin', 'Full system access', '["all"]'::jsonb),
('Manager', 'Restaurant management access', '["orders.manage", "menu.manage", "staff.view", "reports.view"]'::jsonb),
('Waiter', 'Order taking and table management', '["orders.create", "orders.view", "tables.manage"]'::jsonb),
('Chef', 'Kitchen and food preparation', '["orders.view", "orders.update_status", "inventory.view"]'::jsonb),
('Cashier', 'Payment processing', '["orders.view", "payments.process"]'::jsonb),
('Delivery', 'Delivery driver', '["orders.view", "orders.deliver"]'::jsonb);

CREATE TABLE staff (
    staff_id SERIAL PRIMARY KEY,
    role_id INT NOT NULL REFERENCES roles(role_id) ON DELETE RESTRICT,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    password_reset_token_hash VARCHAR(256),
    password_reset_token_expires_at TIMESTAMP,
    last_password_reset_sent_at TIMESTAMP,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    profile_image_url VARCHAR(500),
    hire_date DATE NOT NULL,
    employment_status VARCHAR(20) DEFAULT 'active' CHECK (employment_status IN ('active', 'on_leave', 'terminated')),
    hourly_rate DECIMAL(10, 2),
    last_login_at TIMESTAMP,
    failed_login_attempts INT DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    deleted_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_staff_email ON staff(email) WHERE deleted_at IS NULL;
CREATE INDEX idx_staff_role ON staff(role_id);

-- =====================================================
-- TABLES & RESERVATIONS
-- =====================================================

CREATE TABLE restaurant_tables (
    table_id SERIAL PRIMARY KEY,
    table_number VARCHAR(10) UNIQUE NOT NULL,
    capacity INT NOT NULL CHECK (capacity > 0),
    status VARCHAR(20) DEFAULT 'available' CHECK (status IN ('available', 'occupied', 'reserved', 'maintenance')),
    location VARCHAR(50),
    qr_code_url VARCHAR(500), -- For QR code ordering
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE reservations (
    reservation_id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers(customer_id) ON DELETE CASCADE,
    table_id INT REFERENCES restaurant_tables(table_id) ON DELETE SET NULL,
    reservation_date DATE NOT NULL,
    reservation_time TIME NOT NULL,
    party_size INT NOT NULL CHECK (party_size > 0),
    status VARCHAR(20) DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'seated', 'completed', 'cancelled', 'no_show')),
    special_requests TEXT,
    customer_notes TEXT,
    staff_notes TEXT, -- Internal notes
    confirmation_code VARCHAR(20) UNIQUE,
    reminded_at TIMESTAMP, -- When reminder was sent
    cancelled_at TIMESTAMP,
    cancellation_reason TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_reservations_customer ON reservations(customer_id);
CREATE INDEX idx_reservations_date ON reservations(reservation_date);
CREATE INDEX idx_reservations_status ON reservations(status);

-- =====================================================
-- ORDERS - ENHANCED WITH LIFECYCLE
-- =====================================================

CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    order_number VARCHAR(20) UNIQUE NOT NULL, -- User-friendly order number (e.g., #2024-001)
    customer_id INT REFERENCES customers(customer_id) ON DELETE SET NULL,
    staff_id INT REFERENCES staff(staff_id) ON DELETE SET NULL, -- Who took the order
    assigned_chef_id INT REFERENCES staff(staff_id) ON DELETE SET NULL,
    assigned_driver_id INT REFERENCES staff(staff_id) ON DELETE SET NULL,
    table_id INT REFERENCES restaurant_tables(table_id) ON DELETE SET NULL,
    
    -- Order Type: Pickup, Delivery, Dine-In
    order_type VARCHAR(20) NOT NULL CHECK (order_type IN ('pickup', 'delivery', 'dine_in')),
    
    -- Order Status Lifecycle
    order_status VARCHAR(20) DEFAULT 'pending' CHECK (order_status IN (
        'pending',           -- Order received, awaiting confirmation
        'confirmed',         -- Order confirmed by restaurant
        'preparing',         -- Being prepared in kitchen
        'ready',            -- Ready for pickup/delivery
        'out_for_delivery', -- Driver picked up (delivery only)
        'completed',        -- Order completed
        'cancelled'         -- Order cancelled
    )),
    
    -- Financial details
    subtotal DECIMAL(10, 2) NOT NULL DEFAULT 0 CHECK (subtotal >= 0),
    tax_rate DECIMAL(5, 4) DEFAULT 0.08,
    tax_amount DECIMAL(10, 2) NOT NULL DEFAULT 0 CHECK (tax_amount >= 0),
    delivery_fee DECIMAL(10, 2) DEFAULT 0 CHECK (delivery_fee >= 0),
    discount_amount DECIMAL(10, 2) DEFAULT 0 CHECK (discount_amount >= 0),
    discount_code VARCHAR(50),
    loyalty_points_used INT DEFAULT 0,
    loyalty_points_discount DECIMAL(10, 2) DEFAULT 0,
    tip_amount DECIMAL(10, 2) DEFAULT 0,
    total_amount DECIMAL(10, 2) NOT NULL CHECK (total_amount >= 0),
    
    -- Timing
    estimated_ready_time TIMESTAMP,
    actual_ready_time TIMESTAMP,
    estimated_delivery_time TIMESTAMP,
    actual_delivery_time TIMESTAMP,
    
    -- Notes
    customer_notes TEXT, -- Customer's order note
    kitchen_notes TEXT, -- Notes for kitchen staff
    delivery_notes TEXT, -- Notes for delivery driver
    
    -- Delivery specific
    delivery_address_id INT REFERENCES customer_addresses(address_id) ON DELETE SET NULL,
    delivery_latitude DECIMAL(10, 8),
    delivery_longitude DECIMAL(11, 8),
    
    -- Rating & feedback
    customer_rating INT CHECK (customer_rating BETWEEN 1 AND 5),
    customer_feedback TEXT,
    
    -- Cancellation
    cancelled_at TIMESTAMP,
    cancellation_reason TEXT,
    cancelled_by_type VARCHAR(20), -- 'customer', 'staff', 'system'
    
    -- Audit fields
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Order lifecycle tracking
CREATE TABLE order_status_history (
    history_id BIGSERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    from_status VARCHAR(20),
    to_status VARCHAR(20) NOT NULL,
    changed_by_id INT, -- staff_id or customer_id
    changed_by_type VARCHAR(20), -- 'staff', 'customer', 'system'
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE order_items (
    order_item_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    item_id INT NOT NULL REFERENCES menu_items(item_id) ON DELETE RESTRICT,
    quantity INT NOT NULL CHECK (quantity > 0),
    unit_price DECIMAL(10, 2) NOT NULL CHECK (unit_price >= 0),
    subtotal DECIMAL(10, 2) NOT NULL CHECK (subtotal >= 0),
    
    -- Customer customization note for this item
    item_notes TEXT,
    
    -- Status tracking for individual items
    item_status VARCHAR(20) DEFAULT 'pending' CHECK (item_status IN ('pending', 'preparing', 'ready', 'served')),
    
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Selected options for each order item
CREATE TABLE order_item_options (
    order_item_option_id SERIAL PRIMARY KEY,
    order_item_id INT NOT NULL REFERENCES order_items(order_item_id) ON DELETE CASCADE,
    option_id INT NOT NULL REFERENCES menu_item_options(option_id) ON DELETE RESTRICT,
    option_group_name VARCHAR(100),
    option_name VARCHAR(100),
    price_adjustment DECIMAL(10, 2) DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_orders_customer ON orders(customer_id);
CREATE INDEX idx_orders_status ON orders(order_status);
CREATE INDEX idx_orders_type ON orders(order_type);
CREATE INDEX idx_orders_created ON orders(created_at);
CREATE INDEX idx_orders_number ON orders(order_number);
CREATE INDEX idx_order_items_order ON order_items(order_id);
CREATE INDEX idx_order_status_history_order ON order_status_history(order_id);

-- =====================================================
-- PAYMENTS
-- =====================================================

CREATE TABLE payments (
    payment_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    payment_method VARCHAR(20) NOT NULL CHECK (payment_method IN ('cash', 'credit_card', 'debit_card', 'mobile_payment', 'gift_card', 'loyalty_points')),
    amount DECIMAL(10, 2) NOT NULL CHECK (amount > 0),
    payment_status VARCHAR(20) DEFAULT 'pending' CHECK (payment_status IN ('pending', 'processing', 'completed', 'failed', 'refunded', 'partially_refunded')),
    
    -- Payment gateway details
    transaction_id VARCHAR(255),
    gateway VARCHAR(50), -- 'stripe', 'square', 'paypal', etc.
    gateway_response JSONB,
    
    -- For refunds
    refund_amount DECIMAL(10, 2) DEFAULT 0,
    refund_reason TEXT,
    refunded_at TIMESTAMP,
    
    payment_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_payments_order ON payments(order_id);
CREATE INDEX idx_payments_status ON payments(payment_status);
CREATE INDEX idx_payments_transaction ON payments(transaction_id);

-- =====================================================
-- INVENTORY
-- =====================================================

CREATE TABLE inventory_categories (
    inventory_category_id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE inventory_items (
    inventory_item_id SERIAL PRIMARY KEY,
    inventory_category_id INT REFERENCES inventory_categories(inventory_category_id) ON DELETE SET NULL,
    sku VARCHAR(50) UNIQUE,
    name VARCHAR(200) NOT NULL,
    unit_of_measure VARCHAR(20) NOT NULL,
    current_quantity DECIMAL(10, 2) NOT NULL DEFAULT 0 CHECK (current_quantity >= 0),
    minimum_quantity DECIMAL(10, 2) NOT NULL DEFAULT 0 CHECK (minimum_quantity >= 0),
    reorder_quantity DECIMAL(10, 2) NOT NULL DEFAULT 0,
    unit_cost DECIMAL(10, 2) NOT NULL CHECK (unit_cost >= 0),
    supplier_name VARCHAR(200),
    supplier_contact VARCHAR(200),
    last_restocked_at TIMESTAMP,
    next_restock_date DATE,
    is_active BOOLEAN DEFAULT TRUE,
    deleted_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Inventory transaction log
CREATE TABLE inventory_transactions (
    transaction_id BIGSERIAL PRIMARY KEY,
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id) ON DELETE CASCADE,
    transaction_type VARCHAR(20) NOT NULL CHECK (transaction_type IN ('restock', 'usage', 'waste', 'adjustment', 'return')),
    quantity_change DECIMAL(10, 2) NOT NULL,
    quantity_after DECIMAL(10, 2) NOT NULL,
    unit_cost DECIMAL(10, 2),
    reference_id INT, -- Could be order_id, purchase_order_id, etc.
    reference_type VARCHAR(50),
    notes TEXT,
    staff_id INT REFERENCES staff(staff_id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE menu_item_ingredients (
    menu_item_ingredient_id SERIAL PRIMARY KEY,
    item_id INT NOT NULL REFERENCES menu_items(item_id) ON DELETE CASCADE,
    inventory_item_id INT NOT NULL REFERENCES inventory_items(inventory_item_id) ON DELETE RESTRICT,
    quantity_required DECIMAL(10, 3) NOT NULL CHECK (quantity_required > 0),
    UNIQUE(item_id, inventory_item_id)
);

CREATE INDEX idx_inventory_items_category ON inventory_items(inventory_category_id) WHERE deleted_at IS NULL;
CREATE INDEX idx_inventory_items_low_stock ON inventory_items(current_quantity) 
    WHERE current_quantity <= minimum_quantity AND deleted_at IS NULL;
CREATE INDEX idx_inventory_transactions_item ON inventory_transactions(inventory_item_id);

-- =====================================================
-- PROMOTIONS & DISCOUNTS
-- =====================================================

CREATE TABLE discount_codes (
    discount_code_id SERIAL PRIMARY KEY,
    code VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    discount_type VARCHAR(20) NOT NULL CHECK (discount_type IN ('percentage', 'fixed_amount', 'free_delivery')),
    discount_value DECIMAL(10, 2) NOT NULL,
    minimum_order_amount DECIMAL(10, 2) DEFAULT 0,
    max_uses INT,
    uses_per_customer INT DEFAULT 1,
    current_uses INT DEFAULT 0,
    valid_from TIMESTAMP NOT NULL,
    valid_until TIMESTAMP NOT NULL,
    applicable_order_types VARCHAR(50)[], -- Array: {'pickup', 'delivery'}
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE discount_code_usage (
    usage_id SERIAL PRIMARY KEY,
    discount_code_id INT NOT NULL REFERENCES discount_codes(discount_code_id) ON DELETE CASCADE,
    customer_id INT REFERENCES customers(customer_id) ON DELETE SET NULL,
    order_id INT REFERENCES orders(order_id) ON DELETE SET NULL,
    discount_amount DECIMAL(10, 2) NOT NULL,
    used_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_discount_codes_code ON discount_codes(code) WHERE is_active = TRUE;

-- =====================================================
-- NOTIFICATIONS
-- =====================================================

CREATE TABLE notifications (
    notification_id BIGSERIAL PRIMARY KEY,
    recipient_type VARCHAR(20) NOT NULL CHECK (recipient_type IN ('customer', 'staff')),
    recipient_id INT NOT NULL,
    notification_type VARCHAR(50) NOT NULL, -- 'order_confirmed', 'order_ready', 'delivery_arriving', etc.
    title VARCHAR(200) NOT NULL,
    message TEXT NOT NULL,
    related_entity_type VARCHAR(50), -- 'order', 'reservation', etc.
    related_entity_id INT,
    delivery_channel VARCHAR(20) NOT NULL CHECK (delivery_channel IN ('push', 'email', 'sms', 'in_app')),
    is_read BOOLEAN DEFAULT FALSE,
    read_at TIMESTAMP,
    sent_at TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_notifications_recipient ON notifications(recipient_type, recipient_id, is_read);

-- =====================================================
-- REVIEWS & RATINGS
-- =====================================================

CREATE TABLE reviews (
    review_id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(order_id) ON DELETE CASCADE,
    customer_id INT NOT NULL REFERENCES customers(customer_id) ON DELETE CASCADE,
    rating INT NOT NULL CHECK (rating BETWEEN 1 AND 5),
    food_rating INT CHECK (food_rating BETWEEN 1 AND 5),
    service_rating INT CHECK (service_rating BETWEEN 1 AND 5),
    delivery_rating INT CHECK (delivery_rating BETWEEN 1 AND 5),
    review_text TEXT,
    response_text TEXT, -- Restaurant's response
    responded_by_id INT REFERENCES staff(staff_id) ON DELETE SET NULL,
    responded_at TIMESTAMP,
    is_published BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_reviews_customer ON reviews(customer_id);
CREATE INDEX idx_reviews_rating ON reviews(rating);

-- =====================================================
-- TRIGGERS
-- =====================================================

-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply triggers to relevant tables
CREATE TRIGGER update_categories_updated_at BEFORE UPDATE ON categories
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_menu_items_updated_at BEFORE UPDATE ON menu_items
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_customers_updated_at BEFORE UPDATE ON customers
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_staff_updated_at BEFORE UPDATE ON staff
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_reservations_updated_at BEFORE UPDATE ON reservations
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_orders_updated_at BEFORE UPDATE ON orders
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_inventory_items_updated_at BEFORE UPDATE ON inventory_items
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_reviews_updated_at BEFORE UPDATE ON reviews
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Function to generate order number
CREATE OR REPLACE FUNCTION generate_order_number()
RETURNS TRIGGER AS $$
BEGIN
    NEW.order_number := 'ORD-' || TO_CHAR(CURRENT_TIMESTAMP, 'YYYYMMDD') || '-' || LPAD(NEW.order_id::TEXT, 5, '0');
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER set_order_number BEFORE INSERT ON orders
    FOR EACH ROW EXECUTE FUNCTION generate_order_number();

-- Function to track order status changes
CREATE OR REPLACE FUNCTION track_order_status_change()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.order_status IS DISTINCT FROM NEW.order_status THEN
        INSERT INTO order_status_history (order_id, from_status, to_status, changed_by_type, notes)
        VALUES (NEW.order_id, OLD.order_status, NEW.order_status, 'system', 'Status changed');
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER track_order_status AFTER UPDATE ON orders
    FOR EACH ROW EXECUTE FUNCTION track_order_status_change();

-- Function to update customer stats on order completion
CREATE OR REPLACE FUNCTION update_customer_stats()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.order_status = 'completed' AND (OLD.order_status IS NULL OR OLD.order_status != 'completed') THEN
        UPDATE customers 
        SET 
            total_orders = total_orders + 1,
            total_spent = total_spent + NEW.total_amount,
            loyalty_points = loyalty_points + FLOOR(NEW.total_amount)
        WHERE customer_id = NEW.customer_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_customer_stats_trigger AFTER UPDATE ON orders
    FOR EACH ROW EXECUTE FUNCTION update_customer_stats();

-- =====================================================
-- VIEWS FOR COMMON QUERIES
-- =====================================================

-- Active orders with customer info
CREATE VIEW active_orders_view AS
SELECT 
    o.order_id,
    o.order_number,
    o.order_type,
    o.order_status,
    o.total_amount,
    o.customer_notes,
    o.estimated_ready_time,
    c.first_name || ' ' || c.last_name AS customer_name,
    c.phone AS customer_phone,
    rt.table_number,
    o.created_at
FROM orders o
LEFT JOIN customers c ON o.customer_id = c.customer_id
LEFT JOIN restaurant_tables rt ON o.table_id = rt.table_id
WHERE o.order_status NOT IN ('completed', 'cancelled')
ORDER BY o.created_at DESC;

-- Low stock inventory items
CREATE VIEW low_stock_items AS
SELECT 
    ii.inventory_item_id,
    ii.name,
    ii.sku,
    ii.current_quantity,
    ii.minimum_quantity,
    ii.reorder_quantity,
    ii.unit_of_measure,
    ii.supplier_name,
    ic.name AS category_name
FROM inventory_items ii
LEFT JOIN inventory_categories ic ON ii.inventory_category_id = ic.inventory_category_id
WHERE ii.current_quantity <= ii.minimum_quantity 
    AND ii.deleted_at IS NULL
    AND ii.is_active = TRUE
ORDER BY ii.current_quantity ASC;

-- =====================================================
-- SAMPLE DATA
-- =====================================================

-- Insert categories
INSERT INTO categories (name, description, display_order) VALUES
('Appetizers', 'Start your meal with these delicious options', 1),
('Main Course', 'Our signature dishes', 2),
('Desserts', 'Sweet endings', 3),
('Beverages', 'Drinks and refreshments', 4);

-- Insert sample menu items
INSERT INTO menu_items (category_id, name, description, price, is_available, preparation_time_minutes, is_featured) VALUES
(1, 'Caesar Salad', 'Fresh romaine lettuce with parmesan and croutons', 8.99, TRUE, 10, FALSE),
(1, 'Garlic Bread', 'Toasted bread with garlic butter', 5.99, TRUE, 8, FALSE),
(2, 'Custom Burger', 'Build your own burger with our selection of toppings', 15.99, TRUE, 20, TRUE),
(2, 'Grilled Salmon', 'Atlantic salmon with seasonal vegetables', 24.99, TRUE, 25, TRUE),
(3, 'Chocolate Cake', 'Rich chocolate layer cake', 7.99, TRUE, 5, FALSE),
(4, 'Fresh Lemonade', 'House-made lemonade', 3.99, TRUE, 5, FALSE);

-- Insert option groups for Custom Burger
INSERT INTO menu_item_option_groups (item_id, name, is_required, selection_type, min_selections, max_selections, display_order) VALUES
(3, 'Choose Your Protein', TRUE, 'single', 1, 1, 1),
(3, 'Select Your Cheese', FALSE, 'single', 0, 1, 2),
(3, 'Add Extra Toppings', FALSE, 'multiple', 0, 5, 3),
(3, 'Choose Your Bun', TRUE, 'single', 1, 1, 4);

-- Insert options
INSERT INTO menu_item_options (option_group_id, name, price_adjustment, display_order, is_default) VALUES
-- Protein options
(1, 'Beef Patty', 0.00, 1, TRUE),
(1, 'Chicken Breast', 1.00, 2, FALSE),
(1, 'Veggie Patty', 0.50, 3, FALSE),
(1, 'Double Beef', 3.00, 4, FALSE),
-- Cheese options
(2, 'Cheddar', 0.00, 1, TRUE),
(2, 'Swiss', 0.50, 2, FALSE),
(2, 'Blue Cheese', 1.00, 3, FALSE),
(2, 'No Cheese', 0.00, 4, FALSE),
-- Extra toppings
(3, 'Lettuce', 0.00, 1, FALSE),
(3, 'Tomato', 0.00, 2, FALSE),
(3, 'Pickles', 0.00, 3, FALSE),
(3, 'Onions', 0.00, 4, FALSE),
(3, 'Bacon', 2.00, 5, FALSE),
(3, 'Avocado', 2.50, 6, FALSE),
(3, 'Fried Egg', 1.50, 7, FALSE),
-- Bun options
(4, 'Regular Bun', 0.00, 1, TRUE),
(4, 'Whole Wheat Bun', 0.50, 2, FALSE),
(4, 'Gluten-Free Bun', 1.50, 3, FALSE),
(4, 'Lettuce Wrap', 0.00, 4, FALSE);

-- Insert restaurant tables
INSERT INTO restaurant_tables (table_number, capacity, location, status) VALUES
('T01', 2, 'Indoor', 'available'),
('T02', 4, 'Indoor', 'available'),
('T03', 4, 'Indoor', 'available'),
('T04', 6, 'Outdoor', 'available'),
('T05', 8, 'Private Room', 'available');

-- =====================================================
-- PERFORMANCE OPTIMIZATION
-- =====================================================

-- Analyze tables for query optimization
ANALYZE;
